namespace Shelngn.Services.GameProjects.Build
{
    public interface IGameProjectBuildResultAccessor
    {
        string? GetDebugMainBundle(string gameProjectId);
        string? GetDebugResource(string gameProjectId, string resourcePath);
        string? GetProductionMainBundle(string gameProjectId);
        string? GetProductionResource(string gameProjectId, string resourcePath);
    }
}
