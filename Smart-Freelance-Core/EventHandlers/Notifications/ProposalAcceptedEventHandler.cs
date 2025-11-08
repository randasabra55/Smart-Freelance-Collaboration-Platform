using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Data.Helpers;
using System.Text;

namespace Smart_Freelance_Core.EventHandlers.Notifications
{
    public class ProposalAcceptedEventHandler(RabbitMQSettings rabbitSettings)
        : INotificationHandler<ProposalAcceptedEvent>
    {
        public async Task Handle(ProposalAcceptedEvent notification, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitSettings.Host,
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                VirtualHost = "/",
                Port = 5672
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("ProposalAcceptedQueue", durable: true, exclusive: false, autoDelete: false);

            var json = JsonConvert.SerializeObject(notification);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: "ProposalAcceptedQueue", body: body);

            Console.WriteLine($"[Publisher] Sent message for accepted proposal {notification.ProposalId}");
        }
    }

}
