namespace Shelngn.Services.GameProjects.Build
{
    public interface IGameProjectBuildResultAccessor
    {
        string? GetMainBundle(string gameProjectId);
    }
}
