using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Data.Helpers;
using System.Text;

namespace Smart_Freelance_Core.EventHandlers.Notifications
{

    //consumer in class notification consumer to apply logic operation

    //publisher (publish messages to rabbitMq)  دا كلاس
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly RabbitMQSettings _rabbitSettings;
        private readonly ILogger<ProjectCreatedEventHandler> _logger;


        public ProjectCreatedEventHandler(
            RabbitMQSettings rabbitSettings,
            ILogger<ProjectCreatedEventHandler> logger)
        {
            _rabbitSettings = rabbitSettings;
            _logger = logger;

        }

        public async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(">>> ProjectCreatedEventHandler triggered!");
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitSettings.Host,
                    UserName = _rabbitSettings.UserName,
                    Password = _rabbitSettings.Password,
                    VirtualHost = "/",
                    Port = 5672
                };

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: _rabbitSettings.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var json = JsonConvert.SerializeObject(notification.Project);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties();
                properties.Persistent = true;
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: _rabbitSettings.QueueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"[RabbitMQ] Sent ProjectCreatedEvent: {notification.Project.Title}");


                /* var audit = new AuditLog
                 {
                     Action = "ProjectCreatedEvent Sent",
                     EntityName = "Project",
                     EntityId = notification.Project.Id,
                     CreatedAt = DateTime.UtcNow
                 };
                 _context.AuditLogs.Add(audit);
                 await _context.SaveChangesAsync();*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while publishing ProjectCreatedEvent to RabbitMQ");

                /*var audit = new AuditLog
                {
                    Action = "RabbitMQ Publish Failed",
                    EntityName = "Project",
                    EntityId = notification.Project.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.AuditLogs.Add(audit);
                await _context.SaveChangesAsync();*/
            }
        }
    }
}



