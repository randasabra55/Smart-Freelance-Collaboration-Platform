using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Core.Features.Projects.Dto
{
    public class ProjectDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Budget { get; set; }
        public DateTime Deadline { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Open;

        public static ProjectDto FromEntity(Project entity)
        {
            return new ProjectDto
            {
                Id = entity.Id,
                Budget = entity.Budget,
                Deadline = entity.Deadline,
                Description = entity.Description,
                Status = entity.Status,
                Title = entity.Title
            };
        }
    }
}
