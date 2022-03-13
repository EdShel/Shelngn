using Shelngn.Repositories;
using System.Data;

namespace Shelngn.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlConnectionFactory sqlConnectionFactory;

        private IDbTransaction? transaction;

        public UnitOfWork(ISqlConnectionFactory sqlConnectionFactory)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
        }

        public IDbConnection Connection => this.sqlConnectionFactory.Connection;
        public IDbTransaction? Transaction => transaction;

        public void Begin()
        {
#if DEBUG
            if (this.transaction != null)
            {
                throw new InvalidOperationException("Already started the transaction.");
            }
#endif
            if (this.sqlConnectionFactory.Connection.State == ConnectionState.Closed)
            {
                this.sqlConnectionFactory.Connection.Open();
            }
            this.transaction = this.sqlConnectionFactory.Connection.BeginTransaction();
        }

        public void Commit()
        {
#if DEBUG
            if (this.transaction == null)
            {
                throw new InvalidOperationException("The transaction hasn't began.");
            }
#endif
            this.transaction.Commit();
            this.transaction = null;
        }

        public void Rollback()
        {
#if DEBUG
            if (this.transaction == null)
            {
                throw new InvalidOperationException("The transaction hasn't began.");
            }
#endif
            this.transaction.Rollback();
            this.transaction = null;
        }

        public void Dispose()
        {
            this.transaction?.Dispose();
        }
    }
}
