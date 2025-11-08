using MediatR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Features.Projects.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Core.Features.Projects.Commands.UpdateProject
{
    public class UpdateProjectCommand : IRequest<Result<ProjectDto>>
    {
        public long ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Budget { get; set; }
        public DateTime Deadline { get; set; }
        //public ProjectStatus? Status { get; set; }
    }

    public class UpdateProjectCommandHandler(Context context)
        : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId);
            if (project == null)
                return ErrorCode.NotFound;
            project.Update(request.Title, request.Description, request.Budget, request.Deadline);
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success(ProjectDto.FromEntity(project));
        }
    }
}
