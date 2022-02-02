namespace Shelngn.Services.Auth
{
    public record LoginResult(
        string AccessToken,
        string RefreshToken
    );

}
