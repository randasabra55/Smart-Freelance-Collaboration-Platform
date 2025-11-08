using Microsoft.AspNetCore.Identity;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Data.Entities.Identity
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; } = null!;
        public UserType Role { get; set; }  // "Client", "Freelancer", "Admin"
        public bool IsSuspended { get; set; } = false;
        public string? Specialization { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        public void ConfirmEmail() => EmailConfirmed = true;


        public ICollection<Project> ProjectsCreated { get; set; } = new List<Project>();
        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        //public ICollection<CollaborationRoom> CollaborationRooms { get; set; } = new List<CollaborationRoom>();
        public ICollection<CollaborationRoom> CollaborationRoomsAsClient { get; set; } = new List<CollaborationRoom>();
        public ICollection<CollaborationRoom> CollaborationRoomsAsFreelancer { get; set; } = new List<CollaborationRoom>();

        public ICollection<ProjectSubmission> projectSubmissions { get; set; } = new List<ProjectSubmission>();

    }
}
