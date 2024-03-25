using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace ESD.Infrastructure.DapperORM
{
    public class DapperDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDictionary<DatabaseConnectionName, string> _connectionDict;

        public DapperDbConnectionFactory(IDictionary<DatabaseConnectionName, string> connectionDict)
        {
            _connectionDict = connectionDict;
        }

        public IDbConnection CreateDbConnection(DatabaseConnectionName connectionName,bool IsSQLServer)
        {
            // SQL
            if(IsSQLServer)
            {
                string connectionString = null;
                if (_connectionDict.TryGetValue(connectionName, out connectionString))
                {
                    return new SqlConnection(connectionString);
                }
            }    
           else
            {
                string connectionString = null;
                if (_connectionDict.TryGetValue(connectionName, out connectionString))
                {
                    return new OracleConnection(connectionString);
                }
                throw new ArgumentNullException();
            }

            throw new ArgumentNullException();
            // Oracle
            //string connectionString = null;
            //if (_connectionDict.TryGetValue(connectionName, out connectionString))
            //{
            //    return new OracleConnection(connectionString);
            //}

            //throw new ArgumentNullException();
        }
    }
}
