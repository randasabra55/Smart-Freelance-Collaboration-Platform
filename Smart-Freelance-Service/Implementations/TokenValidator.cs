/*using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Smart_Freelance_Service.Implementations
{
    public class TokenValidator(Context context, ILogger<TokenValidator> logger, UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator) : ITokenValidator
    {
        public async Task<Result> ValidateAccessTokenAsync(string accessToken, bool allowExpired = false)
        {
            if (accessToken == null)
                return ErrorCode.InvalidToken;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenGenerator.GetTokenValidationParameters(!allowExpired), out var _);
                var claims = claimsPrincipal.Claims.ToList();

                var hasId = long.TryParse(claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value, out long userIdClaim);
                if (!hasId)
                {
                    return ErrorCode.MalformedToken;
                }

                var userEmailClaim = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));
                if (userEmailClaim == null)
                {
                    return ErrorCode.MalformedToken;
                }

                var userExists = await userManager.Users.AnyAsync(u => u.Id == userIdClaim && u.Email!.Equals(userEmailClaim.Value));
                if (!userExists)
                {
                    return ErrorCode.UserNotFound;
                }

                return Result.Success();
            }
            catch (SecurityTokenExpiredException)
            {
                logger.LogWarning("Access token expired");
                return ErrorCode.TokenExpired;
            }
            catch (SecurityTokenException ex)
            {
                logger.LogWarning(ex, "Invalid access token");
                return ErrorCode.InvalidToken;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to validate access token");
                return ErrorCode.InternalServerError;
            }
        }

        public async Task<Result> ValidateRefreshTokenAsync(string refreshToken)
        {
            if (refreshToken == null)
                return ErrorCode.RefreshTokenInvalid;
            var refreshtoken = await context.RefreshTokens.FirstOrDefaultAsync(rf => rf.Token.Equals(refreshToken));
            if (refreshtoken == null)
            {
                return ErrorCode.RefreshTokenInvalid;
            }

            if (!refreshtoken.IsActive)
            {
                if (refreshtoken.RevokedOn.HasValue)
                {
                    return ErrorCode.TokenRevoked;
                }

                if (DateTimeOffset.UtcNow >= refreshtoken.ExpiryDate)
                {
                    return ErrorCode.RefreshTokenExpired;
                }
            }

            return Result.Success();
        }
    }
}*/


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Smart_Freelance_Service.Implementations
{
    public class TokenValidator(
    ITokenGenerator tokenGenerator,
    UserManager<ApplicationUser> userManager,
    Context context,
    ILogger<TokenValidator> logger) : ITokenValidator
    {
        public async Task<Result> ValidateAccessTokenAsync(string accessToken, bool allowExpired = false)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return ErrorCode.InvalidToken;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenGenerator.GetTokenValidationParameters(!allowExpired), out var _);
                var claims = claimsPrincipal.Claims.ToList();

                var hasId = long.TryParse(claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value, out long userIdClaim);
                if (!hasId)
                {
                    return ErrorCode.MalformedToken;
                }

                var userEmailClaim = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));
                if (userEmailClaim == null)
                {
                    return ErrorCode.MalformedToken;
                }

                var userExists = await userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.Id == userIdClaim && u.Email!.Equals(userEmailClaim.Value));
                if (!userExists)
                {
                    return ErrorCode.UserNotFound;
                }

                return Result.Success();
            }
            catch (SecurityTokenExpiredException)
            {
                logger.LogWarning("Access token expired");
                return ErrorCode.TokenExpired;
            }
            catch (SecurityTokenException ex)
            {
                logger.LogWarning(ex, "Invalid access token");
                return ErrorCode.InvalidToken;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to validate access token");
                return ErrorCode.InternalServerError;
            }
        }

        public async Task<Result> ValidateRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ErrorCode.RefreshTokenInvalid;
            }

            try
            {
                var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token.Equals(refreshToken));

                if (token == null)
                {
                    return ErrorCode.RefreshTokenInvalid;
                }

                if (!token.IsActive)
                {
                    if (token.RevokedOn.HasValue)
                    {
                        return ErrorCode.TokenRevoked;
                    }

                    if (DateTimeOffset.UtcNow >= token.ExpiryDate)
                    {
                        return ErrorCode.RefreshTokenExpired;
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to validate refresh token");
                return ErrorCode.InternalServerError;
            }
        }
    }
}
