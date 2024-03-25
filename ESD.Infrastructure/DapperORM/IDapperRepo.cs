using System.Data;

namespace ESD.Infrastructure.DapperORM
{
    public interface IDapperRepo 
    {
        public IDbConnection idbConnection { get; }
        //List<T> QueryAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //T QueryFirstAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //T QueryFirstOrDefaultAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //T QuerySingleAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //T QuerySingleOrDefaultAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //SqlMapper.GridReader QueryMultipleAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
    }
}
