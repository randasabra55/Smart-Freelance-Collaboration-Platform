using MediatR;
using Smart_Freelance_Data.Entities;

namespace Smart_Freelance_Core.Events
{
    public class ProjectCreatedEvent : INotification/*, IAuditableRequest*/
    {
        public Project Project { get; }
        public ProjectCreatedEvent(Project project)
        {
            Project = project;
        }

        ///////////////////////////////////////////////////////////
        ///for register audit log
       /* public string GetAuditAction(object? response, Exception? exception)
        {
            if (exception != null)
                return "RabbitMQ Publish Failed";

            return "ProjectCreatedEvent Sent";
        }

        public long? GetEntityId() => Project.Id;

        public string GetEntityName() => "Project";*/
        /////////////////////////////////////////////////////////////
    }
}
