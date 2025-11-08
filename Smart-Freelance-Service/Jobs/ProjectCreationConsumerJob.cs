using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using Smart_Freelance_Service.Consumer;

namespace Smart_Freelance_Service.Jobs
{
    public class ProjectCreationConsumerJob : BackgroundService
    {
        IServiceProvider _serviceProvider;
        RabbitMQSettings _rabbitSettings;
        ILogger<ProjectCreationNotificationConsumer> _logger;

        public ProjectCreationConsumerJob(IServiceProvider serviceProvider,
                                          RabbitMQSettings rabbitSettings,
                                          ILogger<ProjectCreationNotificationConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _rabbitSettings = rabbitSettings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
            var audit = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();
            var log = scope.ServiceProvider.GetRequiredService<ILogger<ProjectCreationNotificationConsumer>>();

            var consumer = new ProjectCreationNotificationConsumer(_serviceProvider, _rabbitSettings, _logger);
            await consumer.StartListening();
        }
    }
}
