namespace Shelngn.Services.Auth
{
    public record RefreshRequest(
        string AuthHeaderValue,
        string RefreshToken
    );

}
