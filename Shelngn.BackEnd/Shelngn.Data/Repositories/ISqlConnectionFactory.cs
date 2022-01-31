using System.Data;

namespace Shelngn.Data.Repositories
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Connection { get; }
    }
}