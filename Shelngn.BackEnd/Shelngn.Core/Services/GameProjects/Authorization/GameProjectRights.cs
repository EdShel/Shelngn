namespace Shelngn.Services.GameProjects.Authorization
{
    public class GameProjectRights
    {
        public bool Workspace { get; set; }
        public bool ChangeMembers { get; set; }
        public bool Publishing { get; set; }

        public static GameProjectRights NoRights()
        {
            return new GameProjectRights();
        }

        public bool DoesOtherObjectSatisfy(GameProjectRights otherRights)
        {
            return (!this.Workspace || (this.Workspace && otherRights.Workspace))
                && (!this.ChangeMembers || (this.ChangeMembers && otherRights.ChangeMembers))
                && (!this.Publishing || (this.Publishing && otherRights.Publishing))
                ;
        }
    }
}
