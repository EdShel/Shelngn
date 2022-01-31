namespace Shelngn.Services.Auth
{
    public record RegisterRequest(
        string Email,
        string? UserName,
        string Password
    );

}
