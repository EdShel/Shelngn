using Shelngn.Models;
using Shelngn.Repositories;
using Dapper;

namespace Shelngn.Data.Repositories
{
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
