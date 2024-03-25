using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.DASNotify;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IHomeServices
    {
        Task<IEnumerable<VMDashBoardPlan>> GetProcessingCollectionPlan(HomeCondition condition);
        Task<VMDashBoardStorage> GetStatisticalStorageByYear(HomeCondition condition);
        Task<VMDashBoardProfile> ProfileAndDocByStatus(HomeCondition condition);
        Task<VMDashBoardExpiryDate> StatisticalExpiryDate(HomeCondition condition);
        Task<List<SelectListItem>> GetDataTypeDashBoard(HomeCondition condition);
        Task<PaginatedList<VMNotification>> GetListNotificationPaging(NotificationCondition condition);
        Task<int> TotalUnreadNotification();
        Task<VMNotification> GetNotificationByUserId(int userId);
        Task<bool> ReadNotification(int id);
    }
}
