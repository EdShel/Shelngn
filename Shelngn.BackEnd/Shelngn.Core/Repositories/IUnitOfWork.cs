using System.Data;

namespace Shelngn.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        void Begin();
        void Commit();
        void Rollback();
    }
}
