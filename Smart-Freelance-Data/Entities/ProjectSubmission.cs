using Smart_Freelance_Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class ProjectSubmission
    {
        public long Id { get; set; }
        [ForeignKey("Project")]
        public long ProjectId { get; set; }
        public Project? Project { get; set; }
        [ForeignKey("User")]
        public long FreelancerId { get; set; }
        public ApplicationUser? User { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Files { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string? ReviewComment { get; set; }
    }
}
