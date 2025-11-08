namespace Smart_Freelance_Data.Entities
{
    public class ErrorLog
    {
        public long Id { get; set; }
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? UserEmail { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    }

}
