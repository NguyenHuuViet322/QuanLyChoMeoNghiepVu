using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ESD.Domain.Enums;
using ESD.Application.Enums;
using ESD.Utility;
using ESD.Infrastructure.ContextAccessors;
using ESD.Domain.Interfaces.DASNotify;
using AutoMapper;
using ESD.Utility.CacheUtils;
using ESD.Application.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESD.Application.Services
{
    public class HomeService : IHomeServices
    {
        protected IDasRepositoryWrapper _dasRepo;
        protected IDasNotifyRepositoryWrapper _dasNotifyRepo;
        protected IUserPrincipalService _userPrincipalService;
        protected IMapper _mapper;
        private readonly ICacheManagementServices _cacheManagementServices;

        public HomeService(IDasRepositoryWrapper dasRepo
            , IDasNotifyRepositoryWrapper dasNotifyRepo
            , IUserPrincipalService userPrincipalService
            , IMapper mapper
            , ICacheManagementServices cacheManagementServices)
        {
            _dasRepo = dasRepo;
            _dasNotifyRepo = dasNotifyRepo;
            _userPrincipalService = userPrincipalService;
            _mapper = mapper;
            _cacheManagementServices = cacheManagementServices;
        }

        public async Task<IEnumerable<VMDashBoardPlan>> GetProcessingCollectionPlan(HomeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var tempPlanDict = await _dasRepo.Plan.GetAllListAsync(p => p.Status == (int)EnumPlan.Status.Approved && !p.IsClosed);
            var planDict = tempPlanDict.ToDictionary(n => n.ID, n => n.Name);
            var IDPlans = planDict.Select(p => p.Key);

            var totalProfiles = await _dasRepo.PlanProfile.GetAllListAsync(p => p.Status != (int)EnumProfilePlan.Status.InActive && IDPlans.Contains(p.IDPlan)
                                        && (condition.Type == (int)EnumHome.TypeDataDashBoard.Agency ? p.IDAgency == userData.IDAgency : p.IDOrgan == userData.IDOrgan));
            var totalProfileDict = totalProfiles.GroupBy(t => t.IDPlan).Select(t => new { IDPlans = t.Key, Count = t.Count() }).ToDictionary(n => n.IDPlans, n => n.Count);
            IDPlans = totalProfileDict.Select(p => p.Key);
            var totalProfileApproveds = await _dasRepo.PlanProfile.GetAllListAsync(p => p.Status == (int)EnumProfilePlan.Status.ArchiveApproved
            && IDPlans.Contains(p.IDPlan)
            && (condition.Type == (int)EnumHome.TypeDataDashBoard.Agency ? p.IDAgency == userData.IDAgency : p.IDOrgan == userData.IDOrgan));
            var totalProfileApprovedDict = totalProfileApproveds.GroupBy(t => t.IDPlan).Select(t => new { IDPlans = t.Key, Count = t.Count() }).ToDictionary(n => n.IDPlans, n => n.Count);

            List<VMDashBoardPlan> list = new List<VMDashBoardPlan>();
            foreach (var item in IDPlans)
            {
                list.Add(new VMDashBoardPlan
                {
                    IDPlan = item,
                    PlanName = planDict.GetValueOrDefault(item),
                    TotalProfile = totalProfileDict.GetValueOrDefault(item),
                    TotalProfileApproved = totalProfileApprovedDict.GetValueOrDefault(item)
                });
            }

            return list;
        }

        public async Task<VMDashBoardStorage> GetStatisticalStorageByYear(HomeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from c in _dasRepo.CatalogingProfile.GetAll()
                       where c.Status == (int)EnumCataloging.Status.StorageApproved
                       && (condition.Type == (int)EnumHome.TypeDataDashBoard.Agency ? c.IDAgency == userData.IDAgency : c.IDOrgan == userData.IDOrgan)
                       select new
                       {
                           Year = c.StartDate.HasValue ? c.StartDate.Value.Year : DateTime.Now.Year
                       };
            var model = await temp.ToListAsync();
            var group = model.GroupBy(t => t.Year).Select(t => new { Year = t.Key, TotalProfile = t.Count() }).OrderBy(t => t.Year);

            return new VMDashBoardStorage { StorageDictStr = JsonConvert.SerializeObject(group) };
        }

        public async Task<VMDashBoardProfile> ProfileAndDocByStatus(HomeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var enumPlanProfile = StringUltils.GetEnumDictionary<EnumProfilePlan.Status>();
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       where pp.Status != (int)EnumProfilePlan.Status.InActive
                       && (condition.Type == (int)EnumHome.TypeDataDashBoard.Agency ? pp.IDAgency == userData.IDAgency : pp.IDOrgan == userData.IDOrgan)
                       orderby pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved descending,
                       pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved descending,
                       pp.Status == (int)EnumProfilePlan.Status.ArchiveReject descending,
                       pp.Status == (int)EnumProfilePlan.Status.Reject descending,
                       pp.Status == (int)EnumProfilePlan.Status.WaitApprove descending,
                       pp.Status == (int)EnumProfilePlan.Status.CollectComplete descending,
                       pp.Status == (int)EnumProfilePlan.Status.Active descending
                       let p = new
                       {
                           pp.Status,
                           Name = enumPlanProfile.GetValueOrDefault(pp.Status),
                       }
                       select p;
            var model = await temp.ToListAsync();
            var group = model.GroupBy(t => t.Name).Select(t => new { Name = t.Key, TotalProfile = t.Count() });
            return new VMDashBoardProfile { ProfileDictStr = JsonConvert.SerializeObject(group) };
        }

        public async Task<VMDashBoardExpiryDate> StatisticalExpiryDate(HomeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var expiryDateDict = (await (from pp in _dasRepo.PlanProfile.GetAll()
                                         join ed in _dasRepo.ExpiryDate.GetAll() on pp.IDExpiryDate equals ed.ID
                                         where pp.Status != (int)EnumProfilePlan.Status.InActive && ed.Status == (int)EnumCommon.Status.Active
                                         && (condition.Type == (int)EnumHome.TypeDataDashBoard.Agency ? pp.IDAgency == userData.IDAgency : pp.IDOrgan == userData.IDOrgan)
                                         orderby ed.Value, ed.ID descending
                                         select new
                                         {
                                             ed.Name
                                         }).ToListAsync()).GroupBy(t => t.Name).Select(t => new { Name = t.Key, Total = t.Count() });

            return new VMDashBoardExpiryDate { ExpiryDateDictStr = JsonConvert.SerializeObject(expiryDateDict) };
        }

        public async Task<List<SelectListItem>> GetDataTypeDashBoard(HomeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
            var list = new List<SelectListItem>();
            if (userData.HasOrganPermission)
            {
                list.Add(new SelectListItem
                {
                    Value = ((int)EnumHome.TypeDataDashBoard.Agency).ToString(),
                    Text = StringUltils.GetEnumDescription(EnumHome.TypeDataDashBoard.Agency),
                    Selected = condition.Type == (int)EnumHome.TypeDataDashBoard.Agency
                });
                list.Add(new SelectListItem
                {
                    Value = ((int)EnumHome.TypeDataDashBoard.Organ).ToString(),
                    Text = StringUltils.GetEnumDescription(EnumHome.TypeDataDashBoard.Organ),
                    Selected = condition.Type == (int)EnumHome.TypeDataDashBoard.Organ
                });
            }

            return list;
        }

        public async Task<PaginatedList<VMNotification>> GetListNotificationPaging(NotificationCondition condition)
        {

            var temp = from n in _dasNotifyRepo.Notification.GetAll()
                       where n.UserId == condition.UserId
                       orderby n.CreatedDate descending
                       select n;
            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var notifications = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            List<VMNotification> vmNotifications = _mapper.Map<List<VMNotification>>(notifications);
            return new PaginatedList<VMNotification>(vmNotifications, (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<int> TotalUnreadNotification()
        {
            var temp = from n in _dasNotifyRepo.Notification.GetAll()
                       where n.UserId == _userPrincipalService.UserId && !n.IsRead
                       select n;

            return Convert.ToInt32(await temp.LongCountAsync());
        }

        public async Task<VMNotification> GetNotificationByUserId(int userId)
        {
            var noti = await _dasNotifyRepo.Notification.GetAll().Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedDate).FirstOrDefaultAsync();
            var rs = _mapper.Map<VMNotification>(noti);
            return rs;
        }

        public async Task<bool> ReadNotification(int id)
        {
            var noti = await _dasNotifyRepo.Notification.GetAsync(id);
            if (!noti.IsNotEmpty())
                return false;
            noti.IsRead = true;
            await _dasNotifyRepo.Notification.UpdateAsync(noti);
            await _dasNotifyRepo.SaveAync();
            return true;
        }
        #region Private method
        #endregion Private method
    }
}
