using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface IEmailConfirmationService
    {
        //   Task<Result> SendEmailConfirmation(long userId);
        Task<Result> SendEmailConfirmation(string email);
        public Task<Result> SendEmailConfirmation(long userId);
        Task<Result> ConfirmEmail(string email, string verificationCode);

        Task<Result> SendChangeEmailOtp(string newEmail, ApplicationUser user);
        Task<Result<string>> ConfirmChangeEmail(ApplicationUser user, string newEmail, string verificationCode);
    }
}
