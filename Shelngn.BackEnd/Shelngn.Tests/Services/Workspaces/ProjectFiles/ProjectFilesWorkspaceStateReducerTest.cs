using Moq;
using Shelngn.Business.Workspaces.ProjectFiles;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects.Crud;
using Shelngn.Services.GameProjects.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Shelngn.Tests.Services.Workspaces.ProjectFiles
{
    public class ProjectFilesWorkspaceStateReducerTest
    {
        [Fact]
        public async Task GetInitialState_IfProjectFound_ReturnsCorrectProjectRoot()
        {
            var gameProjectSearcherStub = new Mock<IGameProjectSearcher>();
            Guid workspaceId = Guid.Empty;
            var gameProject = new GameProject { Id = workspaceId, FilesLocation = "Some location" };
            gameProjectSearcherStub.Setup(p => p.GetByIdAsync(workspaceId)).ReturnsAsync(gameProject);
            var fileSystemStub = new Mock<IFileSystem>();
            fileSystemStub.Setup(f => f.ListDirectoryFilesAsync(gameProject.FilesLocation, default))
                .ReturnsAsync(new ProjectDirectory(
                    "/", Enumerable.Empty<ProjectDirectory>(), Enumerable.Empty<ProjectFile>()));
            var reducer = new ProjectFilesWorkspaceStateReducer(gameProjectSearcherStub.Object, fileSystemStub.Object);

            var initialState = await reducer.GetInitialStateAsync(workspaceId);

            Assert.Equal(gameProject.FilesLocation, initialState.ProjectFilesRoot);
        }

        [Fact]
        public async Task GetInitialState_IfProjectNotFound_ThrowsException()
        {
            var gameProjectSearcherStub = new Mock<IGameProjectSearcher>();
            Guid workspaceId = Guid.Empty;
            GameProject? gameProject = null;
            gameProjectSearcherStub.Setup(p => p.GetByIdAsync(workspaceId)).ReturnsAsync(gameProject);
            var fileSystemStub = new Mock<IFileSystem>();
            var reducer = new ProjectFilesWorkspaceStateReducer(gameProjectSearcherStub.Object, fileSystemStub.Object);

            var call = async () => await reducer.GetInitialStateAsync(workspaceId);

            await Assert.ThrowsAsync<NotFoundException>(call);
        }

        [Fact]
        public async Task GetInitialState_IfProjectDirectoryNotFound_ThrowsException()
        {
            var gameProjectSearcherStub = new Mock<IGameProjectSearcher>();
            Guid workspaceId = Guid.Empty;
            var gameProject = new GameProject { Id = workspaceId, FilesLocation = "Some location" };
            gameProjectSearcherStub.Setup(p => p.GetByIdAsync(workspaceId)).ReturnsAsync(gameProject);
            var fileSystemStub = new Mock<IFileSystem>();
            var reducer = new ProjectFilesWorkspaceStateReducer(gameProjectSearcherStub.Object, fileSystemStub.Object);

            var call = async () => await reducer.GetInitialStateAsync(workspaceId);

            await Assert.ThrowsAsync<NotFoundException>(call);
        }
    }
}
