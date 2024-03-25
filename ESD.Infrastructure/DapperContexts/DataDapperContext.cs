using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ESD.Infrastructure.DapperContexts
{
    public class DataDapperContext 
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DataDapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DASKTNN");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
