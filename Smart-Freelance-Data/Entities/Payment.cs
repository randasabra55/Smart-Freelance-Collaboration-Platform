using Smart_Freelance_Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class Payment
    {
        public long Id { get; set; }

        [ForeignKey("Project")]
        public long ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
