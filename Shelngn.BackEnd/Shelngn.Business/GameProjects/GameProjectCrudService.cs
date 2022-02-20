using Shelngn.Models;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shelngn.Business.GameProjects
{
    public record ProjectDirectory(
        string Name,
        IEnumerable<ProjectDirectory> Directories,
        IEnumerable<ProjectFile> Files
    );

    public class ProjectFile
    {
        public ProjectFile(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public Uri? IconUrl { get; }
    }


    public class LocalFileSystem
    {
        public Task CreateDirectoryOrDoNothingIfExistsAsync(Uri uri)
        {
            string path = UriToPath(uri);
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public async Task CreateOrOverwriteFileAsync(Uri uri, byte[] fileContent, CancellationToken ct = default)
        {
            string path = UriToPath(uri);
            await File.WriteAllBytesAsync(path, fileContent, ct);
        }

        public Task<ProjectDirectory?> ListDirectoryFilesAsync(Uri uri, CancellationToken ct = default)
        {
            string path = UriToPath(uri);
            if (!Directory.Exists(path))
            {
                return Task.FromResult<ProjectDirectory?>(null);
            }
            var root = ListDirectoryFiles(path);
            return Task.FromResult<ProjectDirectory?>(root);
        }

        public ProjectDirectory ListDirectoryFiles(string path)
        {
            string[] fileNames = Directory.GetFiles(path);
            ProjectFile[] files = fileNames.Select(f => new ProjectFile(Path.GetFileName(f))).ToArray();
            string[] directoriesNames = Directory.GetDirectories(path);
            ProjectDirectory[] directories = directoriesNames.Select(d => ListDirectoryFiles(d)).ToArray();
            return new ProjectDirectory(
                Name: Path.GetFileName(path)!,
                Directories: directories,
                Files: files
            );
        }

        private static string UriToPath(Uri uri)
        {
            return uri.LocalPath;
        }
    }

    public interface IGameProjectStorageBalancer
    {
        Task<Uri> RequestNewUriAsync(Guid guid);
    }

    public class GameProjectStorageBalancer : IGameProjectStorageBalancer
    {
        private readonly Uri baseUrl;

        public GameProjectStorageBalancer(string baseUrl)
        {
            this.baseUrl = new Uri(baseUrl);
        }

        public Task<Uri> RequestNewUriAsync(Guid guid)
        {
            string relativeUrl = guid.ToUrlSafeBase64();
            return Task.FromResult(new Uri(baseUrl, relativeUrl));
        }
    }


    public class GameProjectSearcher : IGameProjectSearcher
    {
        private readonly IGameProjectRepository gameProjectRepository;

        public GameProjectSearcher(IGameProjectRepository gameProjectRepository)
        {
            this.gameProjectRepository = gameProjectRepository;
        }

        public async Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId)
        {
            return await gameProjectRepository.GetAllForUserAsync(appUserId);
        }

        public async Task<GameProject> GetByIdAsync(Guid gameProjectId)
        {
            return await gameProjectRepository.GetByIdAsync(gameProjectId);
        }
    }

    public class GameProjectCreator : IGameProjectCreator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositoryFactory repositoryFactory;
        private readonly IGameProjectStorageBalancer storageBalancer;
        private readonly LocalFileSystem fileSystem;

        public GameProjectCreator(
            IUnitOfWork unitOfWork,
            IRepositoryFactory repositoryFactory,
            IGameProjectStorageBalancer storageBalancer, 
            LocalFileSystem fileSystem)
        {
            this.unitOfWork = unitOfWork;
            this.repositoryFactory = repositoryFactory;
            this.storageBalancer = storageBalancer;
            this.fileSystem = fileSystem;
        }

        public async Task<GameProject> CreateAsync(GameProject gameProject, Guid appUserId)
        {
            var gameProjectRepository = repositoryFactory.Create<IGameProjectRepository>(unitOfWork);

            unitOfWork.Begin();
            try
            {
                gameProject.Id = Guid.NewGuid();
                gameProject.FilesLocation = (await storageBalancer.RequestNewUriAsync(gameProject.Id)).ToString();
                await gameProjectRepository.CreateAsync(gameProject);

                var owner = new GameProjectMember { AppUserId = appUserId, GameProjectId = gameProject.Id };
                await gameProjectRepository.AddMemberAsync(owner);

                await fileSystem.CreateDirectoryOrDoNothingIfExistsAsync(new Uri(gameProject.FilesLocation));

                var readmeFileUrl = new Uri(Path.Combine(gameProject.FilesLocation, "readme.txt"));
                var readmeFileContent = "TODO: it's just a placeholder";
                await fileSystem.CreateOrOverwriteFileAsync(readmeFileUrl, Encoding.UTF8.GetBytes(readmeFileContent));

                unitOfWork.Commit();

                return gameProject;
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
        }
    }
}
