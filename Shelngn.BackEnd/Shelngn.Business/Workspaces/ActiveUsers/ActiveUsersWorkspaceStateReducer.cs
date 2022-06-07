using Shelngn.Entities;
using Shelngn.Services;
using Shelngn.Services.Workspaces.ActiveUsers;

namespace Shelngn.Business.Workspaces.ActiveUsers
{
    public class ActiveUsersWorkspaceStateReducer : IActiveUsersWorkspaceStateReducer
    {
        private readonly IAppUserStore appUserStore;

        public ActiveUsersWorkspaceStateReducer(IAppUserStore appUserStore)
        {
            this.appUserStore = appUserStore;
        }

        public Task<IActiveUsersWorkspaceState> GetInitialState()
        {
            return Task.FromResult<IActiveUsersWorkspaceState>(new ActiveUsersWorkspaceState());
        }

        public async Task AddUserAsync(IActiveUsersWorkspaceState workspaceState, string connectionId, Guid userId)
        {
            AppUser user = await this.appUserStore.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");
            WorkspaceUser workspaceUser = new(connectionId, userId, user.UserName);
            workspaceState.AddUser(workspaceUser);
        }

        public Task RemoveUserAsync(IActiveUsersWorkspaceState workspaceState, string connectionId)
        {
            workspaceState.RemoveUser(connectionId);
            return Task.CompletedTask;
        }
    }
}
