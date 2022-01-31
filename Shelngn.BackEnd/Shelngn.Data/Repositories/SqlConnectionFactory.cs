using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace Shelngn.Data.Repositories
{
    public class SqlConnectionFactory : IDisposable, IAsyncDisposable, ISqlConnectionFactory
    {
        private readonly ConnectionStringProvider connectionStringProvider;

        private IDbConnection? connection;

        public SqlConnectionFactory(IOptions<ConnectionStringProvider> connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider.Value;
        }

        public IDbConnection Connection
        {
            get
            {
                if (this.connection == null)
                {
                    this.connection = new SqlConnection(this.connectionStringProvider.SqlServer);
                }
                return this.connection;
            }
        }

        void IDisposable.Dispose()
        {
            if (this.connection != null)
            {
                ((IDisposable)this.connection).Dispose();
            }
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (this.connection != null)
            {
                return ((IAsyncDisposable)this.connection).DisposeAsync();
            }
            return ValueTask.CompletedTask;
        }
    }
}
