namespace Shelngn.Services.GameProjects.Build
{
    public interface IGameProjectBuilder
    {
        Task<BuildResult> BuildProjectAsync(string gameProjectId);
    }
}
