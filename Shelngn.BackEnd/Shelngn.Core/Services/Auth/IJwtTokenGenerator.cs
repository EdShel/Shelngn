using System.Security.Claims;

namespace Shelngn.Services.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateAuthToken(ClaimsIdentity claims);
        ClaimsPrincipal? ExtractPrincipalFromExpiredAuthHeader(string headerValue);
    }

}
