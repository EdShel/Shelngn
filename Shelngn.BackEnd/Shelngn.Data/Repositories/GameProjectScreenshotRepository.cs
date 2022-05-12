using Shelngn.Entities;
using Shelngn.Repositories;

namespace Shelngn.Data.Repositories
{
    public class GameProjectScreenshotRepository : DapperBaseRepository, IGameProjectScreenshotRepository
    {
        public GameProjectScreenshotRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task<GameProjectScreenshot> CreateScreenshotAsync(GameProjectScreenshot screenshot, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            screenshot.Id = Guid.NewGuid();

            const string sql =
                "INSERT INTO game_project_screenshot (id, game_project_id, image_url) " +
                "VALUES (@Id, @GameProjectId, @ImageUrl);";
            await ExecuteAsync(sql, screenshot);

            return screenshot;
        }

        public async Task DeleteScreenshotAsync(Guid screenshotId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql = "DELETE FROM game_project_screenshot WHERE id = @screenshotId";
            await ExecuteAsync(sql, new { screenshotId });
        }
    }
}
