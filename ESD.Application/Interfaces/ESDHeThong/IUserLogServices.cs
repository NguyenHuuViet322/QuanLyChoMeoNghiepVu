using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IUserLogServices
    {
        Task<ServiceResult> LogActionLogin(long userID, string userName);
        Task<ServiceResult> LogActionLogout();
        Task<PaginatedList<VMLogInfo>> GetCRUDLogByCondition(LogInfoCondition condition, bool isExport = false);
        Task<PaginatedList<VMUserLogInfo>> GetUserLogByCondition(LogInfoCondition condition, bool isExport = false);
        Task<VMLogInfo> GetChartCRUDLogByCondition(LogInfoCondition condition);
        Task<VMLogInfoStatistic> GetCRUDLogByConditionErrol(LogInfoCondition condition, bool isExport = false);
    }
}
