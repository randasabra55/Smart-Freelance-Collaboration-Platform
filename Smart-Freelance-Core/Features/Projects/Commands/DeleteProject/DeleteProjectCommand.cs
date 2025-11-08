using MediatR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Core.Features.Projects.Commands.DeleteProject
{
    public record DeleteProjectCommand(long ProjectId) : IRequest<Result>;

    public class DeleteProjectCommandHandler(Context context)
        : IRequestHandler<DeleteProjectCommand, Result>
    {
        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId);
            if (project == null)
                return ErrorCode.NotFound;
            context.Remove(project);
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
