using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class Notification
    {
        public long Id { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public string Message { get; set; } = null!;
        public NotificationType Type { get; set; } = NotificationType.General;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
