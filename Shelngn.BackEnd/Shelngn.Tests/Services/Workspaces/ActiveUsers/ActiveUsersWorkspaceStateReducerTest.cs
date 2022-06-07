using Moq;
using Shelngn.Business.Workspaces.ActiveUsers;
using Shelngn.Entities;
using Shelngn.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Shelngn.Tests.Services.Workspaces.ActiveUsers
{
    public class ActiveUsersWorkspaceStateReducerTest
    {
        [Fact]
        public async Task GetInitialState_Always_ReturnsEmptyList()
        {
            var appUserStoreStub = new Mock<IAppUserStore>();
            var reducer = new ActiveUsersWorkspaceStateReducer(appUserStoreStub.Object);

            var initialState = await reducer.GetInitialState();

            Assert.Empty(initialState.GetUsers());
        }

        [Fact]
        public async Task AddUserAsync_WhenUserFound_AddsUserToCollection()
        {
            Guid userId = Guid.Empty;
            var appUserStoreStub = new Mock<IAppUserStore>();
            var user = new AppUser() { Id = userId };
            appUserStoreStub.Setup(u => u.FindByIdAsync(userId, default)).ReturnsAsync(user);
            var reducer = new ActiveUsersWorkspaceStateReducer(appUserStoreStub.Object);

            var state = await reducer.GetInitialState();
            await reducer.AddUserAsync(state, "", userId);

            Assert.Collection(state.GetUsers(), u => u.UserId.Equals(userId));
        }

        [Fact]
        public async Task AddUserAsync_WhenUserNotFound_ThrowsException()
        {
            Guid userId = Guid.Empty;
            var appUserStoreStub = new Mock<IAppUserStore>();
            appUserStoreStub.Setup(u => u.FindByIdAsync(userId, default)).ReturnsAsync((AppUser?)null);
            var reducer = new ActiveUsersWorkspaceStateReducer(appUserStoreStub.Object);

            var state = await reducer.GetInitialState();
            var call = async () => await reducer.AddUserAsync(state, "", userId);

            await Assert.ThrowsAsync<InvalidOperationException>(call);
        }
    }
}
