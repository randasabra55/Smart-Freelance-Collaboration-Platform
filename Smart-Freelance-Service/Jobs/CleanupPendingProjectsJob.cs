using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Service.Jobs
{
    public class CleanupPendingProjectsJob
    {
        Context context;
        public CleanupPendingProjectsJob(Context context)
        {
            this.context = context;
        }

        public async Task RunAsync()
        {
            var threshold = DateTime.UtcNow.AddDays(-15);
            var oldPendingProjects = await context.projects
                .Where(p => p.Status == ProjectStatus.Open && p.CreatedAt < threshold)
                .ToListAsync();

            if (oldPendingProjects.Any())
            {
                context.projects.RemoveRange(oldPendingProjects);
                await context.SaveChangesAsync();
            }
        }
    }
}
