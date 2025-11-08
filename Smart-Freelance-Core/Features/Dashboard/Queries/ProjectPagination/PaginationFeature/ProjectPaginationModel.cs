using Smart_Freelance_Core.Features.Dashboard.Dto;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Pagination.BuilderPattern;
using System.Linq.Expressions;

namespace Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination.PaginationFeature
{
    public class ProjectPaginationModel : BasePaginationModel
    {
        public long? Id { get; set; }
        public string? Title { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string? CategoryName { get; set; } = null!;
        public ProjectStatus? Status { get; set; }
        public long? ClientId { get; set; }
        public long? AssignedFreelancerId { get; set; }




        public ProjectOrderBy OrderBy { get; set; } = ProjectOrderBy.Title;
    }
    public class ProjectSelector
    {
        public static Expression<Func<Project, ProjectListDto>> Selector =>
            s => new ProjectListDto
            {
                AssignedFreelancerId = s.AssignedFreelancerId,
                Budget = s.Budget,
                CategoryName = s.CategoryName,
                ClientId = s.ClientId,
                CreatedAt = s.CreatedAt,
                Deadline = s.Deadline.Date,
                Description = s.Description,
                Id = s.Id,
                Status = s.Status,
                Title = s.Title
            };
    }
}