using MediatR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Features.Dashboard.Dto;
using Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination.PaginationFeature;
using Smart_Freelance_Infrastructure.Common.Pagination.Service;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination
{
    public class GetProjectsPaginatedQuery : ProjectPaginationModel, IRequest<Result<PaginatedList<ProjectListDto>>> { }

    public class GetProjectPaginatedQueryHandler(
        IPaginationService paginationService,
        Context context)
        : IRequestHandler<GetProjectsPaginatedQuery, Result<PaginatedList<ProjectListDto>>>
    {
        public async Task<Result<PaginatedList<ProjectListDto>>> Handle(GetProjectsPaginatedQuery request, CancellationToken cancellationToken)
        {
            var result = await paginationService
                .For(context.projects.Include(c => c.AssignedFreelancer).Include(c => c.Client))
                .WithModel(request)
                .WithSearch(ProjectSearchConfiguration.GetConfiguration())
                .WithPredicate(ProjectPredicateBuilder.BuildPredicate)
                .WithOrdering(ProjectOrderingProvider.ApplyOrdering)
                .SelectAsync(ProjectSelector.Selector);

            return Result.Success(result);

        }
    }
}
