namespace Shelngn.Services.Auth
{
    public record RevokeRequest(
        Guid UserId,
        string RefreshToken
    );

}
