using MediatR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Features.Projects.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Core.Features.Projects.Queries.GetProjectById
{
    public record GetProjectByIdQuery(long Id) : IRequest<Result<ProjectDto>>;

    public class GetProjectByIdQueryHandler(Context context)
        : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await context.projects.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (project == null)
                return ErrorCode.NotFound;
            return Result.Success(ProjectDto.FromEntity(project));
        }
    }
}
