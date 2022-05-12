namespace Shelngn.Services.GameProjects.Build
{
    public interface IGameProjectBuilder
    {
        Task<BuildResult> BuildDebugProjectAsync(string gameProjectId);
        Task<BuildResult> BuildProductionProjectAsync(string gameProjectId);
    }
}
