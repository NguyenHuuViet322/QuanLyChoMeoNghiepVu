using System.Data;

namespace ESD.Infrastructure.DapperORM
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection(DatabaseConnectionName connectionName,bool IsSQLServer);

    }
}
