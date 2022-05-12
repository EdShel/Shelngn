using Shelngn.Entities;

namespace Shelngn.Repositories
{
    public interface IGameProjectScreenshotRepository
    {
        Task<GameProjectScreenshot> CreateScreenshotAsync(GameProjectScreenshot screenshot, CancellationToken ct = default);
        Task DeleteScreenshotAsync(Guid screenshotId, CancellationToken ct = default);
    }
}
