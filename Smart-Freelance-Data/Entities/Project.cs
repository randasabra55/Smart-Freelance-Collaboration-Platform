using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class Project
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Budget { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CategoryName { get; set; } = null!;
        public ProjectStatus Status { get; set; } = ProjectStatus.Open;

        [ForeignKey("Client")]
        public long ClientId { get; set; }
        public ApplicationUser Client { get; set; } = null!;

        [ForeignKey("AssignedFreelancer")]
        public long? AssignedFreelancerId { get; set; }
        public ApplicationUser? AssignedFreelancer { get; set; }


        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        //public ICollection<CollaborationRoom> CollaborationRooms { get; set; } = new List<CollaborationRoom>();
        public CollaborationRoom? CollaborationRoom { get; set; }
        public ICollection<ProjectSubmission> projectSubmissions { get; set; } = new List<ProjectSubmission>();


        public void Update(string title, string decription, decimal budget, DateTime deadline)
        {
            Title = title;
            Deadline = deadline;
            Description = decription;
            Budget = budget;
        }

    }
}
