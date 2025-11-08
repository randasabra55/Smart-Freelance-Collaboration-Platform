using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using Smart_Freelance_Service.Abstracts;
using System.Text;

namespace Smart_Freelance_Service.Consumer
{
    public class ProposalNotificationConsumer
    {
        private readonly RabbitMQSettings _rabbitSettings;
        private readonly IServiceProvider _serviceProvider;

        /* private readonly Context _context;
         UserManager<ApplicationUser> userManager;
         private readonly IHubContext<NotificationHub> _hubContext;
         AuditLoggerService auditLogger;*/
        ILogger<ProposalNotificationConsumer> logger;

        public ProposalNotificationConsumer(RabbitMQSettings rabbitSettings, IServiceProvider serviceProvider, ILogger<ProposalNotificationConsumer> logger)
        {
            _rabbitSettings = rabbitSettings;
            _serviceProvider = serviceProvider;
            /*  _context = context;
              _hubContext = hubContext;*/
            this.logger = logger;
            /* this.userManager = userManager;
             this.auditLogger = auditLogger;*/
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
                queue: "ProposalSubmittedQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();

                var _context = scope.ServiceProvider.GetRequiredService<Context>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var auditLogger = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();
                // var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var proposal = JsonConvert.DeserializeObject<Proposal>(message);

                    if (proposal == null) return;

                    var project = await _context.projects.FindAsync(proposal.ProjectId);
                    if (project == null) return;

                    var clientId = project.ClientId;
                    var client = await userManager.Users.FirstOrDefaultAsync(u => u.Id == clientId);
                    //save notification in DB
                    var notif = new Notification
                    {
                        UserId = clientId,
                        Message = $"New proposal received for project: {project.Title}"
                    };
                    await _context.Notifications.AddAsync(notif);
                    await _context.SaveChangesAsync();

                    //check client online or off
                    // if online then send only notofication
                    //if offline send him email
                    var isClientOnline = _hubContext.Clients.User(clientId.ToString()) != null;
                    if (isClientOnline)
                    {
                        //send notification to client
                        await _hubContext.Clients.User(clientId.ToString())
                            .SendAsync("ReceiveNotification", notif);

                        //registter audit log
                        await auditLogger.LogAsync("ProposalNotification Sent", "Proposal", proposal.Id, clientId);

                        Console.WriteLine($"[Consumer] Sent notification for proposal {proposal.Id}");
                        logger.LogInformation($"[Consumer] Sent notification for proposal {proposal.Id}");
                    }
                    else
                    {
                        //send email via Hangfire 
                        BackgroundJob.Schedule<IEmailSender>(
                            x => x.SendAsync(client!.Email!, "New Proposal Received", $"Your project {project.Title} got a new proposal!"),
                            TimeSpan.FromMinutes(2)
                            );

                        //registter audit log for email
                        await auditLogger.LogAsync("ProposalEmail Sent", "Proposal", proposal.Id, clientId);
                    }



                }
                catch (Exception ex)
                {
                    var proposal = JsonConvert.DeserializeObject<Proposal>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    //registter audit log for failure
                    await auditLogger.LogAsync("ProposalNotification Failed", "Proposal", proposal.Id);
                    Console.WriteLine($"[Consumer] Error handling proposal notification: {ex.Message}");
                    logger.LogError($"[Consumer] Error handling proposal notification: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(
                queue: "ProposalSubmittedQueue",
                autoAck: false,
                consumer: consumer);
        }
    }
}
