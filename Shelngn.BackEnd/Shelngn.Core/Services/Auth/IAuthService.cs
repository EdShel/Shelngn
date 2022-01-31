namespace Shelngn.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest loginRequest, CancellationToken ct = default);
        Task<RefreshResult> RefreshTokenAsync(RefreshRequest refreshRequest, CancellationToken ct = default);
        Task RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task RevokeTokenAsync(RevokeRequest revokeRequest, CancellationToken ct = default);
    }
}
