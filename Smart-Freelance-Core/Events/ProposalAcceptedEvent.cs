using MediatR;

namespace Smart_Freelance_Core.Events
{
    public class ProposalAcceptedEvent : INotification
    {
        public long ProposalId { get; }
        public long ProjectId { get; }
        public long FreelancerId { get; }
        public long ClientId { get; }

        public ProposalAcceptedEvent(long proposalId, long projectId, long freelancerId, long clientId)
        {
            ProposalId = proposalId;
            ProjectId = projectId;
            FreelancerId = freelancerId;
            ClientId = clientId;
        }
    }
}
