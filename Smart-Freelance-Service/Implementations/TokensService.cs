using Microsoft.Extensions.Logging;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Service.Implementations
{
    public class TokensService(
     ITokenGenerator tokenGenerator,
     ITokenValidator tokenValidator,
     IRefreshTokenRepository refreshTokenRepository,
     ICookieManager cookieManager,
     ILogger<TokensService> logger) : ITokensService
    {
        public async Task<Result<TokenData>> GenerateTokensAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return ErrorCode.UserNotFound;
            }

            try
            {
                var (accessToken, accessExpiry) = await tokenGenerator.GenerateAccessToken(user);
                var (refreshToken, refreshCreationDate, refreshExpiry) = tokenGenerator.GenerateRefreshToken();

                var savedTokenResult = await refreshTokenRepository.SaveRefreshTokenAsync(
                    user.Id, refreshToken, refreshCreationDate, refreshExpiry);

                if (!savedTokenResult.IsSuccess)
                {
                    return savedTokenResult.Error;
                }

                cookieManager.SetHttpOnlyCookie(nameof(RefreshToken), refreshToken, refreshExpiry);

                var tokenData = new TokenData
                {
                    UserId = user.Id,
                    AccessToken = accessToken,
                    AccessTokenExpiry = accessExpiry,
                    RefreshTokenExpiry = refreshExpiry
                };

                return tokenData;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate tokens for user {UserId}", user.Id);
                return ErrorCode.TokenCreationFailed;
            }
        }

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

        public async Task<Result<int>> RemoveExpiredTokensAsync()
        {
            return await refreshTokenRepository.RemoveExpiredTokensAsync();
        }

        public async Task<Result<TokenData>> RefreshAsync(string accessToken)
        {
            var refreshToken = cookieManager.GetRefreshTokenCookie();

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return ErrorCode.InvalidInput;
            }

            var accessTokenValidationResult = await tokenValidator.ValidateAccessTokenAsync(accessToken, true);
            if (!accessTokenValidationResult.IsSuccess)
            {
                return accessTokenValidationResult.Error;
            }

            var refreshTokenValidationResult = await tokenValidator.ValidateRefreshTokenAsync(refreshToken);
            if (!refreshTokenValidationResult.IsSuccess)
            {
                return refreshTokenValidationResult.Error;
            }

            try
            {
                var rt = await refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

                if (rt == null)
                {
                    return ErrorCode.RefreshTokenInvalid;
                }

                var user = rt.User;
                if (user == null)
                {
                    return ErrorCode.UserNotFound;
                }

                // Revoke the current token
                rt.RevokedOn = DateTimeOffset.UtcNow;
                await refreshTokenRepository.SaveRefreshTokenAsync(
                    rt.UserId, rt.Token, rt.CreationDate, rt.ExpiryDate);

                // Generate new tokens
                return await GenerateTokensAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to refresh tokens");
                return ErrorCode.InternalServerError;
            }
        }
    }
}
