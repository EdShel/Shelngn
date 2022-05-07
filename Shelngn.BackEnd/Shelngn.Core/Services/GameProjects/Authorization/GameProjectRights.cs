namespace Shelngn.Services.GameProjects.Authorization
{
    public class GameProjectRights
    {
        public bool Workspace { get; set; }
        public bool ChangeMembers { get; set; }

        // TODO: access to playing game/adding or removing members etc


        public bool DoesOtherObjectSatisfy(GameProjectRights otherRights)
        {
            return (!this.Workspace || (this.Workspace && otherRights.Workspace))
                && (!this.ChangeMembers || (this.ChangeMembers && otherRights.ChangeMembers));
        }
    }
}
