namespace Shelngn.Services.Auth
{
    public record RefreshResult(
        string AccessToken,
        string RefreshToken
    );

}
