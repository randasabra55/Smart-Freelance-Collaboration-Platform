using Smart_Freelance_Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class CollaborationRoom
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //[ForeignKey("Project")]
        [ForeignKey(nameof(Project))]
        public long ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // [ForeignKey("Client")]
        [ForeignKey(nameof(Client))]
        public long ClientId { get; set; }
        public ApplicationUser Client { get; set; } = null!;

        [ForeignKey(nameof(FreelancerId))]
        //[ForeignKey("AssignedFreelancer")]
        public long? FreelancerId { get; set; }
        public ApplicationUser? AssignedFreelancer { get; set; }


    }
}
