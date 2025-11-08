using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface IPasswordService
    {

        Task<Result> CreatePasswordResetOtpAsync(string email, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(string Email, string OTP, string NewPassword, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(string OldPassword, string NewPassword, string ConfirmNewPassword, CancellationToken cancellationToken = default);
        public Task<Result> VerifyOtpAsync(string Email, string OTP, CancellationToken cancellationToken = default);

    }
}
