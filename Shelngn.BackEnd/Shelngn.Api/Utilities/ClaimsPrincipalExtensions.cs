using System.Security.Claims;

namespace Shelngn.Api
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetIdString(this ClaimsPrincipal principal)
        {
            return principal?.FindFirst(AuthConstants.Claims.ID)?.Value
                ?? throw new InvalidOperationException("Cannot find id claim");
        }

        public static Guid GetIdGuid(this ClaimsPrincipal principal)
        {
            return Guid.Parse(principal.GetIdString());
        }
    }
}
