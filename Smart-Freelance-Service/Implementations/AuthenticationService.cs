/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        UserManager<ApplicationUser> userManager;
        ICookieManager cookieManager;
        IIdentityService identityService;
        IHttpContextAccessor httpContextAccessor;
        ITokenGenerator tokenGenerator;
        IRefreshTokenRepository refreshTokenRepository;
        IEmailConfirmationService emailConfirmationService;
        ILogger<AuthenticationService> logger;
        public AuthenticationService(UserManager<ApplicationUser> userManager, ICookieManager cookieManager, ILogger<AuthenticationService> logger, IRefreshTokenRepository refreshTokenRepository, IIdentityService identityService, IEmailConfirmationService emailConfirmationService, IHttpContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator)
        {
            this.userManager = userManager;
            this.cookieManager = cookieManager;
            this.identityService = identityService;
            this.logger = logger;
            this.tokenGenerator = tokenGenerator;
            this.httpContextAccessor = httpContextAccessor;
            this.refreshTokenRepository = refreshTokenRepository;
            this.emailConfirmationService = emailConfirmationService;
        }
        //done
        public async Task<Result> ChangeEmail(string OldEmail, string Password, string NewEmail)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == OldEmail.ToLower());
            if (user == null)
                return ErrorCode.InvalidEmail;
            //check pass
            var validPass = await userManager.CheckPasswordAsync(user, Password);
            if (!validPass)
                return ErrorCode.IncorrectPassword;

            if (NewEmail.ToLower() == OldEmail.ToLower())
                return ErrorCode.SameEmail;
            if (await identityService.IsUserEmailExist(NewEmail!))
                return ErrorCode.EmailAlreadyExists;
            //send otp to new email
            await emailConfirmationService.SendChangeEmailOtp(NewEmail, user);
            // user.Email = NewEmail;
            // EmailT
            return Result.Success();
        }
        //done
        public async Task<Result> ChangePassword(string OldPassword, string NewPassword)
        {
            //get user
            var userIdClaim = httpContextAccessor.HttpContext!.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            long? userId = null;

            if (long.TryParse(userIdClaim, out var parsedId))
            {
                userId = parsedId;
            }

            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ErrorCode.UserNotFound;

            if (OldPassword == NewPassword)
                return ErrorCode.NewPasswordSameAsOld;
            var changePasswordResult = await userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                if (changePasswordResult.Errors.Any(e => e.Code == "PasswordMismatch"))
                    return ErrorCode.OldPasswordIncorrect;

                return ErrorCode.PasswordResetFailed;
            }

            return Result.Success();
        }
        //done
        public async Task<Result> ConfirmEmail(string Email, string Code)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == Email.ToLower());

            if (user is null) return ErrorCode.UnregisteredEmail;

            var confirmationResults = await userManager.ConfirmEmailAsync(user, Code);

            if (!confirmationResults.Succeeded)
            {
                var firstError = confirmationResults.Errors.Select(e => e.Description).FirstOrDefault() ?? "An error occurred during email confirmation.";
                return new Error(ErrorCode.ExternalProviderError, firstError);
            }
            return Result.Success();
        }
        //done
        public async Task<Result<UserSessionDto>> Login(string Email, string Password)
        {
            //check mail
            var user = await userManager.FindByEmailAsync(Email.ToLower());
            if (user == null)
                return ErrorCode.InvalidCredentials;
            if (!user.EmailConfirmed)
                return ErrorCode.EmailNotConfirmed;
            //check pass
            var validPassword = await userManager.CheckPasswordAsync(user, Password);
            if (!validPassword)
                return ErrorCode.InvalidCredentials;
            //create access token
            var (accessToken, Expiry) = await tokenGenerator.GenerateAccessToken(user);
            //create refresh token
            var (refreshToken, creation, expiry) = tokenGenerator.GenerateRefreshToken();
            //save refresh token in database
            var res = await refreshTokenRepository.SaveRefreshTokenAsync(user.Id, refreshToken, creation, expiry);
            if (!res.IsSuccess)
                return ErrorCode.DatabaseQueryFailed;
            //save refreshToken in httponly cookie
            cookieManager.SetHttpOnlyCookie(nameof(RefreshToken), refreshToken, expiry);
            //return seccuss
            var response = new UserSessionDto
            {
                AccessToken = accessToken,
                AccessTokenExpDate = Expiry,
                RefreshTokenExpDate = expiry,
                Email = user.Email!,
                FullName = user.FullName,
                UserId = user.Id
            };
            return Result.Success(response);
        }
        //done
        public async Task<Result<UserSessionDto>> RefreshTokenAsync(string accessToken)
        {
            TokenData tokenGenRes;
            var res = await tokenGenerator.RefreshAsync(accessToken);
            if (!res.IsSuccess || res.Data is null)
            {
                return res.Error;
            }

            tokenGenRes = res.Data;

            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == long.Parse(tokenGenRes.UserId.ToString()));

            if (user == null)
                return ErrorCode.UserNotFound;

            return new UserSessionDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                AccessToken = tokenGenRes.AccessToken,
                AccessTokenExpDate = tokenGenRes.AccessTokenExpiry,
                RefreshTokenExpDate = tokenGenRes.RefreshTokenExpiry
            };
        }
        //done
        public async Task<string> Register(ApplicationUser user, string password)
        {
            //check on email
            var excitingMail = await userManager.FindByEmailAsync(user.Email!);
            if (excitingMail == null)
                return "EmailIsExist";
            //check user name
            var existUserName = await userManager.FindByNameAsync(user.UserName);
            if (existUserName != null)
            {
                return "UserNameIsExist";
            }

            IdentityResult result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                //assign him to role
                if (user.Role == UserType.Client)
                    await userManager.AddToRoleAsync(user, UserType.Client.ToString());
                else if (user.Role == UserType.Freelancer)
                    await userManager.AddToRoleAsync(user, UserType.Freelancer.ToString());
                return "Done";
            }

            return "Error";
        }
        //done
        public async Task<Result> RemoveRefreshTokenAsync()
        {
            var refreshToken = cookieManager.GetRefreshTokenCookie();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ErrorCode.InvalidInput;
            }

            try
            {
                var result = await refreshTokenRepository.RemoveRefreshTokenAsync(refreshToken);

                if (result.IsSuccess)
                {
                    cookieManager.ClearRefreshTokenCookie();
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to remove refresh token");
                return ErrorCode.InternalServerError;
            }
        }
        //done
        public async Task<Result> ResetPassword(string email, string OTP, string newPassword)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
            if (user is null) return ErrorCode.NotFound;
            var resetResult = await userManager.ResetPasswordAsync(user, OTP, newPassword);
            if (!resetResult.Succeeded)
                return ErrorCode.InvalidToken;
            return Result.Success();
        }
        //done
        public async Task<Result> SignOut()
        {
            //delete refresh token
            var refreshToken = cookieManager.GetRefreshTokenCookie();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ErrorCode.InvalidInput;
            }
            try
            {
                var result = await refreshTokenRepository.RemoveRefreshTokenAsync(refreshToken);

                if (result.IsSuccess)
                {
                    cookieManager.ClearRefreshTokenCookie();
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to remove refresh token");
                return ErrorCode.InternalServerError;
            }

        }
        //done
        public async Task<Result> VerifyOtp(string Email, string Code)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
                return ErrorCode.NotFound;
            var isValidOTP = await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", Code);
            if (!isValidOTP)
                return ErrorCode.VerificationCodeNotValid;

            return Result.Success();
        }


    }
}
*/