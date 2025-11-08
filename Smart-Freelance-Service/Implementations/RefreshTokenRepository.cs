using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Service.Abstracts;
using ICookieManager = Smart_Freelance_Service.Abstracts.ICookieManager;

namespace Smart_Freelance_Service.Implementations
{
    public class RefreshTokenRepository(Context context, ITokenGenerator tokenGenerator, ICookieManager cookieManager, ILogger<RefreshTokenRepository> logger) : IRefreshTokenRepository
    {
        public async Task<Result<RefreshToken>> SaveRefreshTokenAsync(long userId, string token, DateTimeOffset creationDate, DateTimeOffset expiryDate)
        {
            try
            {
                var refreshToken = new RefreshToken
                {
                    UserId = userId,
                    Token = token,
                    CreationDate = creationDate,
                    ExpiryDate = expiryDate,
                };

                await context.RefreshTokens.AddAsync(refreshToken);
                await context.SaveChangesAsync();

                return refreshToken;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database error while saving refresh token for user {UserId}", userId);
                return ErrorCode.DatabaseQueryFailed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save refresh token for user {UserId}", userId);
                return ErrorCode.InternalServerError;
            }
        }

        public async Task<Result> RemoveRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ErrorCode.InvalidInput;
            }

            try
            {
                var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token.Equals(refreshToken));
                if (token == null)
                {
                    return Result.Failure(new Error(ErrorCode.RefreshTokenInvalid));
                }

                context.RefreshTokens.Remove(token);
                await context.SaveChangesAsync();

                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database error while removing refresh token");
                return ErrorCode.DatabaseQueryFailed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to remove refresh token");
                return ErrorCode.InternalServerError;
            }
        }

        public async Task<Result<int>> RemoveExpiredTokensAsync()
        {
            try
            {
                var rowsAffected = await context.RefreshTokens
                    .Where(t => DateTimeOffset.UtcNow >= t.ExpiryDate || t.RevokedOn.HasValue)
                    .ExecuteDeleteAsync();

                if (rowsAffected > 0)
                {
                    logger.LogInformation("{RowsAffected} expired or revoked tokens removed", rowsAffected);
                }

                return rowsAffected;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database error while removing expired tokens");
                return ErrorCode.DatabaseQueryFailed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to remove expired tokens");
                return ErrorCode.InternalServerError;
            }
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            try
            {
                return await context.RefreshTokens
                    .Include(rt => rt.User)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.Token.Equals(token));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get refresh token");
                return null;
            }
        }

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

                var savedTokenResult = await SaveRefreshTokenAsync(
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
    }
}
