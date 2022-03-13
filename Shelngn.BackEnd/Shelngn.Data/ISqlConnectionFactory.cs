using System.Data;

namespace Shelngn.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Connection { get; }
    }
}