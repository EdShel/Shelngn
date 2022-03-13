using Dapper;
using Shelngn.Repositories;

namespace Shelngn.Data
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
}
