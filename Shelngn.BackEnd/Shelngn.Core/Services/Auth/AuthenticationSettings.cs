namespace Shelngn.Services.Auth
{
    public class AuthenticationSettings
    {
        public string ValidIssuer { get; set; } = null!;
        public string SigningKey { get; set; } = null!;
        public bool ValidateLifeTime { get; set; }
        public int AccessTokenLifetimeSeconds { get; set; }
        public int RefreshTokenLifetimeSeconds { get; set; }
    }
}
