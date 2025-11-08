using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using System.Text;
using X.Paymob.CashIn;

namespace Smart_Freelance_Service.Consumer
{
    public class CompletionPaymentConsumer : BackgroundService
    {
        private readonly RabbitMQSettings _rabbitSettings;
        private readonly IServiceScopeFactory _scopeFactory;
        // private readonly Context _context;
        //  private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<CompletionPaymentConsumer> _logger;
        //   private readonly AuditLoggerService _auditLogger;
        private readonly IPaymobCashInBroker _paymobBroker;
        private readonly IConfiguration _configuration;

        public CompletionPaymentConsumer(
            RabbitMQSettings rabbitSettings,
            IServiceScopeFactory scopeFactory,
            //   Context context,
            //    IHubContext<NotificationHub> hubContext,
            ILogger<CompletionPaymentConsumer> logger,
        //    AuditLoggerService auditLogger,
            IPaymobCashInBroker paymobBroker,
            IConfiguration configuration)
        {
            _rabbitSettings = rabbitSettings;
            _scopeFactory = scopeFactory;
            //  _context = context;
            //  _hubContext = hubContext;
            _logger = logger;
            //   _auditLogger = auditLogger;
            _paymobBroker = paymobBroker;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitSettings.Host,
                UserName = _rabbitSettings.UserName,
                Password = _rabbitSettings.Password,
                VirtualHost = "/",
                Port = 5672

            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();


            await channel.QueueDeclareAsync(
                queue: "ProjectCompletedQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<Context>();
                var _auditLogger = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();
                var _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonConvert.DeserializeObject<ProjectCompletionMessage>(json);
                    if (message == null) return;

                    _logger.LogInformation($"[CompletionConsumer] Processing completion for Project {message.ProjectId}");

                    //1 تحقق من الدفع الأولي
                    var initialTrans = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.ProjectId == message.ProjectId && t.Type == TransactionType.Deposit && t.Status == TransactionStatus.Success);
                    if (initialTrans == null)
                    {
                        _logger.LogWarning($"No successful deposit for Project {message.ProjectId}");
                        return;
                    }

                    var proposal = await _context.Proposals.FirstOrDefaultAsync(p => p.ProjectId == message.ProjectId);
                    if (proposal == null)
                    {
                        _logger.LogWarning($"No proposal for Project {message.ProjectId}");
                        return;
                    }
                    var totalAmount = proposal.ProposedBudget;

                    //3 احسب العمولة (15%) و Payout
                    var commissionRate = 0.15m;
                    var commission = totalAmount * commissionRate;
                    var payoutAmount = totalAmount - initialTrans.Amount - commission;


                    var payoutTrans = new Transaction
                    {
                        UserId = message.FreelancerId,
                        ProjectId = message.ProjectId,
                        Type = TransactionType.Payout,
                        Currency = "EGP",
                        Amount = payoutAmount,
                        CommissionAmount = null,
                        Status = TransactionStatus.Success,
                        CreatedAt = DateTime.UtcNow,
                        CompletedAt = DateTime.UtcNow
                    };
                    await _context.Transactions.AddAsync(payoutTrans);

                    var commissionTrans = new Transaction
                    {
                        UserId = 0, // النظام
                        ProjectId = message.ProjectId,
                        Type = TransactionType.Commission,
                        Currency = "EGP",
                        Amount = commission,
                        CommissionAmount = commission,
                        Status = TransactionStatus.Success,
                        CreatedAt = DateTime.UtcNow,
                        CompletedAt = DateTime.UtcNow
                    };
                    await _context.Transactions.AddAsync(commissionTrans);

                    var refundAmount = initialTrans.Amount - payoutAmount - commission;
                    if (refundAmount > 0)
                    {
                        var refundTrans = new Transaction
                        {
                            UserId = message.ClientId,
                            ProjectId = message.ProjectId,
                            Type = TransactionType.Refund,
                            Currency = "EGP",
                            Amount = refundAmount,
                            Status = TransactionStatus.Success,
                            CreatedAt = DateTime.UtcNow,
                            CompletedAt = DateTime.UtcNow
                        };
                        await _context.Transactions.AddAsync(refundTrans);

                        // Paymob Refund (اختياري، أضيفيه لو عايزة)
                        // await _paymobBroker.RefundTransactionAsync(initialTrans.PaymentGatewayTransactionId, (int)(refundAmount * 100));
                    }

                    await _context.SaveChangesAsync();

                    await _hubContext.Clients.User(message.FreelancerId.ToString())
                        .SendAsync("PayoutReceived", $"تم صرف {payoutAmount} EGP لك بعد خصم العمولة.");
                    await _hubContext.Clients.User(message.ClientId.ToString())
                        .SendAsync("ProjectCompleted", "تم إكمال المشروع وصرف الدفع للفريلانسر.");

                    // 8. Audit Logs
                    await _auditLogger.LogAsync("Payout processed", "Transaction", payoutTrans.Id);
                    await _auditLogger.LogAsync("Commission deducted", "Transaction", commissionTrans.Id);
                    if (refundAmount > 0)
                    {
                        // await _auditLogger.LogAsync("Refund processed", "Transaction", refundTrans.Id);
                    }

                    _logger.LogInformation($"[CompletionConsumer] Payout {payoutAmount} EGP for Project {message.ProjectId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[CompletionConsumer] Error: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: "ProjectCompletedQueue", autoAck: true, consumer: consumer);
        }

        private class ProjectCompletionMessage
        {
            public long ProjectId { get; set; }
            public long FreelancerId { get; set; }
            public long ClientId { get; set; }
            public long SubmissionId { get; set; }
        }
    }
}
