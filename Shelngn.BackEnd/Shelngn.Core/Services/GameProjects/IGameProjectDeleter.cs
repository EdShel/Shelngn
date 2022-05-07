namespace Shelngn.Services.GameProjects
{
    public interface IGameProjectDeleter
    {
        Task DeleteProjectAsync(Guid gameProjectId);
    }
}
