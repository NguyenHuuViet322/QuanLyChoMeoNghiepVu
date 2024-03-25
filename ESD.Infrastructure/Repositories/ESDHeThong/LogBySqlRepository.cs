using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.CustomModels;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using ESD.Infrastructure.ContextAccessors;
using Newtonsoft.Json;
using System.Linq;
using ESD.Utility;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using ESD.Infrastructure.Constants;
using Oracle.ManagedDataAccess.Client;
namespace ESD.Infrastructure.Repositories.DAS
{
    public class LogBySqlRepository : ILogBySqlRepository
    {
        #region Properties
        private readonly IUserPrincipalService _userPrincipalService;
        #endregion Properties
        #region Sql
        ArrayList arlSqlParam = new ArrayList();
        private SqlCommand Command { get; set; }
        private SqlConnection GetConnect()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigUtils.GetConnectionString("DASLog");
            con.Open();
            return con;
        }
        public void ClearParam()
        {
            arlSqlParam.Clear();
            arlSqlParam.TrimToSize();
        }
        public void AddParam(string sName, object sValue, int iDirection = 1)
        {
            SqlParameter newParam = new SqlParameter();
            newParam.ParameterName = sName;
            newParam.Value = sValue;
            switch (iDirection)
            {
                case 1:
                    newParam.Direction = ParameterDirection.Input;
                    break;
                case 2:
                    newParam.Direction = ParameterDirection.Output;
                    break;
                case 3:
                    newParam.Direction = ParameterDirection.InputOutput;
                    break;
                case 4:
                    newParam.Direction = ParameterDirection.ReturnValue;
                    break;
                default:
                    break;
            }
            arlSqlParam.Add(newParam);
        }
        public void AddParam(SqlParameter param)
        {
            arlSqlParam.Add(param);
        }
        public async Task<SqlDataReader> GetDataReader(string storeName, bool isStore = true)
        {
            SqlConnection conn = GetConnect();
            Command = new SqlCommand(storeName, conn);
            if (isStore)
            {
                Command.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                Command.CommandType = CommandType.StoredProcedure;
            }
            Command.Parameters.Clear();
            for (int i = 0; i < arlSqlParam.Count; i++)
            {
                SqlParameter tempParam = (SqlParameter)arlSqlParam[i];
                Command.Parameters.Add(tempParam);
            }
            var drReaded = await Command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            return drReaded;
        }
        private int ExecuteQuery(string storeName, bool isStore = true)
        {
            int iResult = 0;
            //using (IDbConnection con = new OracleConnection(ConfigUtils.GetConnectionString("DASLog")))
            //{
            //    con.Open();
            //    OracleCommand objCmd = new OracleCommand(sqlQuery, conn);

            //}
            //using (var conn = GetConnect())
            //{
            //    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
            //    {
            //        try
            //        {
                        
            //            Command = new SqlCommand(storeName, conn);
            //            if (isStore) Command.CommandType = CommandType.StoredProcedure;
            //            else Command.CommandType = CommandType.Text;
            //            Command.Transaction = trans;
            //            Command.Parameters.Clear();
            //            for (int i = 0; i < arlSqlParam.Count; i++)
            //            {
            //                SqlParameter tempParam = (SqlParameter)arlSqlParam[i];
            //                Command.Parameters.Add(tempParam);
            //            }
            //            iResult = Command.ExecuteNonQuery();
            //            trans.Commit();
            //        }
            //        catch (Exception)
            //        {
            //            trans.Rollback();
            //            Command.Dispose();
            //        }
            //    }
            //}
            return iResult;
        }
        private int ExecuteQueryOracle(string Query)
        {
            int iResult = 0;
            using (var conn = new OracleConnection(ConfigUtils.GetConnectionString("DASLog")))
            {
                conn.Open();
                OracleCommand objCmd = new OracleCommand(Query, conn);
                return objCmd.ExecuteNonQuery();
            }
            //using (var conn = GetConnect())
            //{
            //    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
            //    {
            //        try
            //        {

            //            Command = new SqlCommand(storeName, conn);
            //            if (isStore) Command.CommandType = CommandType.StoredProcedure;
            //            else Command.CommandType = CommandType.Text;
            //            Command.Transaction = trans;
            //            Command.Parameters.Clear();
            //            for (int i = 0; i < arlSqlParam.Count; i++)
            //            {
            //                SqlParameter tempParam = (SqlParameter)arlSqlParam[i];
            //                Command.Parameters.Add(tempParam);
            //            }
            //            iResult = Command.ExecuteNonQuery();
            //            trans.Commit();
            //        }
            //        catch (Exception)
            //        {
            //            trans.Rollback();
            //            Command.Dispose();
            //        }
            //    }
            //}
            return iResult;
        }

