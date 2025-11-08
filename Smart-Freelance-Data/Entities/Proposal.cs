using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class Proposal
    {
        public long Id { get; set; }
        public string CoverLetter { get; set; } = null!;
        public decimal ProposedBudget { get; set; }
        public int EstimatedDays { get; set; }
        public ProposalStatus Status { get; set; } = ProposalStatus.Pending;

        [ForeignKey("Project")]
        public long ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        [ForeignKey("Freelancer")]
        public long FreelancerId { get; set; }
        public ApplicationUser Freelancer { get; set; } = null!;
    }

}
