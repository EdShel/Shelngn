namespace Shelngn.Services.Auth
{
    public record RefreshResult(
        string AuthToken,
        string RefreshToken
    );

}
