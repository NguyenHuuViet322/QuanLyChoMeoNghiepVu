using ESD.Infrastructure.DapperORM;
using Microsoft.Extensions.Configuration;

namespace ESD.Infrastructure.Repositories.DASKTNN
{
    public class DasDataDapperRepo : DapperRepo, IDasDataDapperRepo
    {
        public DasDataDapperRepo(IConfiguration configuration, IDbConnectionFactory dbConnection)
            : base( configuration, dbConnection.CreateDbConnection(DatabaseConnectionName.DasDataConnection,false) )
        {
            
        }
    }

    public class DynamicDBDapperRepo : DapperRepo, IDynamicDBDapperRepo
    {
        public DynamicDBDapperRepo(IConfiguration configuration, IDbConnectionFactory dbConnection)
            : base(configuration, dbConnection.CreateDbConnection(DatabaseConnectionName.ExcuteGenrateDBConnection,false))
        {

        }
    }
    public class APIManageDBDapperRepo : DapperRepo, IAPIManageDBDapperRepo
    {
        public APIManageDBDapperRepo(IConfiguration configuration, IDbConnectionFactory dbConnection)
            : base(configuration, dbConnection.CreateDbConnection(DatabaseConnectionName.APIManageDBConnection, true))
        {

        }
    }
}
