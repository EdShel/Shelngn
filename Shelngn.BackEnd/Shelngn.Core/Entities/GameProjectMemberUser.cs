namespace Shelngn.Entities
{
    public class GameProjectMemberUser
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public string MemberRole { get; set; } = null!;
    }
}
