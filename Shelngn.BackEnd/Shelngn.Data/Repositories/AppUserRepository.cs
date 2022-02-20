using Dapper;
using Shelngn.Models;
using Shelngn.Repositories;

namespace Shelngn.Data.Repositories
{
    public abstract class DapperBaseRepository : BaseRepository
    {
        protected DapperBaseRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task ExecuteAsync(string sql, object? param = null)
        {
            await this.UnitOfWork.Connection.ExecuteAsync(sql, param, this.UnitOfWork.Transaction);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            return await this.UnitOfWork.Connection.QueryFirstOrDefaultAsync<T>(sql, param, this.UnitOfWork.Transaction);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            return await this.UnitOfWork.Connection.QueryAsync<T>(sql, param, this.UnitOfWork.Transaction);
        }
    }

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

        public async Task<GameProject> GetByIdAsync(Guid gameProjectId, CancellationToken ct = default)
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

    public class AppUserRepository : BaseRepository, IAppUserRepository
    {
        public AppUserRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task CreateAsync(AppUser user, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "INSERT INTO app_user(id, email, user_name, password_hash, avatar_url) " +
                "VALUES (@Id, @Email, @UserName, @PasswordHash, @AvatarUrl);";
            await this.UnitOfWork.Connection.ExecuteAsync(sql, user);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql = "DELETE FROM app_user WHERE id = @id;";
            await this.UnitOfWork.Connection.ExecuteAsync(sql, new { id });
        }

        public async Task<AppUser?> GetFirstByIdOrNullAsync(Guid id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT id, email, user_name, password_hash, avatar_url " +
                "FROM app_user " +
                "WHERE id = @id;";
            return await this.UnitOfWork.Connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { id });
        }

        public async Task<AppUser?> GetFirstByUserNameOrNullAsync(string userName, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT id, email, user_name, password_hash, avatar_url " +
                "FROM app_user " +
                "WHERE user_name = @userName;";
            return await this.UnitOfWork.Connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { userName });
        }
        public async Task<AppUser?> GetFirstByEmailOrNullAsync(string email, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT id, email, user_name, password_hash, avatar_url " +
                "FROM app_user " +
                "WHERE email = @email;";
            return await this.UnitOfWork.Connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { email });
        }

        public async Task UpdateAllAsync(AppUser user, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "UPDATE app_user SET " +
                "user_name = @UserName, " +
                "password_hash = @PasswordHash, " +
                "avatar_url = @AvatarUrl " +
                "WHERE id = @Id;";
            await this.UnitOfWork.Connection.ExecuteAsync(sql, user);
        }
    }
}
