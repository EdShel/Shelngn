namespace Shelngn.Services.GameProjects.Publishing
{
    public interface IGameProjectPublisher
    {
        Task PublishGameProjectAsync(Guid gameProjectId);
        Task UnpublishGameProjectAsync(Guid gameProjectId);
    }

}
