using ESD.Infrastructure.DapperORM;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ESD.Infrastructure.Repositories.DASKTNN
{
    public abstract class DapperRepo: IDapperRepo
    {
        protected readonly IConfiguration _configuration;
        protected readonly IDbConnection _dbConnection;

        public DapperRepo(IConfiguration configuration, IDbConnection dbConnection)
        {
            _configuration = configuration;
            this._dbConnection = dbConnection;
        }
        
        public IDbConnection idbConnection
        {
            get
            {
                return _dbConnection;
            }
        }


        //public List<T> QueryAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        //{

        //    return dbConnection.QueryAsync<T>(sp, parms, commandType: commandType).Result.ToList();
        //}

        //public T QueryFirstAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        //{
        //    return dbConnection.QueryFirstAsync<T>(sp, parms, commandType: commandType).Result;
        //}

        //public T QueryFirstOrDefaultAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        //{

        //    return dbConnection.QueryFirstOrDefaultAsync<T>(sp, parms, commandType: commandType).Result;
        //}

        //public SqlMapper.GridReader QueryMultipleAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        //{

        //    using var multi = dbConnection.QueryMultipleAsync(sp).Result;
        //    return multi;
        //}

        //public T QuerySingleAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        //{
        //    return dbConnection.QuerySingleAsync<T>(sp, parms, commandType: commandType).Result;
        //}

        //public T QuerySingleOrDefaultAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        //{
        //    return dbConnection.QuerySingleOrDefaultAsync<T>(sp, parms, commandType: commandType).Result;
        //}

        //public T Execute_sp<T>(string query, DynamicParameters sp_params, CommandType commandType = CommandType.StoredProcedure)
        //{
        //    T result;
        //    using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("default")))
        //    {
        //        if (dbConnection.State == ConnectionState.Closed)
        //            dbConnection.Open();
        //        using var transaction = dbConnection.BeginTransaction();
        //        try
        //        {
        //            dbConnection.Query<T>(query, sp_params, commandType: commandType, transaction: transaction);
        //            result = sp_params.Get<T>("retVal"); //get output parameter value
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            throw ex;
        //        }
        //    };
        //    return result;
        //}
    }
}
