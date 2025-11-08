using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
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

namespace Smart_Freelance_Service.Consumer
{
    public class ProjectCreationNotificationConsumer
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _rabbitSettings;


        private readonly ILogger<ProjectCreationNotificationConsumer> logger;

        public ProjectCreationNotificationConsumer(IServiceProvider serviceProvider, RabbitMQSettings rabbitSettings, ILogger<ProjectCreationNotificationConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _rabbitSettings = rabbitSettings;
            this.logger = logger;
        }

        public async Task StartListening()
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
                queue: _rabbitSettings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<Context>();
                var _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var _auditLogger = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var project = JsonConvert.DeserializeObject<Project>(message);

                    var freelancers = _context.Users
                       .Where(u => u.Role == UserType.Freelancer && u.Specialization == project.CategoryName)
                       .ToList();

                    foreach (var freelancer in freelancers)
                    {
                        var notif = new Notification
                        {
                            UserId = freelancer.Id,
                            Message = $"New project posted: {project.Title}"
                        };
                        await _context.Notifications.AddAsync(notif);
                        await _hubContext.Clients.User(freelancer.Id.ToString())
                            .SendAsync("ReceiveNotification", notif);
                    }
                    //  await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[NotificationService] Notification sent for project {project.Title}");
                    logger.LogInformation($"[NotificationService] Notification sent for project {project.Title}");

                    //if el donia tamam register audit
                    await _auditLogger.LogAsync(
                        action: "Notification Sent for Project",
                        entityId: project.Id,
                        entityName: "Project");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NotificationService] Error processing notification for project: {ex.Message}");
                    logger.LogError(ex, "Error in NotificationConsumer");
                    //when happen problem
                    var project = JsonConvert.DeserializeObject<Project>(Encoding.UTF8.GetString(ea.Body.ToArray())); // إعادة deserialize للحصول على Id في حالة الخطأ
                    await _auditLogger.LogAsync(
                        action: "Notification Failed for Project",
                        entityName: "Project",
                        entityId: project?.Id ?? 0
                        );
                    //ترجع المسج للكيو تانى 
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: _rabbitSettings.QueueName,
                autoAck: false,
                consumer: consumer);
        }
    }
}