using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Service.Consumer;


namespace Smart_Freelance_Service.Jobs
{
    public class ProposalNotificationConsumerJob : BackgroundService
    {
        IServiceProvider _serviceProvider;
        RabbitMQSettings _rabbitSettings;

        public ProposalNotificationConsumerJob(IServiceProvider serviceProvider, RabbitMQSettings rabbitSettings)
        {
            _serviceProvider = serviceProvider;
            _rabbitSettings = rabbitSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            /* var context = scope.ServiceProvider.GetRequiredService<Context>();
             var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
             var audit = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();*/
            var log = scope.ServiceProvider.GetRequiredService<ILogger<ProposalNotificationConsumer>>();
            // var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var consumer = new ProposalNotificationConsumer(_rabbitSettings, _serviceProvider, log);
            await consumer.StartListening();
        }
    }
}
