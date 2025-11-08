/*using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ICookieManager = Smart_Freelance_Service.Abstracts.ICookieManager;

namespace Smart_Freelance_Service.Implementations
{
    public class TokenGenerator(
        JwtSettings jwtSettings,
        UserManager<ApplicationUser> userManager,
        ICookieManager cookieManager,
         //ITokenValidator tokenValidator, 
         Lazy<ITokenValidator> _lazyTokenValidator,
         IRefreshTokenRepository refreshTokenRepository,
        ILogger<TokenGenerator> logger
        ) : ITokenGenerator

    {
        private ITokenValidator tokenValidator => _lazyTokenValidator.Value; // ✅ Access when needed
        public async Task<(string Token, DateTimeOffset ExpiryDate)> GenerateAccessToken(ApplicationUser user)
        {
            var signCred = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.secret)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings.issuer,
                audience: jwtSettings.audience,
                expires: DateTime.Now.AddMinutes(jwtSettings.AccessTokenExpireDate),
                claims: await GetClaims(user),
                signingCredentials: signCred
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (accessToken, DateTime.Now.AddMinutes(jwtSettings.AccessTokenExpireDate));
        }

        public (string Token, DateTimeOffset CreationDate, DateTimeOffset ExpiryDate) GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                var now = DateTimeOffset.UtcNow;
                return (
                    Convert.ToBase64String(randomNumber),
                    now,
                    now.AddMinutes(jwtSettings.RefreshTokenExpireDate)
                );
            }
        }

        public async Task<IList<Claim>> GetClaims(ApplicationUser user)
        {
            //add user id, name, email, role

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                //spetial number for token genrated
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;

        }

        public TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = false)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = jwtSettings.validateIssuer,
                ValidIssuer = jwtSettings.issuer,

                ValidateAudience = jwtSettings.validateAudience,
                ValidAudience = jwtSettings.audience,

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true

            };
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
                return await refreshTokenRepository.GenerateTokensAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to refresh tokens");
                return ErrorCode.InternalServerError;
            }
        }
    }
}*/




using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Smart_Freelance_Service.Implementations
{
    public class TokenGenerator(JwtSettings jwtSettings, UserManager<ApplicationUser> userManager) : ITokenGenerator
    {
        public async Task<(string Token, DateTimeOffset ExpiryDate)> GenerateAccessToken(ApplicationUser user)
        {
            var expires = DateTimeOffset.Now.AddMinutes(jwtSettings.AccessTokenExpireDate);
            var claims = GetClaims(user);
            var jwt = new JwtSecurityToken(
                issuer: jwtSettings.issuer,
                audience: jwtSettings.audience,
                claims: await claims,
                expires: expires.UtcDateTime,
                signingCredentials: new SigningCredentials(jwtSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return (encodedJwt, expires);
        }

        public (string Token, DateTimeOffset CreationDate, DateTimeOffset ExpiryDate) GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                var now = DateTimeOffset.UtcNow;
                return (
                    Convert.ToBase64String(randomNumber),
                    now,
                    now.AddMinutes(jwtSettings.RefreshTokenExpireDate)
                );
            }
        }

        public async Task<IList<Claim>> GetClaims(ApplicationUser user)
        {
            List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        ];
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        public TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = false) =>
            new TokenValidationParameters()
            {
                ValidateIssuer = jwtSettings.validateIssuer,
                ValidIssuer = jwtSettings.issuer,

                ValidateAudience = jwtSettings.validateAudience,
                ValidAudience = jwtSettings.audience,

                //ClockSkew = TimeSpan.FromMinutes(0),
                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = validateLifetime,
                IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true
            };
    }
}
