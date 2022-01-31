using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shelngn.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shelngn.Business.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private const string SECURITY_ALGORITHM = SecurityAlgorithms.HmacSha256;

        private readonly AuthenticationSettings jwtSettings;
        private readonly TokenValidationParameters expiredTokenValidationParameters;

        public JwtTokenGenerator(IOptions<AuthenticationSettings> jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
            this.jwtSettings = jwtSettings.Value;
            this.expiredTokenValidationParameters = tokenValidationParameters.Clone();
            this.expiredTokenValidationParameters.ValidateLifetime = false;
        }

        public string GenerateAuthToken(ClaimsIdentity claims)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: this.jwtSettings.ValidIssuer,
                    notBefore: now,
                    claims: claims.Claims,
                    expires: now.Add(TimeSpan.FromSeconds(this.jwtSettings.AccessTokenLifetimeSeconds)),
                    signingCredentials: new SigningCredentials(
                        key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.SigningKey)),
                        algorithm: SECURITY_ALGORITHM
                    )
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public ClaimsPrincipal? ExtractPrincipalFromExpiredAuthHeader(string headerValue)
        {
            var tokenHandler = new JwtSecurityTokenHandler
            {
                // Set it to false, becuase it will mangle the claims types
                // kinda email to http://some.really/strange/url/email.
                MapInboundClaims = false
            };
            string token = headerValue.Substring("Bearer ".Length);
            var principal = tokenHandler.ValidateToken(token, this.expiredTokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SECURITY_ALGORITHM, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
    }
}
