namespace Smart_Freelance_Data.Results
{
    public class UserSessionDto
    {
        public long UserId { get; set; }
        public string? FullName { get; set; }
        public required string Email { get; set; }
        public required string AccessToken { get; set; }
        public DateTimeOffset AccessTokenExpDate { get; set; }
        public DateTimeOffset RefreshTokenExpDate { get; set; }
    }
}
