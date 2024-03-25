using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.DASNotify;
using ESD.Infrastructure.ContextAccessors;
using ESD.Infrastructure.Notifications;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.LogUtils;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class PlanService : BaseMasterService, IPlanServices
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;
        private readonly IHubNotificationHelper _hubNotificationHelper;
        private readonly IDistributedCache _cache;

        public PlanService(IDasRepositoryWrapper dasRepository
            , IDasNotifyRepositoryWrapper dasNotifyRepository
            , IMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService iUserPrincipalService
            , ICacheManagementServices cacheManagementServices
            , IHubNotificationHelper hubNotificationHelper
            , IDistributedCache cache) : base(dasRepository, dasNotifyRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = iUserPrincipalService;
            _cacheManagementServices = cacheManagementServices;
            _hubNotificationHelper = hubNotificationHelper;
            _cache = cache;
        }

        #region BaseRepo
        public async Task<IEnumerable<Plan>> Gets()
        {
            return await _dasRepo.Plan.GetAllListAsync();
        }
        public async Task<Plan> Get(object id)
        {
            return await _dasRepo.Plan.GetAsync(id);
        }

        public async Task<ServiceResult> Create(Plan model)
        {
            await _dasRepo.Plan.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Tạo kế hoạch thành công");
        }
        public async Task<ServiceResult> Update(Plan model)
        {
            await _dasRepo.Plan.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật kế hoạch thành công!");
        }


        #endregion BaseRepo

        #region Plan

        #region Search
        public async Task<VMIndexPlan> SearchByConditionPagging(PlanCondition condition, bool isExport)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var model = new VMIndexPlan();
            var temp = _dasRepo.Plan.GetAll()
                .Where(n => n.Status != (int)EnumPlan.Status.InActive
                && n.IDOrgan == userData.IDOrgan
                && (string.IsNullOrEmpty(condition.Keyword) || n.Name.Contains(condition.Keyword))
                && (condition.IDStatus == -1 || n.Status == condition.IDStatus || (condition.IDStatus == (int)EnumPlan.Status.Close && n.IsClosed)))
                .OrderBy(n => n.IsClosed) //) 0 before 1 > DESC
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.Active)
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.Reject)
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.NotApproved)
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.Approved)
                .ThenByDescending(n => n.UpdatedDate ?? n.CreateDate);

            model.DictStatus = Utils.EnumToDic<EnumPlan.Status>();
            model.PlanCondition = condition;
            model.DictUser = await GetDictUser();

            //Get số hồ sơ trong kê shojach

            if (!isExport)
            {
                var total = await temp.LongCountAsync();
                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var plans = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                var vmPlans = _mapper.Map<List<VMPlan>>(plans);
                model.VMPlans = new PaginatedList<VMPlan>(vmPlans, (int)total, condition.PageIndex, condition.PageSize);
            }
            else
            {
                var plans = await temp.ToListAsync();
                var vmPlans = new List<VMPlan>();
                foreach (var item in plans)
                {
                    var vmPlan = _mapper.Map<VMPlan>(item);
                    vmPlan.ApprovedByName = model.DictUser.GetValueOrDefault(item.ApprovedBy);
                    vmPlan.StrCreatedAt = Utils.DateToString(item.CreatedAt);
                    vmPlan.StrStatus = model.DictStatus.GetValueOrDefault(item.Status);
                    vmPlans.Add(vmPlan);
                }
                model.VMPlans = new PaginatedList<VMPlan>(vmPlans, vmPlans.Count(), 1, vmPlans.Count());
            }
            var ids = model.VMPlans.Select(n => n.ID).ToArray();
            model.TotalProfiles = await GetTotalProfileInPlans(ids);
            return model;
        }
        public async Task<VMIndexPlan> SearchApproveByConditionPagging(PlanCondition condition, bool isExport)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var model = new VMIndexPlan();
            var orderByStatus = new List<int> {
                (int)EnumPlan.Status.NotApproved,
                (int)EnumPlan.Status.Approved,
                //(int)EnumPlan.Status.Close,
            };
            var temp = _dasRepo.Plan.GetAll()
                .Where(n => orderByStatus.Contains(n.Status)
                       && n.IDOrgan == userData.IDOrgan
                && (string.IsNullOrEmpty(condition.Keyword) || n.Name.Contains(condition.Keyword))
                && (condition.IDStatus <= 0 || n.Status == condition.IDStatus || (condition.IDStatus == (int)EnumPlan.Status.Close && n.IsClosed))
                && (n.ApprovedBy == _userPrincipalService.UserId))
                .OrderBy(n => n.IsClosed) //) 0 before 1 > DESC
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.Active)
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.NotApproved)
                .ThenByDescending(n => n.Status == (int)EnumPlan.Status.Approved)
                //.ThenByDescending(n => n.Status == (int)EnumPlan.Status.Close)
                .ThenByDescending(n => n.UpdatedDate ?? n.CreateDate);

            model.DictStatus = Utils.EnumToDic<EnumPlan.Status>().Where(n => n.Key != (int)EnumPlan.Status.Active && n.Key != (int)EnumPlan.Status.Reject).ToDictionary(n => n.Key, n => n.Value);
            model.PlanCondition = condition;
            model.DictUser = await GetDictUser();

            if (!isExport)
            {
                var total = await temp.LongCountAsync();
                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var plans = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                var vmPlans = _mapper.Map<List<VMPlan>>(plans);
                model.VMPlans = new PaginatedList<VMPlan>(vmPlans, (int)total, condition.PageIndex, condition.PageSize);
            }
            else
            {
                var plans = await temp.ToListAsync();
                var vmPlans = new List<VMPlan>();
                foreach (var item in plans)
                {
                    var vmPlan = _mapper.Map<VMPlan>(item);
                    vmPlan.ApprovedByName = model.DictUser.GetValueOrDefault(item.ApprovedBy);
                    vmPlan.StrCreatedAt = Utils.DateToString(item.CreatedAt);
                    vmPlan.StrStatus = model.DictStatus.GetValueOrDefault(item.Status);
                    vmPlans.Add(vmPlan);
                }
                model.VMPlans = new PaginatedList<VMPlan>(vmPlans, vmPlans.Count(), 1, vmPlans.Count());
            }
            var ids = model.VMPlans.Select(n => n.ID).ToArray();
            model.TotalProfiles = await GetTotalProfileInPlans(ids);

            return model;
        }
        #endregion Search

        #region Create 
        public async Task<VMCreatePlan> Create()
        {
            var model = new VMCreatePlan { CreatedAt = Utils.DateToString(DateTime.Now) };
            model.DictUser = await GetDictUser();
            return model;
        }
        public async Task<ServiceResult> CreatePlan(VMCreatePlan vmPlan, int status = 0)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //check exist unique field
            var isExist = await _dasRepo.Plan.AnyAsync(m => m.Name.Trim() == vmPlan.Name.Trim() && m.Status != (int)EnumPlan.Status.InActive && userData.IDOrgan == m.IDOrgan);
            if (isExist)
                return new ServiceResultError("Tên kế hoạch đã tồn tại!");

            //update data
            if (status == 0)
                vmPlan.Status = (int)EnumPlan.Status.Active;

            var plan = Utils.Bind<Plan>(vmPlan.KeyValue());
            plan.IsClosed = vmPlan.IsClosed > 0;
            plan.IDOrgan = userData.IDOrgan;

            GetPlanDates(vmPlan, plan, out List<object> lstErr);
            if (lstErr.IsNotEmpty())
                return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

            await _dasRepo.Plan.InsertAsync(plan);
            await _dasRepo.SaveAync();
            if (plan.ID == 0)
                return new ServiceResultError("Thêm mới kế hoạch không thành công!");
            return new ServiceResultSuccess("Thêm mới kế hoạch thành công!");
        }

        #endregion Create

        #region Update 
        public async Task<VMCreatePlan> Edit(int id = 0)
        {
            var plan = await Get(id) ?? new Plan();
            var model = _mapper.Map<VMCreatePlan>(plan);
            model.IsClosed = plan.IsClosed ? 1 : 0;
            model.CreatedAt = Utils.DateToString(plan.CreatedAt);
            if (plan.FromDate.HasValue && plan.FromDate != DateTime.MinValue)
                model.FromDate = Utils.DateToString(plan.FromDate);
            if (plan.EndDate.HasValue && plan.EndDate != DateTime.MinValue)
                model.EndDate = Utils.DateToString(plan.EndDate);
            model.DictUser = await GetDictUser();
            return model;
        }
        public async Task<ServiceResult> UpdatePlan(VMCreatePlan vmPlan)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var isExist = await _dasRepo.Plan.AnyAsync(m => m.Name.Trim() == vmPlan.Name.Trim() && m.Status != (int)EnumPlan.Status.InActive && userData.IDOrgan == m.IDOrgan && m.ID != vmPlan.ID);
            if (isExist)
                return new ServiceResultError("Tên kế hoạch đã tồn tại!");

            var plan = Utils.Bind<Plan>(vmPlan.KeyValue());
            plan.IsClosed = vmPlan.IsClosed > 0;
            plan.IDOrgan = userData.IDOrgan;

            GetPlanDates(vmPlan, plan, out List<object> lstErr);
            if (lstErr.IsNotEmpty())
                return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

            await _dasRepo.Plan.UpdateAsync(plan);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật kế hoạch thành công!");
        }
        #endregion

        #region Delete
        public async Task<ServiceResult> Delete(object id)
        {
            try
            {
                //Logic delete
                var plan = await _dasRepo.Plan.GetAsync(id);
                if (plan == null || plan.Status == (int)EnumPlan.Status.InActive)
                    return new ServiceResultError("kế hoạch này hiện không tồn tại hoặc đã bị xóa");

                if (plan.Status != (int)EnumPlan.Status.Active && plan.Status != (int)EnumPlan.Status.Reject)
                    return new ServiceResultError($"Kế hoạch không được phép xoá!");

                plan.Status = (int)EnumPlan.Status.InActive;
                await _dasRepo.Plan.UpdateAsync(plan);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa kế hoạch thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> Deletes(IEnumerable<int> ids)
        {
            try
            {
                //Logic delete
                var plans = await _dasRepo.Plan.GetAllListAsync(n => ids.Contains(n.ID));
                if (plans == null || plans.Count() == 0)
                    return new ServiceResultError("Kế hoạch đã chọn hiện không tồn tại hoặc đã bị xóa");
                if (plans.Any(n => n.Status != (int)EnumPlan.Status.Active && n.Status != (int)EnumPlan.Status.Reject))
                {
                    var names = new List<string>();
                    foreach (var item in plans)
                    {
                        if (item.Status != (int)EnumPlan.Status.Active && item.Status != (int)EnumPlan.Status.Reject)
                            names.Add(item.Name);
                    }
                    return new ServiceResultError($"Kế hoạch {string.Join(", ", names)} không được phép xoá!");
                }
                foreach (var pos in plans)
                {
                    pos.Status = (int)EnumPlan.Status.InActive;
                }
                await _dasRepo.Plan.UpdateAsync(plans);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa kế hoạch thành công");

                ////check bảng RoleGroupPer, PlanPer
                //await _dasRepo.RoleGroupPer.DeleteAsync(s => ids.Contains(s.IDPlan));
                //await _dasRepo.PlanPer.DeleteAsync(s => ids.Contains(s.IDPlan));
                //await _dasRepo.Plan.DeleteAsync(s => ids.Contains(s.ID));

                //await _dasRepo.SaveAync();
                //return new ServiceResultSuccess("Xóa kế hoạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete


        #region Approve/Reject
        public async Task<ServiceResult> SendApprovePlan(int id)
        {
            var plan = await Get(id);
            if (plan == null)
                return new ServiceResultError("Kế hoạch không còn tồn tại!");

            var hasProfile = await _dasRepo.PlanProfile.AnyAsync(n => n.IDPlan == id);
            if (!hasProfile)
                return new ServiceResultError("Kế hoạch chưa có hồ sơ nào, vui lòng bổ sung!");


            if (plan.Status != (int)EnumPlan.Status.Active && plan.Status != (int)EnumPlan.Status.Reject)
                return new ServiceResultError("Kế hoạch đã được gửi duyệt!");

            plan.SendApprovedBy = _userPrincipalService.UserId;
            plan.Status = (int)EnumPlan.Status.NotApproved;
            plan.Reason = string.Empty;
            await _dasRepo.Plan.UpdateAsync(plan);
            await _dasRepo.SaveAync();

            string content = string.Format("{0} gửi yêu cầu phê duyệt kế hoạch thu thập {1}", _userPrincipalService.UserName, plan.Name);

            //insert notify to db
            List<Notification> listNoti = new List<Notification>();
            listNoti.Add(new Notification
            {
                UserId = plan.ApprovedBy,
                Content = content,
                IsRead = false,
                CreatedDate = DateTime.Now,
                Url = "/Plan/ApproveCatalogingIndex?IDPlan=" + plan.ID
            });
            if (listNoti.IsEmpty())
                return new ServiceResultSuccess("Gửi duyệt kế hoạch thành công!");
            await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            await _dasNotifyRepo.SaveAync();

            //send notify to userIds
            await _hubNotificationHelper.PushToUser(plan.ApprovedBy);

            return new ServiceResultSuccess("Gửi duyệt kế hoạch thành công!");
        }
        public async Task<ServiceResult> ApprovePlan(int id)
        {
            var plan = await Get(id);
            if (plan.ApprovedBy != _userPrincipalService.UserId)
            {
                return new ServiceResultError("Bạn không có quyền duyệt kế hoạch này!"); //Todo
            }
            //Duyệt
            if (plan.Status != (int)EnumPlan.Status.NotApproved)
                return new ServiceResultError("Kế hoạch đã được duyệt!");

            plan.Status = (int)EnumPlan.Status.Approved;
            plan.ApprovedDate = DateTime.Now;
            await _dasRepo.Plan.UpdateAsync(plan);
            await _dasRepo.SaveAync();

            string content = string.Format("{0} duyệt kế hoạch thu thập {1}", _userPrincipalService.UserName, plan.Name);

            //1. send notify to user sendapprovedby
            List<int> userIds = new List<int>();
            userIds.Add(plan.SendApprovedBy);
            List<Notification> listNoti = new List<Notification>();
            listNoti.Add(new Notification
            {
                UserId = plan.SendApprovedBy,
                Content = content,
                IsRead = false,
                CreatedDate = DateTime.Now,
                Url = "/Plan/CatalogingIndex?IDPlan=" + plan.ID
            });
            await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            await _dasNotifyRepo.SaveAync();
            await _hubNotificationHelper.PushToUser(plan.SendApprovedBy);

            //2. send notify to user have permission access page CollectionManagement
            //get listUserId have permission access page CollectionManagement
            //get users by permission
            var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            if (dict.IsEmpty())
                return new ServiceResultSuccess("Duyệt kế hoạch thành công!");

            List<UserPermissionModel> existeds = new List<UserPermissionModel>();
            userIds = new List<int>();
            foreach (var item in dict)
            {
                var per = dict.GetValueOrDefault(item.Key);
                if (per.IsEmpty())
                    continue;
              
                userIds.Add(item.Key);
            }
            if (userIds.IsEmpty())
                return new ServiceResultSuccess("Duyệt kế hoạch thành công!");

            //get listAgency in plan
            var tempAgencyIds = await ((from pp in _dasRepo.PlanProfile.GetAll()
                                    where pp.IDPlan == plan.ID && pp.Status == (int)EnumProfilePlan.Status.Active
                                    select new {
                                        ID = pp.IDAgency
                                    }).ToListAsync());
            var agencyIds = tempAgencyIds.Select(t => t.ID).ToList();
            var tempUserIds = await _dasRepo.User.GetAllListAsync(u => u.Status == (int)EnumCommon.Status.Active && agencyIds.Contains(u.IDAgency) && userIds.Contains(u.ID));
            userIds = tempUserIds.Select(t => t.ID).ToList();
            listNoti = new List<Notification>();
            foreach(var userId in userIds)
            {
                listNoti.Add(new Notification
                {
                    UserId = userId,
                    Content = content,
                    IsRead = false,
                    CreatedDate = DateTime.Now,
                    Url = "/CollectionManagement"
                });
            }
            
            await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            await _dasNotifyRepo.SaveAync();
            foreach (var userId in userIds)
            {
                await _hubNotificationHelper.PushToUser(userId);
            }
            return new ServiceResultSuccess("Duyệt kế hoạch thành công!");
        }
        public async Task<ServiceResult> RejectPlan(int id, string reason = "")
        {
            var plan = await Get(id);
            if (plan.ApprovedBy != _userPrincipalService.UserId)
            {
                return new ServiceResultError("Bạn không có quyền từ chối kế hoạch này!"); //Todo
            }
            //Từ chối
            if (plan.Status != (int)EnumPlan.Status.NotApproved)
                return new ServiceResultError("Kế hoạch đã bị từ chối!");

            plan.Status = (int)EnumPlan.Status.Reject;
            plan.Reason = reason;
            await _dasRepo.Plan.UpdateAsync(plan);
            await _dasRepo.SaveAync();

            string content = string.Format("{0} đã từ chối kế hoạch thu thập {1}", _userPrincipalService.UserName, plan.Name);

            //insert notify to db
            List<Notification> listNoti = new List<Notification>();
            listNoti.Add(new Notification
            {
                UserId = plan.SendApprovedBy,
                Content = content,
                IsRead = false,
                CreatedDate = DateTime.Now,
                Url = "/Plan/CatalogingIndex?IDPlan=" + plan.ID
            });
            if (listNoti.IsEmpty())
                return new ServiceResultSuccess("Đã từ chối kế hoạch!");
            await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            await _dasNotifyRepo.SaveAync();

            //send notify to userIds
            await _hubNotificationHelper.PushToUser(plan.SendApprovedBy);

            return new ServiceResultSuccess("Đã từ chối kế hoạch!");
        }
        public async Task<ServiceResult> ClosePlan(int id)
        {
            var plan = await _dasRepo.Plan.GetAsync(id);
            if (plan == null || plan.Status == (int)EnumPlan.Status.InActive)
                return new ServiceResultError("Không được phép đống, kế hoạch này hiện không tồn tại hoặc đã bị xóa");

            var notAllowClose = await _dasRepo.PlanProfile.AnyAsync(n => n.IDPlan == id && (n.Status != (int)EnumProfilePlan.Status.ArchiveApproved && n.Status != (int)EnumProfilePlan.Status.InActive)); //Check xem toàn bộ hồ sơ trong kế hoạch đã được "Duyệt nộp lưu" hay chưa
            if (notAllowClose)
                return new ServiceResultError("Không được phép đóng, kế hoạch đang trong giai đoạn thu thập");

            plan.IsClosed = true;
            await _dasRepo.Plan.UpdateAsync(plan);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Đóng kế hoạch thành công!");
        }
        public async Task<ServiceResult> OpenPlan(int id)
        {
            var plan = await _dasRepo.Plan.GetAsync(id);
            if (plan == null || plan.Status == (int)EnumPlan.Status.InActive)
                return new ServiceResultError("Kế hoạch này hiện không tồn tại hoặc đã bị xóa");

            if (!plan.IsClosed)
                return new ServiceResultError("Kế hoạch này đã được mở");

            plan.IsClosed = false;
            await _dasRepo.Plan.UpdateAsync(plan);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Mở kế hoạch thành công!");
        }
        #endregion

        #endregion PLan

        #region PlanProfile

        #region List
        public async Task<VMIndexPlanProfile> CatalogingIndex(PlanProfileCondition condition, bool isExport = false)
        {
            return new VMIndexPlanProfile
            {
                PlanProfileCondition = condition,
                VMPlan = _mapper.Map<VMPlan>(await Get(condition.IDPlan)),
                DictUser = await GetDictUser(),
                DictAgency = await GetDictAgencies(0),
                VMPlanProfiles = await GetPlanProfiles(condition, isExport),
                DictExpiryDate = await GetDictExpiryDate(),
                DictProfileTemplate = await GetDictProfileTemplate(0),
                DictStatus = Utils.EnumToDic<EnumPlan.Status>(),
                DictProfileType = Utils.EnumToDic<EnumProfile.Type>()
            };
        }

        public async Task<VMPlanProfile> GetPlanProfile(int? id)
        {
            if (id == null)
                return new VMPlanProfile();
            return _mapper.Map<VMPlanProfile>(await _dasRepo.PlanProfile.GetAsync(id.Value) ?? new PlanProfile());
        }
        #endregion

        #region Create

        public async Task<VMUpdatePlanProfile> CreatePlanProfile(int id)
        {
            var model = new VMUpdatePlanProfile()
            {
                IDPlan = id
            };
            await GetUpdateModel(model);
            return model;
        }

        public async Task<ServiceResult> CreatePlanProfile(VMUpdatePlanProfile vmProfile)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var data = vmProfile.KeyValue();
                var profile = Utils.Bind<PlanProfile>(data);
                GetPlanProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

                var language = Utils.GetStrings(data, nameof(VMUpdateProfile.Language));
                profile.Language = language.IsNotEmpty() ? Utils.Serialize(language) : string.Empty;

                profile.Status = (int)EnumProfile.Status.Active;
                if (await _dasRepo.PlanProfile.IsCodeExist(profile.FileCode, (int)EnumProfile.Status.Active))
                    return new ServiceResultError("Mã hồ sơ đã tồn tại");

                var font = await _dasRepo.ProfileTemplate.GetAsync(profile.IDProfileTemplate) ?? new ProfileTemplate();
                if (font.Type == (int)EnumProfileTemplate.Type.Close)
                    return new ServiceResultError("Phông đang chọn đã bị đóng, vui lòng chọn phông khác");

                profile.ID = 0;
                profile.IDOrgan = userData.IDOrgan;
                profile.Type = (int)EnumProfile.Type.Digital;

                await _dasRepo.PlanProfile.InsertAsync(profile);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Thêm hồ sơ thành công");
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        #endregion Create

        #region Update

        public async Task<VMUpdatePlanProfile> UpdatePlanProfile(int? id)
        {
            var profile = await _dasRepo.PlanProfile.GetAsync(id.Value) ?? new PlanProfile();
            var model = Utils.Bind<VMUpdatePlanProfile>(profile.KeyValue());
            model.StartDate = Utils.DateToString(profile.StartDate);
            model.EndDate = Utils.DateToString(profile.EndDate);
            model.VMPlan = _mapper.Map<VMPlan>(await _dasRepo.Plan.GetAsync(profile.IDPlan) ?? new Plan());
            await GetUpdateModel(model);
            return model;
        }
        public async Task<ServiceResult> UpdatePlanProfile(VMUpdatePlanProfile vmProfile)
        {
            try
            {
                //1. Update Profile
                var profile = await _dasRepo.PlanProfile.GetAsync(vmProfile.ID);
                if (profile == null)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");
                var data = vmProfile.KeyValue();
                profile.Bind(data);
                GetPlanProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

                if (await _dasRepo.PlanProfile.IsCodeExist(profile.FileCode, (int)EnumProfile.Status.Active, vmProfile.ID))
                    return new ServiceResultError("Mã hồ sơ đã tồn tại");

                var font = await _dasRepo.ProfileTemplate.GetAsync(profile.IDProfileTemplate) ?? new ProfileTemplate();
                if (font.Type == (int)EnumProfileTemplate.Type.Close)
                    return new ServiceResultError("Phông đang chọn đã bị đóng, vui lòng chọn phông khác");

                var language = Utils.GetStrings(data, nameof(VMUpdateProfile.Language));
                profile.Language = language.IsNotEmpty() ? Utils.Serialize(language) : string.Empty;

                await _dasRepo.PlanProfile.UpdateAsync(profile);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật hồ sơ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeletePlanProfile(int id)
        {
            try
            {
                //Logic delete
                var positionDelete = await _dasRepo.PlanProfile.GetAsync(id);
                if (positionDelete == null || positionDelete.Status == (int)EnumCommon.Status.InActive)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");

                positionDelete.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.PlanProfile.UpdateAsync(positionDelete);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa hồ sơ thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> DeletePlanProfiles(IEnumerable<int> ids)
        {
            try
            {
                //Logic delete
                var positionDeletes = await _dasRepo.PlanProfile.GetAllListAsync(n => ids.Contains(n.ID));
                if (positionDeletes == null || positionDeletes.Count() == 0)
                    return new ServiceResultError("Hồ sơ đã chọn hiện không tồn tại hoặc đã bị xóa");

                foreach (var pos in positionDeletes)
                {
                    pos.Status = (int)EnumCommon.Status.InActive;
                }
                await _dasRepo.PlanProfile.UpdateAsync(positionDeletes);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa hồ sơ thành công");

                ////check bảng RoleGroupPer, ProfilePer
                //await _dasRepo.RoleGroupPer.DeleteAsync(s => ids.Contains(s.IDProfile));
                //await _dasRepo.ProfilePer.DeleteAsync(s => ids.Contains(s.IDProfile));
                //await _dasRepo.Profile.DeleteAsync(s => ids.Contains(s.ID));

                //await _dasRepo.SaveAync();
                //return new ServiceResultSuccess("Xóa hồ sơ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete

        #endregion

        #region CollectionManagement
        #region Search
        public async Task<VMIndexPlan> SearchByConditionPlanCollection(PlanCondition condition, bool isExport = false)
        {
            var model = new VMIndexPlan
            {
                VMPlans = await SearchByConditionPaggingByAgency(condition, isExport)
            };
            var ids = model.VMPlans.Select(n => n.ID).ToArray();
            model.CountProfiles = await GetTotalProfileInPlansByAgency(ids);
            return model;
        }
        /// <summary>
        /// Lây các Plan theo IDAgency có liên quan
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="agencyID"></param>
        /// <returns></returns>
        public async Task<PaginatedList<VMPlan>> SearchByConditionPaggingByAgency(PlanCondition condition, bool isExport = false)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //Lấy Hồ sơ liên quan Agency
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where conditionStr
                       && (pp.Status == (int)EnumProfilePlan.Status.Active
                       || pp.Status == (int)EnumProfilePlan.Status.WaitApprove
                       || pp.Status == (int)EnumProfilePlan.Status.CollectComplete
                       || pp.Status == (int)EnumProfilePlan.Status.Reject)
                       //orderby pp.ID descending
                       select pp.IDPlan;
            var listIdPlan = await temp.ToListAsync();
            //Lây plan
            var temp1 = from p in _dasRepo.Plan.GetAll()
                .Where(x => x.Status == (int)EnumPlan.Status.Approved
                && (string.IsNullOrEmpty(condition.Keyword) || x.Name.Contains(condition.Keyword))
                && (condition.IDStatus == -1 || x.Status == condition.IDStatus))
                .Where(x => listIdPlan.Distinct().Contains(x.ID))
                        join u in _dasRepo.User.GetAll() on p.ApprovedBy equals u.ID
                        orderby p.UpdatedDate descending
                        select new VMPlan
                        {
                            ID = p.ID,
                            IDChannel = p.IDChannel,
                            Name = p.Name,
                            ApprovedBy = p.ApprovedBy,
                            ApprovedByName = u.Name,
                            CreatedAt = p.CreatedAt,
                            Content = p.Content,
                            FromDate = p.FromDate,
                            EndDate = p.EndDate,
                            Status = p.Status,
                            CreatedBy = p.CreatedBy,
                            CreateDate = p.CreateDate,
                            UpdatedDate = p.UpdatedDate,
                            UpdatedBy = p.UpdatedBy
                        };
            if (isExport)
            {
                var rs = await temp1.ToListAsync();
                return new PaginatedList<VMPlan>(rs, rs.Count(), 1, rs.Count());
            }
            var total = await temp1.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var plans = await temp1.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var vmPlans = _mapper.Map<List<VMPlan>>(plans);
            return new PaginatedList<VMPlan>(vmPlans, (int)total, condition.PageIndex, condition.PageSize);
        }
        /// <summary>
        /// Pagging các hồ sơ trong kế hoạch, lấy theo Agency của User
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<VMIndexPlanProfile> GetDocumentCollectListCondition(PlanProfileCondition condition, bool isExport = false)
        {
            var model = new VMIndexPlanProfile
            {
                VMPlanProfiles = await GetProfileCollect(condition, isExport),
                PlanProfileCondition = condition,
                VMPlan = _mapper.Map<VMPlan>(await Get(condition.IDPlan)),
                DictUser = await GetDictUser(),
                DictExpiryDate = await GetDictExpiryDate()
            };
            var ids = model.VMPlanProfiles.Select(n => n.ID).ToArray();
            model.TotalDocs = await GetTotalDocInProfiles(ids);
            return model;
            //return new VMIndexPlanProfile
            //{
            //    VMPlanProfiles = await GetProfileCollect(condition),
            //    PlanProfileCondition = condition,
            //    VMPlan = _mapper.Map<VMPlan>(await Get(condition.IDPlan)),
            //    DictUser = await GetDictUser(),
            //    DictExpiryDate = await GetDictExpiryDate(),
            //    //TotalDocs = await GetTotalDocInProfiles(VMPlanProfiles.Select(n => n.ID).ToArray())
            //};
        }
        public async Task<ServiceResult> UpdatePlanProfileInCollect(VMUpdatePlanProfile vmProfile)
        {
            try
            {
                //1. Update Profile
                var profile = await _dasRepo.PlanProfile.GetAsync(vmProfile.ID);
                if (profile == null)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");

                var data = vmProfile.KeyValue();
                //Các trường không đổi
                var fileCode = profile.FileCode;
                var idProfileTemplate = profile.IDProfileTemplate;
                var status = profile.Status;
                var identifier = profile.Identifier;
                profile.Bind(data);

                GetPlanProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

                profile.FileCode = fileCode;
                profile.IDProfileTemplate = idProfileTemplate;
                profile.Status = status;
                profile.Identifier = identifier;
                profile.Language = Utils.Serialize(Utils.GetStrings(data, nameof(VMUpdateProfile.Language)));
                await UpdateDocWhenProfileChange(profile);
                await _dasRepo.PlanProfile.UpdateAsync(profile);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật hồ sơ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }


        #endregion Search

        #region CollectionManagement_Business
        public async Task<ServiceResult> SendApproveProfile(int id)
        {
            try
            {
                //get profile by id
                var profile = await _dasRepo.PlanProfile.GetAsync(id);
                if (profile == null || profile.Status == (int)EnumProfilePlan.Status.InActive)
                    return new ServiceResultError("Hồ sơ không tồn tại!");

                //check status profile
                if (profile.Status == (int)EnumProfilePlan.Status.WaitApprove)
                    return new ServiceResultError("Có lỗi xảy ra, vui lòng tải lại trang!");

                //Check status Doc
                var docs = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == id && x.Status == (int)EnumDocCollect.Status.Active);
                if (docs != null && docs.Count() > 0)
                    return new ServiceResultError("Có tài liệu chưa hoàn thành biên muc");

                docs = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == id && x.Status != (int)EnumDocCollect.Status.InActive);
                if (docs == null || docs.Count() == 0)
                    return new ServiceResultError("Hồ sơ chưa có tài liệu nào");

                //update status profile
                profile.Status = (int)EnumProfilePlan.Status.WaitApprove;

                //update this profile
                await _dasRepo.PlanProfile.UpdateAsync(profile);
                await _dasRepo.SaveAync();

                //get users by permission
                var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
                if (dict.IsEmpty())
                    return new ServiceResultSuccess("Gửi xét duyệt hồ sơ thành công!");

                List<int> userIds = new List<int>();
                List<UserPermissionModel> existeds = new List<UserPermissionModel>();
                foreach (var item in dict)
                {
                    var per = dict.GetValueOrDefault(item.Key);
                    if (per.IsEmpty())
                        continue;
                    userIds.Add(item.Key);
                }
                if (userIds.IsEmpty())
                    return new ServiceResultSuccess("Gửi xét duyệt hồ sơ thành công!");

                var tempUserIds = await _dasRepo.User.GetAllListAsync(u => u.Status == (int)EnumCommon.Status.Active && u.IDAgency == profile.IDAgency && userIds.Contains(u.ID));
                userIds = tempUserIds.Select(t => t.ID).ToList();
                if (userIds.IsEmpty())
                    return new ServiceResultSuccess("Gửi xét duyệt hồ sơ thành công!");

                string content = string.Format("{0} gửi yêu cầu phê duyệt hồ sơ thu thập {1}", _userPrincipalService.UserName, profile.Title);

                //insert notify to db
                List<Notification> listNoti = new List<Notification>();
                foreach (var userId in userIds)
                {
                    listNoti.Add(new Notification
                    {
                        UserId = userId,
                        Content = content,
                        IsRead = false,
                        CreatedDate = DateTime.Now
                    });
                }
                await _dasNotifyRepo.Notification.InsertAsync(listNoti);
                await _dasNotifyRepo.SaveAync();
                foreach (var userId in userIds)
                {
                    //send notify to userIds
                    await _hubNotificationHelper.PushToUser(userId);
                }

                return new ServiceResultSuccess("Gửi xét duyệt hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion CollectionManagement_Business

        #region Private Func

        private async Task<ServiceResult> UpdateDocWhenProfileChange(PlanProfile profile)
        {
            List<string> listFieldChange = new List<string> { "FileCode", "Identifier", "Organld", "FileCatalog", "FileNotation" };
            var temp = from d in _dasRepo.Doc.GetAll()
                       from dt in _dasRepo.DocType.GetAll().Where(x => x.ID == d.IDDocType).DefaultIfEmpty()
                       from dtf in _dasRepo.DocTypeField.GetAll().Where(x => x.IDDocType == dt.ID).DefaultIfEmpty()
                       from df in _dasRepo.DocField.GetAll().Where(x => x.IDDoc == d.ID && x.IDDocTypeField == dtf.ID).DefaultIfEmpty()
                       where d.Status != (int)EnumDocCollect.Status.InActive
                       && d.IDProfile == profile.ID
                       && dt.Type == (int)EnumDocType.Type.Doc
                       && listFieldChange.Contains(dtf.Code)
                       select new
                       {
                           docField = df,
                           FieldCode = dtf.Code
                       };
            var rs = await temp.ToListAsync();
            if (rs != null && rs.Count() > 0)
            {
                foreach (var df in rs)
                {
                    switch (df.FieldCode)
                    {
                        case "FileCode":
                            df.docField.Value = profile.FileCode;
                            break;
                        case "Identifier":
                            df.docField.Value = profile.Identifier;
                            break;
                        case "Organld":
                            df.docField.Value = profile.IDProfileTemplate.ToString();
                            break;
                        case "FileCatalog":
                            df.docField.Value = profile.FileCatalog.ToString();
                            break;
                        case "FileNotation":
                            df.docField.Value = profile.FileNotation;
                            break;
                        //case "OrganName":
                        //    df.docField.Value = (await _dasRepo.Organ.GetAsync(idOrgan)).Name; 
                        //    break;
                        //Organ không đổi
                        default:
                            break;
                    }
                }
            }
            var listDocField = rs.Select(x => x.docField);
            await _dasRepo.DocField.UpdateAsync(listDocField);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật tài liệu thành công");
        }

        /// <summary>
        /// Lây các hồ sơ trong kế hoạch thu thập
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMPlanProfile>> GetProfileCollect(PlanProfileCondition condition, bool isExport = false)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            List<int> orderStatus = new List<int> {
                (int)EnumProfilePlan.Status.Reject,
                (int)EnumProfilePlan.Status.WaitApprove,
                (int)EnumProfilePlan.Status.CollectComplete,
                (int)EnumProfilePlan.Status.Active,
            };
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (pp.Status == (int)EnumProfilePlan.Status.Active || pp.Status == (int)EnumProfilePlan.Status.CollectComplete || pp.Status == (int)EnumProfilePlan.Status.WaitApprove || pp.Status == (int)EnumProfilePlan.Status.Reject)
                       && (pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword))
                       && (condition.IDStatus == -1 || condition.IDStatus == pp.Status)
                       //&& (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(pp.Status.ToString()))
                       join a in _dasRepo.Agency.GetAll() on pp.IDAgency equals a.ID
                       where conditionStr
                       //orderby orderStatus.IndexOf(pp.Status), pp.ID descending
                       select _mapper.Map<VMPlanProfile>(pp);
            var profile = new List<VMPlanProfile>();
            if (condition.PageIndex == -1)
            {
                //Nopaging
                profile = await temp.OrderBy(x => orderStatus.IndexOf(x.Status)).ToListAsync();
                return new PaginatedList<VMPlanProfile>(profile, profile.Count, 1, profile.Count);
            }
            if (isExport)
            {
                profile = ((await temp.ToListAsync()).OrderBy(x => orderStatus.IndexOf(x.Status)).ThenByDescending(x => x.ID)).ToList();
                return new PaginatedList<VMPlanProfile>(profile, profile.Count, 1, profile.Count);
            }

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var temprs = (await temp.ToListAsync()).OrderBy(x => orderStatus.IndexOf(x.Status)).ThenByDescending(x => x.ID);
            profile = temprs.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();
            return new PaginatedList<VMPlanProfile>(profile, (int)total, condition.PageIndex, condition.PageSize);
        }
        #endregion Private Func
        #endregion CollectionManagement

        #region ReceiveArchive

        #region Search
        /// <summary>
        /// DS kế hoạch (group theo kế hoạch và đơn vị, phòng ban)
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<VMIndexPlan> ReceiveArchiveIndex(PlanCondition condition)
        {
            var model = new VMIndexPlan();

            var temp1 = from doc in _dasRepo.Doc.GetAll()
                        join profile in _dasRepo.PlanProfile.GetAll() on doc.IDProfile equals profile.ID
                        join plan in _dasRepo.Plan.GetAll() on profile.IDPlan equals plan.ID
                        join agency in _dasRepo.Agency.GetAll() on profile.IDAgency equals agency.ID
                        where
                        ((!condition.IsReceived && profile.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved)  //ds chờ nhận
                        || (condition.IsReceived && profile.Status == (int)EnumProfilePlan.Status.ArchiveApproved)) //Ds nhận
                        && (condition.Keyword.IsEmpty() || (condition.Keyword.IsNotEmpty() && (plan.Name.Contains(condition.Keyword))))
                        && (condition.IDAgeny <= 0 || (condition.IDAgeny > 0 && condition.IDAgeny == agency.ID))
                        && (condition.IDPlan <= 0 || (condition.IDPlan > 0 && condition.IDPlan == profile.IDPlan))
                        && plan.IDOrgan == _userPrincipalService.IDOrgan
                        && (doc.Status == (int)EnumDocCollect.Status.Active || doc.Status == (int)EnumDocCollect.Status.Complete)
                        && agency.Status == (int)EnumAgency.Status.Active
                        group new { plan, agency, IDProfile = profile.ID, IDDoc = doc.ID } by new
                        {
                            PlanID = plan.ID,
                            PlanName = plan.Name,
                            AgencyID = agency.ID,
                            AgencyName = agency.Name,
                            IdProfile = profile.ID
                        } into g
                        select new
                        {
                            IDPlan = g.Key.PlanID,
                            IDAgency = g.Key.AgencyID,
                            g.Key.PlanName,
                            g.Key.AgencyName,
                            g.Key.IdProfile,
                            CountDoc = g.Count()
                        };


            var total = await temp1.LongCountAsync();

            var temp = from x in temp1
                       group x by new { x.IDPlan, x.IDAgency, x.PlanName, x.AgencyName }
                      into gr
                       select new VMPlan
                       {
                           ID = gr.Key.IDPlan,
                           AgencyID = gr.Key.IDAgency,
                           Name = gr.Key.PlanName,
                           AgencyName = gr.Key.AgencyName,
                           TotalProfile = gr.Count(),
                           TotalDoc = gr.Sum(x => x.CountDoc)
                       };

            total = await temp.LongCountAsync();

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var plans = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMPlans = new PaginatedList<VMPlan>(plans, (int)total, condition.PageIndex, condition.PageSize);
            model.DictPlans = (await _dasRepo.Plan.GetAllListAsync(n => n.Status != (int)EnumPlan.Status.InActive)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
            model.DictAgencies = await GetDictAgencies(0);
            model.PlanCondition = condition;
            return model;
        }

        /// <summary>
        /// Ds hồ sơ trong kế hoạch 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<VMIndexPlanProfile> ReceiveArchiveProfileIndex(PlanProfileCondition condition, bool isExport = false)
        {
            var model = new VMIndexPlanProfile();
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                           //join p in _dasRepo.Plan.GetAll() on pp.IDPlan equals p.ID
                           //join a in _dasRepo.Agency.GetAll() on pp.IDAgency equals a.ID
                       where
                     ((condition.IsReceived == 0 && pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved) || (condition.IsReceived > 0 && pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved))
                       && pp.IDPlan == condition.IDPlan
                       && (condition.IDAgency == 0 || (condition.IDAgency > 0 && pp.IDAgency == condition.IDAgency))
                       && (condition.Keyword.IsEmpty() || (condition.Keyword.IsNotEmpty() && (pp.Title.Contains(condition.Keyword) || pp.FileNotation.Contains(condition.Keyword))))
                       //group pp by new { IDAgency = pp.IDAgency, p.ID, p.Name, AgencyName = a.Name } into gg
                       orderby pp.ID descending
                       select _mapper.Map<VMPlanProfile>(pp);

            model.DictAgency = await GetDictAgencies(0);
            model.DictExpiryDate = await GetDictExpiryDate();
            model.DictProfileType = Utils.EnumToDic<EnumProfile.Type>();
            model.PlanProfileCondition = condition;
            if (isExport)
            {
                var profiles = await temp.ToListAsync();
                model.VMPlanProfiles = new PaginatedList<VMPlanProfile>(profiles, profiles.Count(), 1, profiles.Count());
                foreach (var item in model.VMPlanProfiles)
                {
                    item.ExpiryDateName = model.DictExpiryDate.GetValueOrDefault(item.IDExpiryDate);
                    item.ProfileTime = Utils.DateToString(item.StartDate) + " - " + Utils.DateToString(item.EndDate);
                    item.MaintenanceAndPageNumber = item.Maintenance + "/" + item.PageNumber;
                    item.AgencyName = model.DictAgency.GetValueOrDefault(item.IDAgency);
                }
            }
            else
            {
                var total = await temp.LongCountAsync();
                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var profiles = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                model.VMPlanProfiles = new PaginatedList<VMPlanProfile>(profiles, (int)total, condition.PageIndex, condition.PageSize);
            }

            model.AgenctyName = (await _dasRepo.Agency.GetAsync(model.VMPlanProfiles.FirstOrNewObj().IDAgency) ?? new Agency()).Name;
            model.TotalDocs = await GetTotalDocInProfiles(model.VMPlanProfiles.Select(n => n.ID).ToArray(), condition.IDAgency);
            return model;
        }

        #endregion Search

        #region Approve
        public async Task<ServiceResult> ApproveArchiveProfile(int id)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var profile = await _dasRepo.PlanProfile.GetAsync(id);

            //Duyệt
            if (profile.Status != (int)EnumProfilePlan.Status.WaitArchiveApproved)
                return new ServiceResultError("Hồ sơ đã được nhận nộp lưu!");

            profile.Status = (int)EnumProfilePlan.Status.ArchiveApproved;
            profile.ApprovedDate = DateTime.Now;
            profile.ApprovedBy = _userPrincipalService.UserId;

            await CloneCatalogingData(profile);
            await _dasRepo.PlanProfile.UpdateAsync(profile);
            await _dasRepo.SaveAync();

            ////get users by permission
            //var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            //if (dict.IsEmpty())
            //    return new ServiceResultSuccess("Nhận nộp lưu hồ sơ thành công!");

            //List<int> userIds = new List<int>();
            //List<UserPermissionModel> existeds = new List<UserPermissionModel>();
            //foreach (var item in dict)
            //{
            //    var per = dict.GetValueOrDefault(item.Key);
            //    if (per.IsEmpty())
            //        continue;
            //    existeds = per.Where(p => p.CodeModule == (int)EnumModule.Code.C3060 && p.Type == (int)EnumPermission.Type.Approve).ToList();
            //    if (existeds.IsEmpty())
            //        continue;
            //    userIds.Add(item.Key);
            //}
            //if (userIds.IsEmpty())
            //    return new ServiceResultSuccess("Nhận nộp lưu hồ sơ thành công!");

            //string organName = string.Empty;
            //var organ = await _dasRepo.Organ.GetAsync(userData.IDOrgan);
            //if (organ.IsNotEmpty())
            //    organName = organ.Name;

            //string content = string.Format("{0} {1} duyệt hồ sơ nộp lưu {2}", organName, _userPrincipalService.UserName, profile.Title);

            ////insert notify to db
            //List<Notification> listNoti = new List<Notification>();
            //foreach (var userId in userIds)
            //{
            //    listNoti.Add(new Notification
            //    {
            //        UserId = userId,
            //        Content = content,
            //        IsRead = false,
            //        CreatedDate = DateTime.Now
            //    });
            //}
            //if (listNoti.IsEmpty())
            //    return new ServiceResultSuccess("Nhận nộp lưu hồ sơ thành công!");

            //await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            //await _dasNotifyRepo.SaveAync();

            //foreach (var userId in userIds)
            //{
            //    //send notify to userIds
            //    await _hubNotificationHelper.PushToUser(userId);
            //}

            return new ServiceResultSuccess("Nhận nộp lưu hồ sơ thành công!");
        }


        public async Task<ServiceResult> RejectArchiveProfile(int id, string reason = "")
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var profile = await _dasRepo.PlanProfile.GetAsync(id);
            //Từ chới
            if (profile.Status != (int)EnumProfilePlan.Status.WaitArchiveApproved)
                return new ServiceResultError("Hồ sơ đã bị từ chối nhận!");

            profile.Status = (int)EnumProfilePlan.Status.ArchiveReject;
            profile.ReasonToReject = reason;
            profile.ApprovedDate = DateTime.Now;
            profile.ApprovedBy = _userPrincipalService.UserId;
            await _dasRepo.PlanProfile.UpdateAsync(profile);
            await _dasRepo.SaveAync();

            ////get users by permission
            //var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            //if (dict.IsEmpty())
            //    return new ServiceResultSuccess("Đã từ chối nhận hồ sơ!");

            //List<int> userIds = new List<int>();
            //List<UserPermissionModel> existeds = new List<UserPermissionModel>();
            //foreach (var item in dict)
            //{
            //    var per = dict.GetValueOrDefault(item.Key);
            //    if (per.IsEmpty())
            //        continue;
            //    existeds = per.Where(p => p.CodeModule == (int)EnumModule.Code.C3060 && p.Type == (int)EnumPermission.Type.Approve).ToList();
            //    if (existeds.IsEmpty())
            //        continue;
            //    userIds.Add(item.Key);
            //}
            //if (userIds.IsEmpty())
            //    return new ServiceResultSuccess("Đã từ chối nhận hồ sơ!");

            //string organName = string.Empty;
            //var organ = await _dasRepo.Organ.GetAsync(userData.IDOrgan);
            //if (organ.IsNotEmpty())
            //    organName = organ.Name;
            //string content = string.Format("{0} {1} từ chối duyệt hồ sơ nộp lưu {2}", organName, _userPrincipalService.UserName, profile.Title);

            ////insert notify to db
            //List<Notification> listNoti = new List<Notification>();
            //foreach (var userId in userIds)
            //{
            //    listNoti.Add(new Notification
            //    {
            //        UserId = userId,
            //        Content = content,
            //        IsRead = false,
            //        CreatedDate = DateTime.Now
            //    });
            //}
            //if (listNoti.IsEmpty())
            //    return new ServiceResultSuccess("Đã từ chối nhận hồ sơ!");

            //await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            //await _dasNotifyRepo.SaveAync();

            //foreach (var userId in userIds)
            //{
            //    //send notify to userIds
            //    await _hubNotificationHelper.PushToUser(userId);
            //}

            return new ServiceResultSuccess("Đã từ chối nhận hồ sơ!");
        }
        public async Task<ServiceResult> ApproveArchiveProfiles(int[] ids)
        {
            if (ids.IsEmpty())
                return new ServiceResultError("Vui lòng chọn hồ sơ cần nhận nộp lưu!");

            var profiles = await _dasRepo.PlanProfile.GetAllListAsync(n => ids.Contains(n.ID));

            if (profiles.Any(n => n.Status != (int)EnumProfilePlan.Status.WaitArchiveApproved))
            {
                var names = new List<string>();
                foreach (var item in profiles)
                {
                    if (item.Status != (int)EnumProfilePlan.Status.WaitArchiveApproved)
                        names.Add(item.Title);
                }
                return new ServiceResultError($"Hồ sơ {string.Join(", ", names)} không được phép nhận nộp lưu!");
            }
            foreach (var profile in profiles)
            {
                profile.Status = (int)EnumProfilePlan.Status.ArchiveApproved;
                profile.ApprovedDate = DateTime.Now;
                profile.ApprovedBy = _userPrincipalService.UserId;
            }
            await _dasRepo.PlanProfile.UpdateAsync(profiles);
            await _dasRepo.SaveAync();

            //Tạo CatalogingProfile
            await CloneCatalogingDatas(profiles);
            //  //Tạo CatalogingDoc

            return new ServiceResultSuccess("Nhận nộp lưu hồ sơ thành công!");
        }


        public async Task<ServiceResult> RejectArchiveProfiles(int[] ids, string reason = "")
        {
            if (ids.IsEmpty())
                return new ServiceResultError("Vui lòng chọn hồ sơ cần từ chối nhận nộp lưu!");
            var profiles = await _dasRepo.PlanProfile.GetAllListAsync(n => ids.Contains(n.ID));

            if (profiles.Any(n => n.Status != (int)EnumProfilePlan.Status.WaitArchiveApproved))
            {
                var names = new List<string>();
                foreach (var item in profiles)
                {
                    if (item.Status != (int)EnumProfilePlan.Status.WaitArchiveApproved)
                        names.Add(item.Title);
                }
                return new ServiceResultError($"Hồ sơ {string.Join(", ", names)} không được phép nhận nộp lưu!");
            }
            foreach (var profile in profiles)
            {
                profile.Status = (int)EnumProfilePlan.Status.ArchiveReject;
                profile.ReasonToReject = reason;
                profile.ApprovedDate = DateTime.Now;
                profile.ApprovedBy = _userPrincipalService.UserId;
            }
            await _dasRepo.PlanProfile.UpdateAsync(profiles);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Từ chối nhận nộp lưu hồ sơ thành công!");
        }
        #endregion Approve

        #endregion ReceiveArchive

        #region Private method      

        private async Task<Dictionary<int, string>> GetDictAgencies(int parentId = -1)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Agency.GetAllListAsync(n =>
            n.Status == (int)EnumAgency.Status.Active && n.IDOrgan == userData.IDOrgan
            && (parentId < 0 || n.ParentId == parentId)
            )).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        /// <summary>
        /// Get hồ sơ trong kế hoạch
        /// </summary>
        /// <param name="idAgency">-1 get all</param>
        /// <param name="pageIndex">-1 get all</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMPlanProfile>> GetPlanProfiles(PlanProfileCondition condition, bool isExport)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       where pp.Status != (int)EnumProfilePlan.Status.InActive
                       && pp.IDOrgan == userData.IDOrgan
                       && (pp.IDPlan == condition.IDPlan)
                       && (condition.IDAgency <= 0 || condition.IDAgency == pp.IDAgency)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword))
                       orderby pp.ID descending
                       select _mapper.Map<VMPlanProfile>(pp);

            var profile = new List<VMPlanProfile>();
            if (isExport)
            {
                var dictAgencies = await GetDictAgencies();
                var dictExpiryDates = await GetDictExpiryDate();
                profile = await temp.ToListAsync();
                foreach (var item in profile)
                {
                    item.ProfileTime = Utils.DateToString(item.StartDate) + " - " + Utils.DateToString(item.EndDate);
                    item.MaintenanceAndPageNumber = item.Maintenance + "/" + item.PageNumber;
                    item.ExpiryDateName = dictExpiryDates.GetValueOrDefault(item.IDExpiryDate);
                    item.AgencyName = dictAgencies.GetValueOrDefault(item.IDAgency);
                }
                return new PaginatedList<VMPlanProfile>(profile, profile.Count(), 1, profile.Count());
            }
            else
            {
                if (condition.PageIndex == -1)
                {
                    //Nopaging
                    profile = await temp.ToListAsync();
                    return new PaginatedList<VMPlanProfile>(profile, profile.Count, 1, profile.Count);
                }

                var total = await temp.LongCountAsync();
                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                profile = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                return new PaginatedList<VMPlanProfile>(profile, (int)total, condition.PageIndex, condition.PageSize);
            }
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }

        private async Task GetUpdateModel(VMUpdatePlanProfile model)
        {
            model.DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString());
            model.DictProfileTemplate = await GetDictProfileTemplate(model.IDProfileTemplate.GetValueOrDefault(0));
            model.DictStorage = await GetDictCategory(EnumCategoryType.Code.DM_Kho.ToString());
            model.DictLangugage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString());
            model.DictExpiryDate = await GetDictExpiryDate();
            model.DictBox = await GetDictCategory(EnumCategoryType.Code.DM_HopSo.ToString());
            model.DictSecurityLevel = (await _dasRepo.SercureLevel.GetAllListAsync(u => u.Status == (int)EnumSercureLevel.Status.Active)).ToDictionary(n => n.ID, n => n.Name);
            model.DictAgencies = await GetDictAgencies(0);
        }

        public async Task<IEnumerable<Plan>> GetComboboxPlan()
        {
            if (_userPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //get idPlans by idAgency
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (pp.Status == (int)EnumProfilePlan.Status.WaitApprove || pp.Status == (int)EnumProfilePlan.Status.ArchiveReject)
                       && conditionStr
                       select pp.IDPlan;
            var idPlans = await temp.ToListAsync();
            if (!IsExisted(idPlans))
                return null;

            return await _dasRepo.Plan.GetAllListAsync(x => x.Status == (int)EnumPlan.Status.Approved
                && idPlans.Contains(x.ID));
        }

        private async Task<Dictionary<int, string>> GetDictUser()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
            if(userData ==null)
                return  new Dictionary<int, string>();
            var temp = from u in _dasRepo.User.GetAll()
                       where u.IDOrgan == userData.IDOrgan && u.Status == (int)EnumCommon.Status.Active
                       orderby u.Name
                       select new VMUser
                       {
                           ID = u.ID,
                           Name = u.Name
                       };
            var rs = await temp.ToListAsync();
            return rs.ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictCategory(string codeType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var cates = (await _dasRepo.Category.GetAllListAsync(n => codeType.IsNotEmpty() && n.CodeType == codeType && n.Status == (int)EnumCategory.Status.Active && n.IDOrgan == userData.IDOrgan));

            if (cates.Any(n => n.ParentId > 0))
            {
                //Render tree
                var treeModels = Utils.RenderTree(cates.Select(n => new TreeModel<VMPosition>
                {
                    ID = n.ID,
                    Name = n.Name,
                    Parent = n.ParentId ?? 0,
                    ParentPath = n.ParentPath ?? "0",
                }).ToList(), null, "--");
                return treeModels.ToDictionary(n => (int)n.ID, n => n.Name);
            }
            return cates.ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictProfileTemplate(int idProfileTemplate)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a =>
            a.Status == (int)EnumOrgan.Status.Active
           && a.IDOrgan == userData.IDOrgan
            && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
            )).ToDictionary(n => n.ID, n => n.FondName);
        }

        private void GetPlanDates(VMCreatePlan vmPlan, Plan plan, out List<object> errObj)
        {
            var createdAt = Utils.GetDate(vmPlan.CreatedAt);
            var fromDate = Utils.GetDate(vmPlan.FromDate);
            var endDate = Utils.GetDate(vmPlan.EndDate);

            errObj = new List<object>();

            if (createdAt.HasValue)
            {
                plan.CreatedAt = createdAt.Value;
            }
            else
            {
                errObj.Add(new
                {
                    Field = "CreatedAt",
                    Mss = "Ngày tạo không được để trống"
                });
            }
            if (fromDate.HasValue)
            {
                plan.FromDate = fromDate.Value;
            }
            if (endDate.HasValue)
            {
                plan.EndDate = endDate.Value;
            }

            if (fromDate.HasValue && endDate.HasValue)
            {
                if (plan.FromDate > plan.EndDate)
                {
                    errObj.Add(new
                    {
                        Field = $"FromDate",
                        Mss = "Thu thập từ ngày không được lớn hơn Đến ngày"
                    });
                }
            }
        }
        private void GetPlanProfileDates(VMUpdatePlanProfile vmProfile, PlanProfile profile, out List<object> errObj)
        {
            var startDate = Utils.GetDate(vmProfile.StartDate);
            var endDate = Utils.GetDate(vmProfile.EndDate);
            errObj = new List<object>();

            if (startDate.HasValue)
            {
                profile.StartDate = startDate.Value;
            }
            if (endDate.HasValue)
            {
                profile.EndDate = endDate.Value;
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                if (profile.StartDate > profile.EndDate)
                {
                    errObj.Add(new
                    {
                        Field = $"StartDate",
                        Mss = "Thời gian bắt đầu không được lớn hơn Thời gian kết thúc"
                    });
                }
            }
        }

        public async Task<IEnumerable<VMPlan>> GetActive()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.Plan.GetAll().Where(n => n.Status == (int)EnumPlan.Status.Active && n.IDOrgan == userData.IDOrgan)
                       select _mapper.Map<VMPlan>(p);
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMPlanProfile>> GetPlanProfileByListID(List<string> lstID, VMDeliveryRecord model = null)
        {
            if (model == null || model.IDPlan == 0 || model.IDAgency == 0)
                return null;

            var lstIDPlanProfile = lstID.Select(int.Parse).ToList();
            var temp = await _dasRepo.PlanProfile.GetAllListAsync(n => n.IDAgency == model.IDAgency && n.IDPlan == model.IDPlan && n.Status == (int)EnumProfilePlan.Status.ArchiveApproved);

            //Lấy các biên bản đã được lập
            var delivery = await _dasRepo.DeliveryRecord.GetAllListAsync(x => x.ID != model.ID && x.IDPlan == model.IDPlan && x.IDAgency == model.IDAgency && x.Status == (int)EnumDeliveryRecord.Status.Active && string.IsNullOrWhiteSpace(x.lstDeliveryPlanProfile));
            if (!IsExisted(delivery))
                return _mapper.Map<List<VMPlanProfile>>(temp.ToList());

            var lstPP = new List<int>();
            foreach (var item in delivery)
            {
                var lstIDPP = JsonConvert.DeserializeObject<List<int>>(item.lstDeliveryPlanProfile);
                lstPP.AddRange(lstIDPP);
            }

            var result = temp.Where(x => !lstPP.Contains(x.ID));
            return _mapper.Map<IEnumerable<VMPlanProfile>>(result);
        }

        public async Task<IEnumerable<VMPlan>> GetApprove()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.Plan.GetAll().Where(n => n.Status == (int)EnumPlan.Status.Approved && n.IDOrgan == userData.IDOrgan)
                       select _mapper.Map<VMPlan>(p);
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMPlan>> GetPlanForDelivery()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var planProfile = await _dasRepo.PlanProfile.GetAllListAsync(x => x.Status == (int)EnumProfilePlan.Status.ArchiveApproved);
            if (!IsExisted(planProfile))
                return null;
            var delivery = await _dasRepo.DeliveryRecord.GetAllListAsync(x => x.Status == (int)EnumDeliveryRecord.Status.Active && !string.IsNullOrEmpty(x.lstDeliveryPlanProfile));
            var lstIDPP = new List<int>();
            foreach (var item in delivery)
            {
                if (!item.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                    lstIDPP.Add(int.Parse(item.lstDeliveryPlanProfile));
                else
                    lstIDPP.AddRange(JsonConvert.DeserializeObject<List<int>>(item.lstDeliveryPlanProfile));
            }
            var lstIDPlan = planProfile.Where(x => !lstIDPP.Contains(x.ID)).ToList().Select(x => x.IDPlan).ToList();
            var temp = from p in _dasRepo.Plan.GetAll().Where(n => n.Status == (int)EnumPlan.Status.Approved && n.IDOrgan == _userPrincipalService.IDOrgan && lstIDPlan.Contains(n.ID))
                       select _mapper.Map<VMPlan>(p);
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMPlan>> GetPlanForEditDelivery()
        {
            var planProfile = await _dasRepo.PlanProfile.GetAllListAsync(x => x.Status == (int)EnumProfilePlan.Status.ArchiveApproved);
            if (!IsExisted(planProfile))
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var lstIDPlan = planProfile.Select(x => x.IDPlan).ToList();
            var temp = from p in _dasRepo.Plan.GetAll().Where(n => n.Status == (int)EnumPlan.Status.Approved && n.IDOrgan == userData.IDOrgan && lstIDPlan.Contains(n.ID))
                       select _mapper.Map<VMPlan>(p);
            return await temp.ToListAsync();
        }
        /// <summary>
        /// Tạo dữ liệu Hs, tài liệu sagn phần biên mục
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private async Task CloneCatalogingData(PlanProfile profile)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var catalogingProfile = Utils.Bind<CatalogingProfile>(profile.KeyValue());
            catalogingProfile.ID = 0;
            catalogingProfile.IDPlanProfile = profile.ID;
            catalogingProfile.Status = (int)EnumCataloging.Status.Active;
            catalogingProfile.ReasonToReject = null;
            catalogingProfile.IDOrgan = userData.IDOrgan;
            catalogingProfile.UpdatedBy = null;
            catalogingProfile.UpdatedDate = null;
            catalogingProfile.CreateDate = DateTime.Now;
            catalogingProfile.CreatedBy = _userPrincipalService.UserId;
            await _dasRepo.CatalogingProfile.InsertAsync(catalogingProfile);
            await _dasRepo.SaveAync();

            var docs = await _dasRepo.Doc.GetAllListAsync(n => n.IDProfile == profile.ID && n.Status != (int)EnumCommon.Status.InActive);
            if (docs.IsNotEmpty())
            {
                var docIds = docs.Select(n => n.ID);
                var docFields = await _dasRepo.DocField.GetAllListAsync(n => docIds.Contains(n.IDDoc));

                var catalogingDocs = docs.Select(n => new CatalogingDoc
                {
                    IDFile = n.IDFile,
                    IDDoc = n.ID,
                    IDCatalogingProfile = catalogingProfile.ID,
                    IDDocType = n.IDDocType,
                    Status = (int)EnumCommon.Status.Active
                }).ToList();

                await _dasRepo.CatalogingDoc.InsertAsync(catalogingDocs);
                await _dasRepo.SaveAync();

                if (docFields.IsNotEmpty())
                {
                    var catalogingDocFields = docFields.Select(df => new CatalogingDocField
                    {
                        IDCatalogingDoc = catalogingDocs.FirstOrNewObj(n => n.IDDoc == df.IDDoc).ID,
                        IDDocTypeField = df.IDDocTypeField,
                        Value = df.Value,
                        Status = (int)EnumCommon.Status.Active
                    }).ToList();
                    await _dasRepo.CatalogingDocField.InsertAsync(catalogingDocFields);
                    await _dasRepo.SaveAync();
                }
            }
        }

        private async Task CloneCatalogingDatas(IEnumerable<PlanProfile> profiles)
        {
            if (profiles.IsNotEmpty())
            {
                foreach (var profile in profiles)
                {
                    await CloneCatalogingData(profile);
                }
            }
        }

        /// <summary>
        /// Get so hs theo ke hoach
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<Dictionary<int, int>> GetTotalProfileInPlans(int[] ids)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var tempStatistic = (await (from p in _dasRepo.Plan.GetAll()
                                        join pp in _dasRepo.PlanProfile.GetAll() on p.ID equals pp.IDPlan
                                        where ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(p.ID))
                                        && pp.Status != (int)EnumProfilePlan.Status.InActive && pp.IDOrgan == userData.IDOrgan
                                        group p by p.ID into g
                                        select new
                                        {
                                            IDPlan = g.Key,
                                            TotalProfile = g.Count()
                                        }).ToListAsync()).ToDictionary(n => n.IDPlan, n => n.TotalProfile);
            return tempStatistic;
        }

        /// <summary>
        /// Get so hs theo ke hoach thu thập theo Agency
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<Dictionary<int, ProfileCount>> GetTotalProfileInPlansByAgency(int[] ids)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.Plan.GetAll()
                       join pp in _dasRepo.PlanProfile.GetAll() on p.ID equals pp.IDPlan
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(p.ID)))
                       && (pp.Status == (int)EnumProfilePlan.Status.Active
                       || pp.Status == (int)EnumProfilePlan.Status.Reject
                       || pp.Status == (int)EnumProfilePlan.Status.CollectComplete
                       || pp.Status == (int)EnumProfilePlan.Status.WaitApprove)
                       && conditionStr
                       group new { p, pp } by new { p.ID } into g
                       select new
                       {
                           IDplan = g.Key.ID,
                           Value = new ProfileCount
                           {
                               TotalCount = g.Count(),
                               HasReject = g.Sum(t => t.pp.Status == (int)EnumProfilePlan.Status.Reject ? 1 : 0) > 0
                           }

                       };
            return (await temp.ToListAsync()).ToDictionary(k => k.IDplan, v => v.Value);

        }

        private async Task<Dictionary<int, int>> GetTotalDocInProfiles(int[] ids)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       join d in _dasRepo.Doc.GetAll() on pp.ID equals d.IDProfile
                       where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(pp.ID)) && d.Status != (int)EnumCommon.Status.InActive
                        && pp.IDOrgan == userData.IDOrgan)
                       group new { pp, d } by new { pp.ID } into g
                       select new
                       {
                           IDProfile = g.Key.ID,
                           TotalDoc = g.Sum(x => (x.d.Status == (int)EnumDocCollect.Status.Active || x.d.Status == (int)EnumDocCollect.Status.Complete) ? 1 : 0)
                       };
            return (await temp.ToListAsync()).ToDictionary(k => k.IDProfile, v => v.TotalDoc);
        }

        /// <summary>
        /// Lấy số tài liệu theo hồ sơ và đơn vị
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="idAgency"></param>
        /// <returns></returns>
        private async Task<Dictionary<int, int>> GetTotalDocInProfiles(int[] ids, int idAgency = 0)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var tempStatistic = (await (from pp in _dasRepo.PlanProfile.GetAll()
                                        join d in _dasRepo.Doc.GetAll() on pp.ID equals d.IDProfile
                                        where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(pp.ID)) && (d.Status != (int)EnumDocCollect.Status.Active || d.Status != (int)EnumDocCollect.Status.Complete))
                                        && (idAgency == 0 || (idAgency > 0 && idAgency == pp.IDAgency)) && pp.IDOrgan == userData.IDOrgan
                                        group new { pp, d } by new { pp.ID } into g
                                        select new
                                        {
                                            IDProfile = g.Key.ID,
                                            TotalDoc = g.Sum(x => (x.d.Status == (int)EnumDocCollect.Status.Active || x.d.Status == (int)EnumDocCollect.Status.Complete) ? 1 : 0)
                                        }).ToListAsync()).ToDictionary(n => n.IDProfile, n => n.TotalDoc);
            return tempStatistic;
        }

        #endregion Private method
    }
}
