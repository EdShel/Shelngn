namespace Shelngn.Services.GameProjects
{
    public interface IGameProjectUpdater
    {
        Task UpdateNameAsync(Guid gameProjectId, string newProjectName, CancellationToken ct = default);
    }
}
