using MediatR;
using Smart_Freelance_Data.Entities;


namespace Smart_Freelance_Core.Events
{
    public class ProposalSubmittedEvent : INotification/*, IAuditableRequest*/
    {
        public Proposal proposal { get; }
        public ProposalSubmittedEvent(Proposal proposal)
        {
            this.proposal = proposal;
        }

        ///////////////////////////////////////////////////////////
        ///for register audit log
       /* public string GetAuditAction(object? response, Exception? exception)
        {
            if (exception != null)
                return "RabbitMQ Publish Failed";

            return "ProposalSubmittedEvent Sent";
        }

        public long? GetEntityId() => proposal.Id;

        public string GetEntityName() => "Proposal";*/
        /////////////////////////////////////////////////////////////
    }
}
