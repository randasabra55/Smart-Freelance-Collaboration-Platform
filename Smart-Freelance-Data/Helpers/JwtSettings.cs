using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Smart_Freelance_Data.Helpers
{
    public class JwtSettings
    {
        public string secret { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public bool validateAudience { get; set; }
        public bool validateIssuer { get; set; }
        public bool validateLifetime { get; set; }
        public bool validateIssuerSigningKey { get; set; }
        public int AccessTokenExpireDate { get; set; }
        public int RefreshTokenExpireDate { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret!));
    }
}
