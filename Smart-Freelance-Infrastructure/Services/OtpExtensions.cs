using Microsoft.AspNetCore.Identity;
using Smart_Freelance_Data.Entities.Identity;

namespace Smart_Freelance_Infrastructure.Services
{
    public static class OtpExtensions
    {
        public static async Task<string> GenerateOtpTokenAsync(this UserManager<ApplicationUser> manager,
            string purpose, ApplicationUser user)
        {
            var tokenProvider = new EmailOtpTokenProvider();
            return await tokenProvider.GenerateOtpTokenAsync(purpose, manager, user);
        }
    }
}
