namespace Shelngn.Api.Filters
{
    public static class GameProjectAuthPolicy
    {
        public const string JustBeingMember = nameof(JustBeingMember);
        public const string ChangeMembers = nameof(ChangeMembers);
        public const string WorkspaceWrite = nameof(WorkspaceWrite);
        public const string Publishing = nameof(Publishing);
    }
}
