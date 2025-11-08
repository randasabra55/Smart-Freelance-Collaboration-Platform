using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface IRefreshTokenRepository
    {
        Task<Result<RefreshToken>> SaveRefreshTokenAsync(long userId, string token, DateTimeOffset creationDate, DateTimeOffset expiryDate);
        Task<Result> RemoveRefreshTokenAsync(string refreshToken);
        Task<Result<int>> RemoveExpiredTokensAsync();
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        public Task<Result<TokenData>> GenerateTokensAsync(ApplicationUser user);

    }
}
