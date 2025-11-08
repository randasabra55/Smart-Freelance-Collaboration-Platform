using Smart_Freelance_Data.Entities;
using Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms;
using Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms.Configuration;

namespace Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination.PaginationFeature
{
    public static class ProjectSearchConfiguration
    {
        public static ISearchConfiguration<Project> GetConfiguration()
        {
            return new SearchConfiguration<Project>()
            .AddField(p => p.Title, SearchMatchType.Contains, priority: 1, scoreMultiplier: 3.0)
            .AddField(p => p.Description, SearchMatchType.Contains, priority: 2, scoreMultiplier: 2.5)
            .AddField(p => p.CategoryName, SearchMatchType.Contains, priority: 3, scoreMultiplier: 2.0);

        }
    }
}
