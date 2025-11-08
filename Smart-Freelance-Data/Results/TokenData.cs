namespace Smart_Freelance_Data.Results
{
    public class TokenData
    {
        public long UserId { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset AccessTokenExpiry { get; set; }
        public DateTimeOffset RefreshTokenExpiry { get; set; }
    }
}