        #endregion Sql
        #region Ctor
        public LogBySqlRepository(IUserPrincipalService userPrincipalService)
        {
            _userPrincipalService = userPrincipalService;
        }
        #endregion Ctor

        #region InsertLog
        public async Task<int> InsertCRUDLog(LogInfo info)
        {
            int rs = 0;
            //return 1;//DacPV ToDO
            info.UserId = _userPrincipalService.UserId;
            info.Username = _userPrincipalService.UserName;
            var change = Object_Diff(info);
            var oldvalue = info.OldValue != null ? JsonConvert.SerializeObject(info.OldValue) : string.Empty;
            var newvalue = (info.NewValue != null ? JsonConvert.SerializeObject(info.NewValue) : string.Empty);
            var changevalue = change.Count > 0 ? JsonConvert.SerializeObject(info.ChangedValue) : "";
            try
            {
                var query = @$"INSERT INTO LogSystemCRUD
           (ID
           ,TagName
           ,Entity
           ,Action
           ,CreateDate
           ,UserID
           ,UserName
           ,OldValue
           ,NewValue
           ,ChangedValue
		   )
     VALUES
           ('{Guid.NewGuid().ToString()}'
           ,'{ info.TagName}'
           ,'{info.Entity}'
           ,'{info.Action}'
            ,TO_DATE('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')
           ,{info.UserId}
           ,'{info.Username}'
           ,'{oldvalue}'
           ,'{newvalue}'
           ,'{changevalue}'
		   )";
            rs = ExecuteQueryOracle(query);
            }
            catch (Exception ex)
            {

            }
            return rs;
        }
        public async Task<int> InsertUserLog(LogUserInfo info)
        {
            int rs = 0;
            var query =@$"
INSERT INTO LogUserAction
           (ID
           ,Action
           ,CreateDate
           ,UserID
           ,UserName
           ,IPAddress)
     VALUES
           ('{Guid.NewGuid().ToString()}'
           ,'{info.Action}'
           ,TO_DATE('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')
           ,{info.UserId}
           ,'{info.Username}'
           ,'{info.IPAddress}')
";
            try
            {
                rs = ExecuteQueryOracle(query);
            }
            catch (Exception ex)
            {

            }
            return rs;
        }

        
        #endregion InsertLog
        #region GetEntityOld
        private object CreateWithValues(EntityEntry values)
        {
            object entity = Activator.CreateInstance(values.Entity.GetType());
            foreach (var property in values.Entity.GetType().GetProperties())
            {
                //var property = type.GetProperty(propname.);
                property.SetValue(entity, values.Property(property.Name).OriginalValue);
            }

            return entity;
        }
        #endregion
        #region Private
        private List<PdInfo> Object_Diff(LogInfo logInfo, bool ignoreCase = true)
        {
            List<PdInfo> results = new List<PdInfo>();
            if (logInfo.OldValue != null && logInfo.NewValue != null)
            {
                foreach (var prop in logInfo.OldValue.GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()))
                {
                    if (prop.PropertyType.IsGenericType)
                    {
                        if (prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                            continue;
                    }

                    var originValue = prop.GetValue(logInfo.OldValue, null);
                    var otherValue = prop.GetValue(logInfo.NewValue, null);
                    if (originValue == null)
                        originValue = "";
                    if (otherValue == null)
                        otherValue = "";
                    var isModified = !originValue.Equals(otherValue);
                    if (ignoreCase & originValue is string & otherValue is string & otherValue != null)
                    {
                        isModified = originValue.ToString().ToLower() != otherValue.ToString().ToLower();
                    }

                    if (isModified)
                    {
                        results.Add(new PdInfo(prop.Name, originValue.ToString(), otherValue.ToString()));
                    }
                }
            }

            return results;
        }
        #endregion Private
    }
}
