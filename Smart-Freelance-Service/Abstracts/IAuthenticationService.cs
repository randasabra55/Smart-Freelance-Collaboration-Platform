/*using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface IAuthenticationService
    {
        public Task<string> Register(ApplicationUser user, string Password);
        public Task<Result> ConfirmEmail(string Email, string Code);
        public Task<Result<UserSessionDto>> Login(string Email, string Password);
        //public Task<string> ForgetPassword(string Email);
        public Task<Result> ResetPassword(string email, string otp, string newPassword);
        public Task<Result> ChangePassword(string OldPassword, string NewPassword);
        public Task<Result> ChangeEmail(string OldEmail, string Password, string NewEmail);
        public Task<Result> VerifyOtp(string Email, string Code);
        public Task<Result> RemoveRefreshTokenAsync();
        public Task<Result> SignOut();

        // public Task<bool> DeleteUserAsync(string userId);
        public Task<Result<UserSessionDto>> RefreshTokenAsync(string accessToken);
    }
}
*/