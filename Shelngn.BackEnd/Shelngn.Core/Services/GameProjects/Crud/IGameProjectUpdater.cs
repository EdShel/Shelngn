namespace Shelngn.Services.GameProjects.Crud
{
    public interface IGameProjectUpdater
    {
        Task UpdateNameAsync(Guid gameProjectId, string newProjectName, CancellationToken ct = default);
    }
}
