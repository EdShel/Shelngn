using Shelngn.Entities;

namespace Shelngn.Services.Workspaces.ActiveUsers
{
    public class ActiveUsersWorkspaceStateReducer
    {
        private readonly IAppUserStore appUserStore;

        public ActiveUsersWorkspaceStateReducer(IAppUserStore appUserStore)
        {
            this.appUserStore = appUserStore;
        }

        public Task<ActiveUsersWorkspaceState> GetInitialState()
        {
            return Task.FromResult(new ActiveUsersWorkspaceState());
        }

        public async Task AddUserAsync(string connectionId, Guid userId, WorkspaceState workspaceState)
        {
            AppUser user = await this.appUserStore.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");
            WorkspaceUser workspaceUser = new(connectionId, userId, user.UserName);
            workspaceState.ActiveUsers.AddUser(workspaceUser);
        }

        public Task RemoveUserAsync(string connectionId, WorkspaceState workspaceState)
        {
            workspaceState.ActiveUsers.RemoveUser(connectionId);
            return Task.CompletedTask;
        }
    }
}
