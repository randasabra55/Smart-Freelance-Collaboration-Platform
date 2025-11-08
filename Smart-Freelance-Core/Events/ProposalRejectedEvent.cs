using MediatR;

namespace Smart_Freelance_Core.Events
{
    public class ProposalRejectedEvent : INotification
    {
        public long ProposalId { get; }
        public long ProjectId { get; }
        public long FreelancerId { get; }

        public ProposalRejectedEvent(long proposalId, long projectId, long freelancerId)
        {
            ProposalId = proposalId;
            ProjectId = projectId;
            FreelancerId = freelancerId;
        }
    }
}
