using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Helpers;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Services;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Service.Implementations
{
    public class PasswordService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor) : IPasswordService
    {
        public async Task<Result> CreatePasswordResetOtpAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());

            if (user is null)
                return ErrorCode.NotFound;

            var otp = await userManager.GeneratePasswordResetTokenAsync(user);

            if (!otp.HasValue())
                return ErrorCode.TokenCreationFailed;


            var emailMessage = new EmailMessage
            {
                To = user.Email!,
                Subject = "Password Reset",
                Content = EmailTemplateBuilder.BuildResetPasswordEmail(otp, user.FullName),
                IsHtml = true
            };


            await emailSender.SendAsync(emailMessage);

            return Result.Success();
        }

        public async Task<Result> VerifyOtpAsync(string email, string oTP, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                return ErrorCode.NotFound;

            var isValid = await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", oTP);
            if (!isValid)
                return ErrorCode.VerificationCodeNotValid;

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(string email, string oTP, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
            if (user is null) return ErrorCode.NotFound;

            var resetResult = await userManager.ResetPasswordAsync(user, oTP, newPassword);

            if (!resetResult.Succeeded)
                return ErrorCode.InvalidToken;
            return Result.Success();
        }


        public async Task<Result> ChangePasswordAsync(string OldPassword, string NewPassword, string ConfirmNewPassword, CancellationToken cancellationToken = default)
        {
            var userIdClaim = httpContextAccessor.HttpContext!.User
               .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            long? userId = null;

            if (long.TryParse(userIdClaim, out var parsedId))
            {
                userId = parsedId;
            }
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);


            if (user is null) return ErrorCode.NotAuthorized;

            if (OldPassword == NewPassword)
            {
                return ErrorCode.NewPasswordSameAsOld;
            }
            var changePasswordResult = await userManager.ChangePasswordAsync(user, OldPassword, NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                if (changePasswordResult.Errors.Any(e => e.Code == "PasswordMismatch"))
                    return ErrorCode.OldPasswordIncorrect;

                return ErrorCode.PasswordResetFailed;
            }
            // return ErrorCode.PasswordResetFailed;


            return Result.Success();
        }


    }
}
