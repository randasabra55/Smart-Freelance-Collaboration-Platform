using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface IIdentityService
    {
        Task<string?> GetUserNameAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string policyName);
        Task<bool> IsUserPhoneNumExist(string phoneNum);
        Task<bool> IsUserEmailExist(string email);


        Task<Result<long>> CreateUserAsync(ApplicationUser userData, string password, bool isConfirmed = false);

        Task<Result<UserSessionDto>> SignIn(string Email, string Password);
        Task<Result> SignOut();

        //Task<Result> DeleteUserAsync(string userId);
        Task<Result<UserSessionDto>> RefreshTokenAsync(string accessToken);

        ///////////////////////////////////////////////////////////////



    }
}
