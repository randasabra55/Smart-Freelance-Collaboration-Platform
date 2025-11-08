using Smart_Freelance_Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Freelance_Data.Entities
{
    public class Message
    {
        public long Id { get; set; }

        /*[ForeignKey("Project")]
        public long ProjectId { get; set; }
        public Project Project { get; set; } = null!;*/

        [ForeignKey("Room")]
        public long RoomId { get; set; }
        public CollaborationRoom Room { get; set; } = null!;

        [ForeignKey("Sender")]
        public long SenderId { get; set; }
        public ApplicationUser Sender { get; set; } = null!;

        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

}
