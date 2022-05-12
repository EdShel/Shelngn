namespace Shelngn.Services.GameProjects.Crud
{
    public interface IGameProjectStorageBalancer
    {
        Task<Uri> RequestNewUriAsync(Guid guid);
    }
}
