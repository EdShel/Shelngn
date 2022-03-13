using Shelngn.Entities;

namespace Shelngn.Services.Auth
{
    public interface IRefreshTokenFactory
    {
        public RefreshToken GenerateRefreshToken(Guid userId);
    }

}
