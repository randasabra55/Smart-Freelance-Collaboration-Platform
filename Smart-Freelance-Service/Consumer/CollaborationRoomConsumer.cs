using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using System.Text;

namespace Smart_Freelance_Service.Consumer
{
    public class CollaborationRoomConsumer : BackgroundService
    {
        private readonly RabbitMQSettings _rabbitSettings;
        // private readonly Context _context;
        private readonly IServiceScopeFactory _scopeFactory;
        // private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<CollaborationRoomConsumer> _logger;
        // private readonly AuditLoggerService _auditLogger;

        public CollaborationRoomConsumer(
            RabbitMQSettings rabbitSettings,
            //Context context,
            IServiceScopeFactory scopeFactory,
            // IHubContext<NotificationHub> hubContext,
            ILogger<CollaborationRoomConsumer> logger)
        //AuditLoggerService auditLogger)
        {
            _rabbitSettings = rabbitSettings;
            //_context = context;
            _scopeFactory = scopeFactory;
            //_hubContext = hubContext;
            _logger = logger;
            // _auditLogger = auditLogger;
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
                queue: "ProposalAcceptedQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueDeclareAsync(
                queue: "InitialPaymentQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var auditLogger = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonConvert.DeserializeObject<ProposalAcceptedMessage>(json);

                    if (message == null) return;

                    //create collaboration room
                    CollaborationRoom room;
                    var existingRoom = await context.CollaborationRooms
                          .FirstOrDefaultAsync(r => r.ProjectId == message.ProjectId);
                    if (existingRoom == null)
                    {
                        room = new CollaborationRoom
                        {
                            ProjectId = message.ProjectId,
                            ClientId = message.ClientId,
                            FreelancerId = message.FreelancerId,
                            CreatedAt = DateTime.UtcNow
                        };

                        await context.CollaborationRooms.AddAsync(room);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        room = existingRoom;
                        _logger.LogInformation($"Room already exists for project {message.ProjectId}");
                    }
                    //to include projects to avoid null expection error
                    //نوع اللود explicit
                    await context.Entry(room).Reference(r => r.Project).LoadAsync();
                    var notificationData = new
                    {
                        Message = $"You have a new collaboration room for project {room.Project.Title}",
                        Room = room
                        //for clarification about room id
                        /*Room = new
                        {
                            room.Id,
                            room.ProjectId,
                            room.ClientId,
                            room.FreelancerId,
                            room.CreatedAt
                        }*/
                    };
                    //notify both users
                    await hubContext.Clients.User(message.ClientId.ToString())
                        .SendAsync("CollaborationRoomCreated", notificationData);
                    await hubContext.Clients.User(message.FreelancerId.ToString())
                        .SendAsync("CollaborationRoomCreated", notificationData);

                    await auditLogger.LogAsync("Collaboration room created", "CollaborationRoom", room.Id);

                    _logger.LogInformation($"[Consumer] Collaboration room created for Project {message.ProjectId}");

                    //////////////////////////////////////////////////////////////////////////////////////////
                    // الجزء الجديد: إرسال event للدفع الأولي

                    var proposal = await context.Proposals.FindAsync(message.ProposalId);
                    if (proposal == null)
                    {
                        _logger.LogWarning($"Proposal {message.ProposalId} not found");
                        return;
                    }

                    var initialAmount = proposal.ProposedBudget * 0.20m;

                    var paymentMessage = new InitialPaymentMessage
                    {
                        ProjectId = message.ProjectId,
                        ClientId = message.ClientId,
                        FreelancerId = message.FreelancerId,
                        Amount = initialAmount,
                        Currency = "EGP"
                    };

                    var jsonPayment = JsonConvert.SerializeObject(paymentMessage);
                    var bodyPayment = Encoding.UTF8.GetBytes(jsonPayment);


                    var properties = new BasicProperties();
                    properties.Persistent = true;
                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: "InitialPaymentQueue",
                        mandatory: true,
                        basicProperties: properties,
                        body: bodyPayment);

                    _logger.LogInformation($"[Consumer] Sent initial payment request for Project {message.ProjectId}, Amount: {initialAmount} EGP");
                    await auditLogger.LogAsync($"[Consumer] Sent initial payment request for Project {message.ProjectId}, Amount: {initialAmount} EGP", "Payment", proposal.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[Consumer] Error creating room: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: "ProposalAcceptedQueue", autoAck: true, consumer: consumer);
        }

        private class ProposalAcceptedMessage
        {
            public long ProposalId { get; set; }
            public long ProjectId { get; set; }
            public long FreelancerId { get; set; }
            public long ClientId { get; set; }
        }

        public class InitialPaymentMessage
        {
            public long ProjectId { get; set; }
            public long ClientId { get; set; }
            public long FreelancerId { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "EGP";
        }
    }
}
