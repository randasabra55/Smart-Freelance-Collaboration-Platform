using Microsoft.IdentityModel.Tokens;
using Smart_Freelance_Data.Entities.Identity;
using System.Security.Claims;

namespace Smart_Freelance_Service.Abstracts
{
    public interface ITokenGenerator
    {
        /* public Task<(string Token, DateTimeOffset ExpiryDate)> GenerateAccessToken(ApplicationUser user);
         (string Token, DateTimeOffset CreationDate, DateTimeOffset ExpiryDate) GenerateRefreshToken();
         public Task<IList<Claim>> GetClaims(ApplicationUser user);
         TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = false);
         public Task<Result<TokenData>> RefreshAsync(string accessToken);*/

        Task<(string Token, DateTimeOffset ExpiryDate)> GenerateAccessToken(ApplicationUser user);
        (string Token, DateTimeOffset CreationDate, DateTimeOffset ExpiryDate) GenerateRefreshToken();
        Task<IList<Claim>> GetClaims(ApplicationUser user);
        TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = false);
    }

}
