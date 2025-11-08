using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Data.Helpers;
using System.Text;

namespace Smart_Freelance_Core.EventHandlers.Notifications
{
    public class ProposalSubmittedEventHandler : INotificationHandler<ProposalSubmittedEvent>
    {
        private readonly RabbitMQSettings _rabbitSettings;
        private readonly ILogger<ProposalSubmittedEventHandler> _logger;

        public ProposalSubmittedEventHandler(RabbitMQSettings rabbitSettings, ILogger<ProposalSubmittedEventHandler> logger)
        {
            _rabbitSettings = rabbitSettings;
            _logger = logger;
        }

        public async Task Handle(ProposalSubmittedEvent notification, CancellationToken cancellationToken)
        {
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
                    queue: "ProposalSubmittedQueue",

                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                //var json = JsonConvert.SerializeObject(notification.proposal);
                var json = JsonConvert.SerializeObject(notification.proposal, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties { Persistent = true };
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "ProposalSubmittedQueue",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"[RabbitMQ] Published ProposalSubmittedEvent for Proposal ID: {notification.proposal.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing ProposalSubmittedEvent to RabbitMQ");
            }
        }
    }
}



