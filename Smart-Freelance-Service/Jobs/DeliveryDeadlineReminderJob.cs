using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;

namespace Smart_Freelance_Service.Jobs
{
    public class DeliveryDeadlineReminderJob
    {
        private readonly Context _context;
        private readonly IHubContext<NotificationHub> hubContext;

        public DeliveryDeadlineReminderJob(Context context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
        }

        public async Task RunAsync()
        {
            var upcoming = DateTime.UtcNow.AddDays(1);
            var dueProjects = await _context.projects
                .Where(p => p.Deadline <= upcoming && p.Status == ProjectStatus.InProgress)
                .ToListAsync();

            foreach (var project in dueProjects)
            {
                var message = $"Reminder: Project '{project.Title}' deadline is approaching ({project.Deadline:dd MMM}).";
                await hubContext.Clients.User(project.ClientId.ToString())
                .SendAsync("NotificationReceived", message);

                await hubContext.Clients.User(project.AssignedFreelancerId.ToString()!)
                    .SendAsync("NotificationReceived", message);

            }
        }
    }
}
