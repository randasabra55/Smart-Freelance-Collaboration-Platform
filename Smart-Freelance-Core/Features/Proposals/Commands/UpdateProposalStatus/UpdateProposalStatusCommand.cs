using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Core.Features.Proposals.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Services;
using System.Security.Claims;

namespace Smart_Freelance_Core.Features.Proposals.Commands.UpdateProposalStatus
{
    public record UpdateProposalStatusCommand(long ProposalId/*, long ClientId*/) : IRequest<Result<ProposalDto>>;

    public class UpdateProposalStatusCommandHandler(Context context, IMediator mediator, AuditLoggerService auditLogger, IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<UpdateProposalStatusCommand, Result<ProposalDto>>
    {
        public async Task<Result<ProposalDto>> Handle(UpdateProposalStatusCommand request, CancellationToken cancellationToken)
        {
            var clientId = long.Parse(httpContextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (clientId == null)
                return ErrorCode.NotAuthorized;

            var proposal = await context.Proposals
                 .Include(p => p.Project)
                  .Include(p => p.Freelancer)
                 .FirstOrDefaultAsync(p => p.Id == request.ProposalId, cancellationToken);

            if (proposal == null)
                throw new Exception("Proposal not found");

            if (proposal.Status != ProposalStatus.Pending)
                throw new Exception("Proposal already handled");

            var project = proposal.Project;
            if (project == null)
                throw new Exception("Project not found");

            //Accept the selected proposal
            proposal.Status = ProposalStatus.Accepted;
            project.Status = ProjectStatus.InProgress;
            project.AssignedFreelancerId = proposal.FreelancerId;

            //Reject all other proposals
            var otherProposals = await context.Proposals
                .Where(p => p.ProjectId == project.Id && p.Id != proposal.Id && p.Status == ProposalStatus.Pending)
                .ToListAsync(cancellationToken);

            foreach (var p in otherProposals)
                p.Status = ProposalStatus.Rejected;

            await context.SaveChangesAsync(cancellationToken);

            //Audit log
            await auditLogger.LogAsync("Accepted proposal", "Proposal", proposal.Id, clientId);
            foreach (var p in otherProposals)
                await auditLogger.LogAsync("Rejected proposal", "Proposal", p.Id, clientId);

            //Publish domain events
            await mediator.Publish(new ProposalAcceptedEvent(proposal.Id, project.Id, proposal.FreelancerId, project.ClientId), cancellationToken);
            foreach (var p in otherProposals)
                await mediator.Publish(new ProposalRejectedEvent(p.Id, project.Id, p.FreelancerId), cancellationToken);

            return Result.Success(ProposalDto.FromEntity(proposal));
        }
    }
}
