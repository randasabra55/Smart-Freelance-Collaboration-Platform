using Microsoft.AspNetCore.Http;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Service.Implementations
{
    class CookieManager(IHttpContextAccessor httpContextAccessor) : ICookieManager
    {
        public void ClearRefreshTokenCookie()
        {
            httpContextAccessor.HttpContext!.Response.Cookies.Delete(nameof(RefreshToken));
        }

        public string? GetRefreshTokenCookie()
        {
            return httpContextAccessor.HttpContext!.Request.Cookies[nameof(RefreshToken)];
        }

        public void SetHttpOnlyCookie(string key, string value, DateTimeOffset expiry)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = expiry,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false
            };

            httpContextAccessor.HttpContext!.Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}
