using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Features.Projects.Dto;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using System.Security.Claims;

namespace Smart_Freelance_Core.Features.Projects.Commands.SubmitProject
{
    // [Endpoint(EndpointMethod.Post, EndpointTag.Projects, "SubmitProject")] // /api/project/submit
    public record SubmitProjectCommand(long ProjectId, string Description, string? Files) : IRequest<Result<SubmitProjectResponseDto>>;

    public class SubmitProjectCommandHandler(
        Context context,
        IHubContext<NotificationHub> hubContext,
        AuditLoggerService auditLogger,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<SubmitProjectCommand, Result<SubmitProjectResponseDto>>
    {
        private readonly Context _context = context;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly AuditLoggerService _auditLogger = auditLogger;

        private readonly long _freelancerId = long.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        public async Task<Result<SubmitProjectResponseDto>> Handle(SubmitProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.projects
                    .Include(p => p.CollaborationRoom)
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.AssignedFreelancerId == _freelancerId && p.Status == ProjectStatus.InProgress, cancellationToken);

                if (project == null)
                {
                    return Result.Failure<SubmitProjectResponseDto>(new Error(ErrorCode.NotFound, "Project not found or not in progress"));
                }


                var submission = new ProjectSubmission
                {
                    ProjectId = request.ProjectId,
                    FreelancerId = _freelancerId,
                    Description = request.Description,
                    Files = request.Files,
                    SubmittedAt = DateTime.UtcNow
                };
                await _context.ProjectSubmissions.AddAsync(submission, cancellationToken);
                // await _context.SaveChangesAsync(cancellationToken);

                project.Status = ProjectStatus.Submitted;
                await _context.SaveChangesAsync(cancellationToken);

                // send to client notify
                var clientId = project.CollaborationRoom!.ClientId;
                await _hubContext.Clients.User(clientId.ToString())
                    .SendAsync("ProjectSubmitted", new { SubmissionId = submission.Id, ProjectId = request.ProjectId, Message = "الفريلانسر قدم الشغل للمراجعة!" });

                // Audit Log
                await _auditLogger.LogAsync("Project submitted", "ProjectSubmission", submission.Id);

                var response = new SubmitProjectResponseDto
                {
                    SubmissionId = submission.Id,
                    Message = "تم تقديم الشغل بنجاح"
                };

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<SubmitProjectResponseDto>(new Error(ErrorCode.BadRequest, $"Error: {ex.Message}"));
            }
        }
    }
}
