using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Core.Features.Proposals.Dto
{
    public class ProposalDto
    {
        public long Id { get; set; }
        public string CoverLetter { get; set; } = null!;
        public decimal ProposedBudget { get; set; }
        public int EstimatedDays { get; set; }
        public ProposalStatus Status { get; set; }

        public long ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;

        public long FreelancerId { get; set; }
        public string FreelancerName { get; set; } = string.Empty;

        public static ProposalDto FromEntity(Proposal entity)
        {
            return new ProposalDto
            {
                Id = entity.Id,
                Status = entity.Status,
                CoverLetter = entity.CoverLetter,
                EstimatedDays = entity.EstimatedDays,
                FreelancerId = entity.FreelancerId,
                FreelancerName = entity.Freelancer.FullName,
                ProjectId = entity.ProjectId,
                ProjectTitle = entity.Project.Title,
                ProposedBudget = entity.ProposedBudget
            };
        }


    }

}

