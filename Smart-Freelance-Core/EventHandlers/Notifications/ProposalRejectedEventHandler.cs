using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.EventHandlers.Notifications
{
    public class ProposalRejectedEventHandler(
        Context context,
        IHubContext<NotificationHub> hubContext,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailService,
        AuditLoggerService auditLogger)
        : INotificationHandler<ProposalRejectedEvent>
    {
        public async Task Handle(ProposalRejectedEvent notification, CancellationToken cancellationToken)
        {
            var project = await context.projects.FindAsync(notification.ProjectId);
            if (project == null) return;

            var freelancer = await userManager.Users.FirstOrDefaultAsync(u => u.Id == notification.FreelancerId);

            var notif = new Notification
            {
                UserId = notification.FreelancerId,
                Message = $"Your proposal for project '{project.Title}' was rejected."
            };

            await context.Notifications.AddAsync(notif, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            //regisre audit
            await auditLogger.LogAsync("Proposal rejected notification sent", "Proposal", notification.ProposalId, notification.FreelancerId);

            //check if user online
            //var isOnline = ConnectedUsersRepository.IsOnline(notification.FreelancerId);
            var isOnline = hubContext.Clients.User(notification.FreelancerId.ToString()) != null;
            if (isOnline)
            {
                await hubContext.Clients.User(notification.FreelancerId.ToString())
                    .SendAsync("ReceiveNotification", notif, cancellationToken);
            }
            else
            {
                //send email instead
                BackgroundJob.Schedule(() =>
                    emailService.SendAsync(freelancer!.Email!, "Proposal Rejected", notif.Message),
                    TimeSpan.FromMinutes(2));
            }
        }
    }

}
