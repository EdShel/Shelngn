using Shelngn.Entities;
using Shelngn.Repositories;

namespace Shelngn.Data.Repositories
{
    public class GameProjectPublicationRepository : DapperBaseRepository, IGameProjectPublicationRepository
    {
        public GameProjectPublicationRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task CreatePublicationAsync(GameProjectPublication publication, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                @"INSERT INTO game_project_publication(game_project_id) VALUES (@GameProjectId);";
            await ExecuteAsync(sql, publication);
        }

        public async Task DeletePublicationAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                @"DELETE FROM game_project_publication WHERE game_project_id = @gameProjectId;";
            await ExecuteAsync(sql, new { gameProjectId });
        }

        public async Task<bool> ExistsAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                @"SELECT 1 FROM game_project_publication WHERE game_project_id = @gameProjectId;";

            return await ExecuteScalarAsync<int>(sql, new { gameProjectId }) == 1;
        }
    }
}
