using ESD.Domain.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;


namespace ESD.Domain.Interfaces.DAS
{
    public interface ILogBySqlRepository
    {
        Task<int> InsertCRUDLog(LogInfo info);
        Task<int> InsertUserLog(LogUserInfo info);
        void ClearParam();
        void AddParam(string sName, object sValue, int iDirection = 1);
        void AddParam(SqlParameter param);
        Task<SqlDataReader> GetDataReader(string storeName, bool isStore = true);

    }
}
