using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Smart_Freelance_Core.Features.Projects.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using System.Security.Claims;
using System.Text;

namespace Smart_Freelance_Core.Features.Projects.Commands.ReviewProject
{
    // [Endpoint(EndpointMethod.Post, EndpointTag.Projects, "ReviewProjectSubmission")] 
    public record ReviewProjectSubmissionCommand(long SubmissionId, bool Accepted, string? Comment) : IRequest<Result<ReviewProjectSubmissionResponseDto>>;


    public class ReviewProjectSubmissionCommandHandler(
        Context context,
        IHubContext<NotificationHub> hubContext,
        AuditLoggerService auditLogger,
        RabbitMQSettings rabbitSettings,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<ReviewProjectSubmissionCommand, Result<ReviewProjectSubmissionResponseDto>>
    {
        private readonly Context _context = context;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly AuditLoggerService _auditLogger = auditLogger;
        private readonly RabbitMQSettings _rabbitSettings = rabbitSettings;
        private readonly long _clientId = long.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        public async Task<Result<ReviewProjectSubmissionResponseDto>> Handle(ReviewProjectSubmissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // جيب الـ Submission مع الـ Project والـ Room
                var submission = await _context.ProjectSubmissions
                    .Include(s => s.Project)
                    .ThenInclude(p => p.CollaborationRoom)
                    .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

                if (submission == null || submission.Project.CollaborationRoom.ClientId != _clientId)
                {
                    return Result.Failure<ReviewProjectSubmissionResponseDto>(new Error(ErrorCode.AccessDenied, "Access denied or submission not found"));
                }


                submission.IsAccepted = request.Accepted;
                submission.ReviewComment = request.Comment;
                submission.ReviewedAt = DateTime.UtcNow;

                var project = submission.Project;
                if (request.Accepted)
                {
                    project.Status = ProjectStatus.Completed;

                    //make event to rabbit to make final payment
                    var factory = new ConnectionFactory()
                    {
                        HostName = _rabbitSettings.Host,
                        UserName = _rabbitSettings.UserName,
                        Password = _rabbitSettings.Password
                    };
                    await using var connection = await factory.CreateConnectionAsync();
                    await using var channel = await connection.CreateChannelAsync();
                    await channel.QueueDeclareAsync("ProjectCompletedQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var completionMessage = new ProjectCompletionMessage
                    {
                        ProjectId = project.Id,
                        FreelancerId = project.AssignedFreelancerId,
                        ClientId = _clientId,
                        SubmissionId = request.SubmissionId
                    };
                    var json = JsonConvert.SerializeObject(completionMessage);
                    var body = Encoding.UTF8.GetBytes(json);
                    var properties = new BasicProperties();
                    properties.Persistent = true;
                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: "ProjectCompletedQueue",
                         //mandatory
                         true,
                        basicProperties: properties,
                        body: body
                        );

                    // send to freelancer nitification
                    await _hubContext.Clients.User(project.AssignedFreelancerId.ToString()!)
                        .SendAsync("ProjectAccepted", "تم قبول الشغل! الدفع قيد المعالجة.");

                    await _auditLogger.LogAsync("Project accepted and completed event sent", "Project", project.Id);
                }
                else
                {
                    project.Status = ProjectStatus.Rejected;

                    await _hubContext.Clients.User(submission.FreelancerId.ToString())
                        .SendAsync("ProjectRejected", $"رفض الشغل: {request.Comment ?? "يرجى التعديل"}");

                    await _auditLogger.LogAsync("Project rejected", "ProjectSubmission", request.SubmissionId);
                }

                await _context.SaveChangesAsync(cancellationToken);

                var response = new ReviewProjectSubmissionResponseDto
                {
                    IsAccepted = request.Accepted,
                    Status = request.Accepted ? "Accepted" : "Rejected"
                };

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<ReviewProjectSubmissionResponseDto>(new Error(ErrorCode.BadRequest, $"Error: {ex.Message}"));
            }
        }


        private class ProjectCompletionMessage
        {
            public long ProjectId { get; set; }
            public long? FreelancerId { get; set; }
            public long ClientId { get; set; }
            public long SubmissionId { get; set; }
        }
    }
}
