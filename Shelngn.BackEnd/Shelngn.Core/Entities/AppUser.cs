namespace Shelngn.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public DateTimeOffset InsertDate { get; set; }
    }
}
