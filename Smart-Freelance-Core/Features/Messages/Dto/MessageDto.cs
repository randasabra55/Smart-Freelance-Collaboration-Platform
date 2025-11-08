using Smart_Freelance_Data.Entities;

namespace Smart_Freelance_Core.Features.Messages.Dto
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long RoomId { get; set; }
        public long SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public static MessageDto FromEntity(Message entity)
        {
            return new MessageDto
            {
                Content = entity.Content,
                Id = entity.Id,
                RoomId = entity.RoomId,
                SenderId = entity.SenderId,
                SenderName = entity.Sender.FullName,
                SentAt = entity.SentAt
            };
        }
    }
}
