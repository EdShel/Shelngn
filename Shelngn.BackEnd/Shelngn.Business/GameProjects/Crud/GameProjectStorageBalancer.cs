using Shelngn.Services.GameProjects.Crud;

namespace Shelngn.Business.GameProjects.Crud
{
    public class GameProjectStorageBalancer : IGameProjectStorageBalancer
    {
        private readonly Uri baseUrl;

        public GameProjectStorageBalancer(string baseUrl)
        {
            this.baseUrl = new Uri(baseUrl);
        }

        public Task<Uri> RequestNewUriAsync(Guid guid)
        {
            string relativeUrl = guid.ToUrlSafeBase64();
            return Task.FromResult(new Uri(baseUrl, relativeUrl));
        }
    }
}
