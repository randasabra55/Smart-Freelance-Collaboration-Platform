using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class Transaction
    {
        public long Id { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Project")]
        public long ProjectId { get; set; }
        public Project? Project { get; set; }

        public TransactionType Type { get; set; } // Deposit, Payout, Refund, Commission

        public string Currency { get; set; } = "EGP";

        public decimal Amount { get; set; }

        public decimal? CommissionAmount { get; set; }

        public TransactionStatus Status { get; set; } // Pending, Success, Failed

        // رقم العملية في Paymob
        public string? PaymentGatewayTransactionId { get; set; }

        public string PaymentKey { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; }

    }

}
