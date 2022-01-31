namespace Shelngn.Services.Auth
{
    public record LoginResult(
        string Token,
        string RefreshToken
    );

}
