using Shelngn.Models;

namespace Shelngn.Services.Auth
{
    public interface IRefreshTokenFactory
    {
        public RefreshToken GenerateRefreshToken(Guid userId);
    }

}
