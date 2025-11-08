using Smart_Freelance_Infrastructure.Common.Responses;

namespace Smart_Freelance_Service.Abstracts
{
    public interface ITokenValidator
    {
        Task<Result> ValidateAccessTokenAsync(string accessToken, bool allowExpired = false);
        Task<Result> ValidateRefreshTokenAsync(string refreshToken);
    }
}
