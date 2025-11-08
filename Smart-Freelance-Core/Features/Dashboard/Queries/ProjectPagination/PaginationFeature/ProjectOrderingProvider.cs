using Smart_Freelance_Data.Entities;

namespace Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination.PaginationFeature
{
    public enum ProjectOrderBy
    {
        Id,
        Title,
        Budject,
        Deadline,
        Status,
        CreatedAt
    }
    public class ProjectOrderingProvider
    {
        public static IQueryable<Project> ApplyOrdering(ProjectPaginationModel model, IQueryable<Project> query)
        {
            return model.OrderBy switch
            {
                ProjectOrderBy.Id => model.IsDescending
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id),

                ProjectOrderBy.Title => model.IsDescending
                    ? query.OrderByDescending(c => c.Title)
                    : query.OrderBy(c => c.Title),

                ProjectOrderBy.Budject => model.IsDescending
                    ? query.OrderByDescending(c => c.Budget)
                    : query.OrderBy(c => c.Budget),

                ProjectOrderBy.Deadline => model.IsDescending
                ? query.OrderByDescending(c => c.Deadline)
                : query.OrderBy(c => c.Deadline),

                ProjectOrderBy.Status => model.IsDescending
                ? query.OrderByDescending(c => c.Status)
                : query.OrderBy(c => c.Status),

                ProjectOrderBy.CreatedAt => model.IsDescending
                    ? query.OrderByDescending(c => c.CreatedAt)
                    : query.OrderBy(c => c.CreatedAt),

                _ => model.IsDescending
                    ? query.OrderByDescending(c => c.Title)
                    : query.OrderBy(c => c.Title)
            };
        }
    }
}
