using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface ITokensService
    {
        Task<Result<TokenData>> GenerateTokensAsync(ApplicationUser user);
        Task<Result> RemoveRefreshTokenAsync();
        Task<Result<int>> RemoveExpiredTokensAsync();
        Task<Result<TokenData>> RefreshAsync(string accessToken);
    }
}
