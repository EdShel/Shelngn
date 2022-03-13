using Shelngn.Entities;
using Shelngn.Repositories;

namespace Shelngn.Data.Repositories
{
    public class GameProjectRepository : DapperBaseRepository, IGameProjectRepository
    {
        public GameProjectRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task CreateAsync(GameProject gameProject, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "INSERT INTO game_project(id, project_name, files_location) " +
                "VALUES(@Id, @ProjectName, @FilesLocation)";
            await ExecuteAsync(sql, gameProject);
        }

        public async Task UpdateNameAsync(Guid gameProjectId, string newProjectName, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "UPDATE game_project SET project_name = @newProjectName WHERE id = @gameProjectId";
            await ExecuteAsync(sql, new { gameProjectId, newProjectName });
        }

        public async Task DeleteAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "DELETE FROM game_project WHERE id = @gameProjectId;";
            await ExecuteAsync(sql, new { gameProjectId });
        }

        public async Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT g.id, g.project_name, g.files_location, g.insert_date " +
                "FROM game_project g " +
                "JOIN game_project_member gp ON g.id = gp.game_project_id " +
                "WHERE gp.app_user_id = @appUserId " +
                "ORDER BY gp.insert_date;";
            return await QueryAsync<GameProject>(sql, new { appUserId });
        }

        public async Task<GameProject?> GetByIdAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT g.id, g.project_name, g.files_location, g.insert_date " +
                "FROM game_project g " +
                "WHERE g.id = @gameProjectId;";
            return await QueryFirstOrDefaultAsync<GameProject>(sql, new { gameProjectId });
        }
        public async Task AddMemberAsync(GameProjectMember gameProjectMember, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "INSERT INTO game_project_member (game_project_id, app_user_id) " +
                "VALUES (@GameProjectId, @AppUserId);";
            await ExecuteAsync(sql, gameProjectMember);
        }
    }
}
