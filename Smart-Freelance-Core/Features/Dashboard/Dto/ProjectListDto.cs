using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Core.Features.Dashboard.Dto
{
    public class ProjectListDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Budget { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CategoryName { get; set; } = null!;
        public ProjectStatus Status { get; set; } = ProjectStatus.Open;
        public long ClientId { get; set; }
        public long? AssignedFreelancerId { get; set; }

        public static ProjectListDto FromEntity(Project entity)
        {
            return new ProjectListDto
            {
                AssignedFreelancerId = entity.AssignedFreelancerId,
                Budget = entity.Budget,
                CategoryName = entity.CategoryName,
                ClientId = entity.ClientId,
                CreatedAt = entity.CreatedAt,
                Deadline = entity.Deadline.Date,
                Description = entity.Description,
                Id = entity.Id,
                Status = entity.Status,
                Title = entity.Title
            };
        }
    }
}
