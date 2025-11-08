using MediatR;
using Microsoft.AspNetCore.Http;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Core.Features.Projects.Dto;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using System.Security.Claims;

namespace Smart_Freelance_Core.Features.Projects.Commands.CreateProject
{
    // [Endpoint(EndpointMethod.Post, EndpointTag.Projects, "Create")]
    public record CreateProjectCommand(string Title, string Description, decimal Budget, DateTime Deadline, string CategoryName) : IRequest<Result<ProjectDto>>;

    public class CreateProjectCommandHandler(Context context, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var clientId = long.Parse(httpContextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (clientId == null)
                return ErrorCode.NotAuthorized;
            var project = new Project
            {
                ClientId = clientId,
                Title = request.Title,
                Description = request.Description,
                Budget = request.Budget,
                Deadline = request.Deadline,
                CategoryName = request.CategoryName
            };
            await context.projects.AddAsync(project);
            await context.SaveChangesAsync(cancellationToken);
            //publich event 
            await mediator.Publish(new ProjectCreatedEvent(project), cancellationToken);

            return Result.Success(ProjectDto.FromEntity(project));
        }
    }
}
