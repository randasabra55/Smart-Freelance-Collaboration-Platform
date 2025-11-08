namespace Smart_Freelance_Data.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }
        public string Action { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public long? EntityId { get; set; }
        public long? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
