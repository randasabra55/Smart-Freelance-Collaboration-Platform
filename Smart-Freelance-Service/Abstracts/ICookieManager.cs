namespace Smart_Freelance_Service.Abstracts
{
    public interface ICookieManager
    {
        void SetHttpOnlyCookie(string key, string value, DateTimeOffset expiry);
        string? GetRefreshTokenCookie();
        void ClearRefreshTokenCookie();
    }
}
