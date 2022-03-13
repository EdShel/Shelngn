namespace Shelngn.Services.GameProjects
{
    public interface IGameProjectStorageBalancer
    {
        Task<Uri> RequestNewUriAsync(Guid guid);
    }
}
