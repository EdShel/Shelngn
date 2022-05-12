namespace Shelngn.Services.GameProjects.Crud
{
    public interface IGameProjectDeleter
    {
        Task DeleteProjectAsync(Guid gameProjectId);
    }
}
