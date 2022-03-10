using Microsoft.AspNetCore.SignalR;

namespace Shelngn.Api.Utilities
{
    public class IdClaimUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.GetIdString();
        }
    }
}
