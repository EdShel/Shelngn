using Microsoft.Extensions.Options;
using Shelngn.Models;
using Shelngn.Services.Auth;

namespace Shelngn.Business.Auth
{
    public class RefreshTokenFactory : IRefreshTokenFactory
    {
        private readonly int lifeTimeSeconds;

        public RefreshTokenFactory(IOptions<AuthenticationSettings> settings)
        {
            this.lifeTimeSeconds = settings.Value.RefreshTokenLifetimeSeconds;
        }

        public RefreshToken GenerateRefreshToken(Guid userId)
        {
            return new RefreshToken
            {

                UserId = userId,
                Value = GenerateRefreshTokenValue(),
                Expires = DateTime.UtcNow.AddSeconds(this.lifeTimeSeconds)
            };
        }

        private static string GenerateRefreshTokenValue()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
