using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Events;
using Smart_Freelance_Core.Features.Proposals.Dto;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using System.Security.Claims;

namespace Smart_Freelance_Core.Features.Proposals.Commands.SubmitProposal
{
    public class SubmitProposalCommand : IRequest<Result<ProposalDto>>
    {
        public string CoverLetter { get; set; } = null!;
        public decimal ProposedBudget { get; set; }
        public int EstimatedDays { get; set; }
        public long ProjectId { get; set; }
    }

    public class SubmitProposalCommandHandler(Context context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IMediator mediator)
        : IRequestHandler<SubmitProposalCommand, Result<ProposalDto>>
    {
        public async Task<Result<ProposalDto>> Handle(SubmitProposalCommand request, CancellationToken cancellationToken)
        {
            var userId = long.Parse(httpContextAccessor.HttpContext?.User?
                     .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ErrorCode.NotAuthorized;
            var project = await context.projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId);
            if (project == null)
                return ErrorCode.NotFound;
            var proposal = new Proposal
            {
                ProjectId = request.ProjectId,
                CoverLetter = request.CoverLetter,
                EstimatedDays = request.EstimatedDays,
                ProposedBudget = request.ProposedBudget,
                Status = ProposalStatus.Pending,
                FreelancerId = userId
            };
            await context.Proposals.AddAsync(proposal);
            await context.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new ProposalSubmittedEvent(proposal), cancellationToken);

            return Result.Success(ProposalDto.FromEntity(proposal));
        }
    }
}
