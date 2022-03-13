using Shelngn.Entities;
using Shelngn.Repositories;
using Dapper;

namespace Shelngn.Data.Repositories
{
    public class RefreshTokenRepository : BaseRepository, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task CreateAsync(RefreshToken refreshToken, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "INSERT INTO refresh_token (value, user_id, expires) " +
                "VALUES (@Value, @UserId, @Expires);";
            await this.UnitOfWork.Connection.ExecuteAsync(sql, refreshToken, this.UnitOfWork.Transaction);
        }

        public async Task DeleteAsync(string value, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql = "DELETE FROM refresh_token WHERE value = @value;";
            await this.UnitOfWork.Connection.ExecuteAsync(sql, new { value }, this.UnitOfWork.Transaction);
        }

        public async Task<RefreshToken?> GetByValueAsync(string value, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql = 
                "SELECT value, user_id, expires, insert_date " +
                "FROM refresh_token WHERE value = @value";
            return await this.UnitOfWork.Connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { value }, this.UnitOfWork.Transaction);
        }
    }
}
