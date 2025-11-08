
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using X.Paymob.CashIn;

namespace Smart_Freelance_Core.Features.Payment.Commands
{
    //[Endpoint(EndpointMethod.Post, EndpointTag.Payment, "PaymobWebhook")]
    public record PaymobWebhookCommand(string Hmac, dynamic Callback) : IRequest<Result<string>>;


    public class PaymobWebhookCommandHandler(
        Context context,
        IPaymobCashInBroker paymobBroker,
        IHubContext<NotificationHub> hubContext,
        AuditLoggerService auditLogger,
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
        : IRequestHandler<PaymobWebhookCommand, Result<string>>
    {
        private readonly Context _context = context;
        private readonly IPaymobCashInBroker _paymobBroker = paymobBroker;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly AuditLoggerService _auditLogger = auditLogger;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger _logger = loggerFactory.CreateLogger("PaymobWebhook");

        public async Task<Result<string>> Handle(PaymobWebhookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var hmacSecret = _configuration["Paymob:Hmac"];

                // 1. تحقق HMAC للأمان (manual بـ HMACSHA256)
                using var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(hmacSecret ?? ""));
                var contentBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request.Callback));
                var computedHmac = Convert.ToBase64String(hmacSha256.ComputeHash(contentBytes));
                var isValid = computedHmac == request.Hmac;

                if (!isValid)
                {
                    _logger.LogWarning("Invalid HMAC in webhook");
                    return Result.Failure<string>(new Error(ErrorCode.InvalidInput, "Invalid HMAC"));
                }

                using var doc = JsonDocument.Parse(JsonSerializer.Serialize(request.Callback));
                var root = doc.RootElement;

                var callbackType = root.GetProperty("type").GetString();
                if (callbackType == "transaction")
                {
                    var obj = root.GetProperty("obj");
                    var transId = obj.GetProperty("id").GetString();
                    var success = obj.GetProperty("success").GetBoolean();
                    var status = success ? TransactionStatus.Success : TransactionStatus.Failed;

                    var transactions = await _context.Transactions
                        .ToListAsync(cancellationToken);
                    var transaction = transactions.FirstOrDefault(t => t.PaymentGatewayTransactionId == transId);
                    if (transaction != null)
                    {
                        transaction.Status = status;
                        transaction.CompletedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync(cancellationToken);

                        await _auditLogger.LogAsync($"Payment {status}", "Transaction", transaction.Id);

                        if (status == TransactionStatus.Success)
                        {
                            await _hubContext.Clients.User(transaction.UserId.ToString())
                                .SendAsync("PaymentSuccess", "تم الدفع الأولي بنجاح! يمكنك البدء في الشغل.", cancellationToken);

                            var room = await _context.CollaborationRooms
                                .FirstOrDefaultAsync(r => r.ProjectId == transaction.ProjectId, cancellationToken);
                            if (room != null)
                            {
                                await _hubContext.Clients.User(room.FreelancerId.ToString()!)
                                    .SendAsync("PaymentSuccess", "تم الدفع الأولي، ابدأ الشغل مع العميل!", cancellationToken);
                            }

                            var project = await _context.projects.FindAsync(transaction.ProjectId, cancellationToken);
                            if (project != null)
                            {
                                project.Status = ProjectStatus.InProgress;
                                await _context.SaveChangesAsync(cancellationToken);
                            }
                        }
                        else
                        {
                            await _hubContext.Clients.User(transaction.UserId.ToString())
                                .SendAsync("PaymentFailed", "فشل الدفع، يرجى المحاولة مرة أخرى.", cancellationToken);
                        }

                        _logger.LogInformation($"Webhook processed: Transaction {transaction.Id} {status}");
                    }
                }

                var Message = "Webhook processed successfully";
                /* var response = new PaymobWebhookResponseDto
                 {
                     Message = "Webhook processed successfully"
                 };*/

                return Result.Success(Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Webhook error: {ex.Message}");
                return Result.Failure<string>(new Error(ErrorCode.BadRequest, $"Error: {ex.Message}"));
            }
        }
    }
}


