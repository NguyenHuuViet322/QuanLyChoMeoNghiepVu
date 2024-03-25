using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Utility;
using System;
using System.Threading.Tasks;
using ESD.Domain.Enums;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Presentation;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using ESD.Utility.LogUtils;
using System.Collections;
using Newtonsoft.Json;
using ESD.Domain.Interfaces.DASNotify;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;
using ESD.Application.Constants;
using ESD.Domain.Models.DASNotify;
using ESD.Infrastructure.Notifications;

namespace ESD.Application.Services 
{
    public class ArchiveManagementService : IArchiveManagementService
    {
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IDasNotifyRepositoryWrapper _dasNotifyRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _iUserPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;
        private readonly IHubNotificationHelper _hubNotificationHelper;
        public ArchiveManagementService(IDasRepositoryWrapper dasRepository
            , IDasNotifyRepositoryWrapper dasNotifyRepo
            , IMapper mapper
            , IUserPrincipalService iUserPrincipalService
            , ILoggerManager logger
            , IPermissionService permissionService
            , ICacheManagementServices cacheManagementServices
            , IHubNotificationHelper hubNotificationHelper) 
        {
            _dasRepo = dasRepository;
            _dasNotifyRepo = dasNotifyRepo;
            _mapper = mapper;
            _iUserPrincipalService = iUserPrincipalService;
            _logger = logger;
            _cacheManagementServices = cacheManagementServices;
            _hubNotificationHelper = hubNotificationHelper;
        }

        public async Task<VMIndexPlanProfile> SearchByConditionPagging(ArchiveManagementCondition condition)
        {
            if (_iUserPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (pp.Status == (int)EnumProfilePlan.Status.WaitApprove || pp.Status == (int)EnumProfilePlan.Status.ArchiveReject || pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved)
                       && (condition.IDPlan <= 0 || pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileNotation.Contains(condition.Keyword))
                       && conditionStr
                       orderby pp.Status == (int)EnumProfilePlan.Status.WaitApprove descending
                       , pp.Status == (int)EnumProfilePlan.Status.ArchiveReject descending
                       , pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved descending
                       , pp.UpdatedDate ?? pp.CreateDate descending
                       select _mapper.Map<VMPlanProfile>(pp);
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            VMIndexPlanProfile result = new VMIndexPlanProfile();
            var list = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            result.VMPlanProfiles = new PaginatedList<VMPlanProfile>(list, (int)total, condition.PageIndex, condition.PageSize);
            var ids = result.VMPlanProfiles.Select(n => n.ID).ToArray();
            result.TotalDocs = await GetTotalDocInProfiles(ids);
            var tempDate = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                           select new VMExpiryDate
                           {
                               ID = r.ID,
                               IDChannel = r.IDChannel,
                               Code = r.Code,
                               Name = r.Name,
                               Status = r.Status,
                               Description = r.Description
                           };
            var expiryDates = await tempDate.ToListAsync();
            if (IsExisted(expiryDates))
                result.DictExpiryDate = (await tempDate.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
            result.DictProfileTemplate = await GetDictProfileTemplate(0);
            result.DictAgency = await GetDictAgencies();
            return result;
        }

        public async Task<VMIndexPlanProfile> SearchListApprovedByConditionPagging(ArchiveManagementCondition condition)
        {
            if (_iUserPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved
                       && (condition.IDPlan <= 0 || pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileNotation.Contains(condition.Keyword))
                       && conditionStr
                       orderby pp.UpdatedDate ?? pp.CreateDate descending
                       select _mapper.Map<VMPlanProfile>(pp);
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            VMIndexPlanProfile result = new VMIndexPlanProfile();
            var list = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            result.VMPlanProfiles = new PaginatedList<VMPlanProfile>(list, (int)total, condition.PageIndex, condition.PageSize);
            var ids = result.VMPlanProfiles.Select(n => n.ID).ToArray();
            result.TotalDocs = await GetTotalDocInProfiles(ids);
            var tempDate = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                           select new VMExpiryDate
                           {
                               ID = r.ID,
                               IDChannel = r.IDChannel,
                               Code = r.Code,
                               Name = r.Name,
                               Status = r.Status,
                               Description = r.Description
                           };
            var expiryDates = await tempDate.ToListAsync();
            if (IsExisted(expiryDates))
                result.DictExpiryDate = (await tempDate.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
            result.DictProfileTemplate = await GetDictProfileTemplate(0);
            result.DictAgency = await GetDictAgencies();
            return result;
        }

        public async Task<VMIndexPlanProfile> SearchByCondition(ArchiveManagementCondition condition)
        {
            if (_iUserPrincipalService == null || _iUserPrincipalService.IDAgency == 0)
                return null;

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       where (pp.Status == (int)EnumProfilePlan.Status.WaitApprove || pp.Status == (int)EnumProfilePlan.Status.ArchiveReject || pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved)
                       && (condition.IDPlan <= 0 || pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileNotation.Contains(condition.Keyword))
                       join a in _dasRepo.Agency.GetAll() on pp.IDAgency equals a.ID
                       where (a.ID == _iUserPrincipalService.IDAgency || ("|" + a.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       orderby pp.Status == (int)EnumProfilePlan.Status.WaitApprove descending
                       , pp.Status == (int)EnumProfilePlan.Status.ArchiveReject descending
                       , pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved descending
                       , pp.UpdatedDate ?? pp.CreateDate descending
                       select _mapper.Map<VMPlanProfile>(pp);
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            VMIndexPlanProfile result = new VMIndexPlanProfile
            {
                VMListPlanProfiles = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync()
            };
            var ids = result.VMListPlanProfiles.Select(n => n.ID).ToArray();
            result.TotalDocs = await GetTotalDocInProfiles(ids);
            var tempDate = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                           select new VMExpiryDate
                           {
                               ID = r.ID,
                               IDChannel = r.IDChannel,
                               Code = r.Code,
                               Name = r.Name,
                               Status = r.Status,
                               Description = r.Description
                           };
            var expiryDates = await tempDate.ToListAsync();
            if (IsExisted(expiryDates))
                result.DictExpiryDate = (await tempDate.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
            result.DictProfileTemplate = await GetDictProfileTemplate(0);
            result.DictAgency = await GetDictAgencies();
            return result;
        }

        public async Task<VMIndexPlanProfile> SearchListApprovedByCondition(ArchiveManagementCondition condition)
        {
            if (_iUserPrincipalService == null || _iUserPrincipalService.IDAgency == 0)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved
                       && (condition.IDPlan <= 0 || pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileNotation.Contains(condition.Keyword))
                       && conditionStr
                       orderby pp.UpdatedDate ?? pp.CreateDate descending
                       select _mapper.Map<VMPlanProfile>(pp);
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            VMIndexPlanProfile result = new VMIndexPlanProfile
            {
                VMListPlanProfiles = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync()
            };
            var ids = result.VMListPlanProfiles.Select(n => n.ID).ToArray();
            result.TotalDocs = await GetTotalDocInProfiles(ids);
            var tempDate = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                           select new VMExpiryDate
                           {
                               ID = r.ID,
                               IDChannel = r.IDChannel,
                               Code = r.Code,
                               Name = r.Name,
                               Status = r.Status,
                               Description = r.Description
                           };
            var expiryDates = await tempDate.ToListAsync();
            if (IsExisted(expiryDates))
                result.DictExpiryDate = (await tempDate.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
            result.DictProfileTemplate = await GetDictProfileTemplate(0);
            result.DictAgency = await GetDictAgencies();
            return result;
        }

        #region Get
        /// <summary>
        /// Lấy Index cho màn hình các thành phần hồ sơ
        /// Lấy Plan, hồ sơ cho Breadcrumb
        /// Lấy list Doc
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<VMIndexDocPlan> PlanDocDetailIndex(PlanDocCondition condition, bool isExport = false)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocs(condition, isExport);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter -= listUnvalid.Count();
            }
            return new VMIndexDocPlan
            {
                VMPlanProfile = await GetPlanProfile(condition.IDProfile),
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }

        public async Task<VMIndexDocPlan> PlanDocDetailIndexNoPaging(PlanDocCondition condition)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocsNoPaging(condition);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter -= listUnvalid.Count();
            }
            return new VMIndexDocPlan
            {
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }

        public async Task<VMIndexDocPlan> PlanDocDetailIndexListApproved(PlanDocCondition condition)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocsListApproved(condition);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter -= listUnvalid.Count();
            }
            return new VMIndexDocPlan
            {
                VMPlanProfile = await GetPlanProfileListApproved(condition.IDProfile),
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }

        public async Task<VMIndexDocPlan> PlanDocDetailIndexListApprovedNoPaging(PlanDocCondition condition)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocsListApprovedNoPaging(condition);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter -= listUnvalid.Count();
            }
            return new VMIndexDocPlan
            {
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }

        public async Task<VMDocCreate> GetDocCollect(int IDDoc)
        {
            var temp = from doc in _dasRepo.Doc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                       && doc.ID == IDDoc
                       select _mapper.Map<VMDocCreate>(doc);
            var model = await temp.FirstOrDefaultAsync();
            model.VMDocTypes = await GetDocTypes();
            model.VMPlanProfile = await GetPlanProfile(model.IDProfile);
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMDocFields = await GetDocFieldsByID(IDDoc);
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile));

            return model;

        }

        public async Task<IEnumerable<Plan>> GetComboboxPlan()
        {
            if (_iUserPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //get idPlans by idAgency
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       where (pp.Status == (int)EnumProfilePlan.Status.WaitApprove || pp.Status == (int)EnumProfilePlan.Status.ArchiveReject || pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved)
                       && userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : pp.IDAgency == userData.IDAgency
                       select pp.IDPlan;
            var idPlans = await temp.ToListAsync();
            if (!IsExisted(idPlans))
                return null;

            return await _dasRepo.Plan.GetAllListAsync(x => x.Status == (int)EnumPlan.Status.Approved
                && idPlans.Contains(x.ID));
        }

        public async Task<IEnumerable<Plan>> GetComboboxPlanListApproved()
        {
            if (_iUserPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //get idPlans by idAgency
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       where pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved
                       && userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : pp.IDAgency == userData.IDAgency
                       select pp.IDPlan;
            var idPlans = await temp.ToListAsync();
            if (!IsExisted(idPlans))
                return null;

            return await _dasRepo.Plan.GetAllListAsync(x => x.Status == (int)EnumPlan.Status.Approved
                && idPlans.Contains(x.ID));
        }
        #endregion Get

        #region Status
        public async Task<ServiceResult> ApprovePlanProfile(int id)
        {
            try
            {
                //get planprofile by id
                var planProfile = await _dasRepo.PlanProfile.GetAsync(id);
                if (!IsExisted(planProfile))
                    return new ServiceResultError("Hồ sơ không tồn tại!");

                //check status profile
                if (planProfile.Status != (int)EnumProfilePlan.Status.WaitApprove && planProfile.Status != (int)EnumProfilePlan.Status.ArchiveReject)
                    return new ServiceResultError("Có lỗi xảy ra, vui lòng tải lại trang!");

                //update status profile
                planProfile.Status = (int)EnumProfilePlan.Status.WaitArchiveApproved;

                //update this profile
                await _dasRepo.PlanProfile.UpdateAsync(planProfile);
                await _dasRepo.SaveAync();

                ////get users by permission
                //var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
                //if (dict.IsEmpty())
                //    return new ServiceResultSuccess("Duyệt hồ sơ thành công!");

                //List<int> userIds = new List<int>();
                //List<int> otherUserIds = new List<int>();
                //List<UserPermissionModel> existeds = new List<UserPermissionModel>();
                //List<UserPermissionModel> otherExisteds = new List<UserPermissionModel>();
                //foreach (var item in dict)
                //{
                //    var per = dict.GetValueOrDefault(item.Key);
                //    if (per.IsEmpty())
                //        continue;
                //    existeds = per.Where(p => p.CodeModule == (int)EnumModule.Code.C3040 && p.Type == (int)EnumPermission.Type.Approve).ToList();
                //    if (existeds.IsEmpty())
                //        continue;
                //    userIds.Add(item.Key);
                //}
                //foreach (var item in dict)
                //{
                //    var per = dict.GetValueOrDefault(item.Key);
                //    if (per.IsEmpty())
                //        continue;
                //    otherExisteds = per.Where(p => p.CodeModule == (int)EnumModule.Code.C3070 && p.Type == (int)EnumPermission.Type.Approve).ToList();
                //    if (otherExisteds.IsEmpty())
                //        continue;
                //    otherUserIds.Add(item.Key);
                //}
                //if (userIds.IsEmpty() && otherUserIds.IsEmpty())
                //    return new ServiceResultSuccess("Duyệt hồ sơ thành công!");

                //string content = string.Format("{0} duyệt hồ sơ thu thập {1}", _iUserPrincipalService.UserName, planProfile.Title);
                //string otherContent = string.Format("{0} gửi yêu cầu phê duyệt hồ sơ nộp lưu {1}", _iUserPrincipalService.UserName, planProfile.Title);

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

                //List<Notification> listOtherNoti = new List<Notification>();
                //foreach (var userId in otherUserIds)
                //{
                //    listOtherNoti.Add(new Notification
                //    {
                //        UserId = userId,
                //        Content = content,
                //        IsRead = false,
                //        CreatedDate = DateTime.Now
                //    });
                //}

                //await _dasNotifyRepo.Notification.InsertAsync(listNoti);
                //await _dasNotifyRepo.Notification.InsertAsync(listOtherNoti);
                //await _dasNotifyRepo.SaveAync();

                //foreach (var userId in userIds)
                //{
                //    //send notify to userIds
                //    await _hubNotificationHelper.PushToUser(userId);
                //}

                //foreach (var userId in otherUserIds)
                //{
                //    //send notify to userIds
                //    await _hubNotificationHelper.PushToUser(userId);
                //}

                return new ServiceResultSuccess("Duyệt hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> RejectPlanProfile(int id, string note)
        {
            try
            {
                //get planprofile by id
                var planProfile = await _dasRepo.PlanProfile.GetAsync(id);
                if (!IsExisted(planProfile))
                    return new ServiceResultError("Hồ sơ không tồn tại!");

                //check status profile
                if (planProfile.Status != (int)EnumProfilePlan.Status.WaitApprove && planProfile.Status != (int)EnumProfilePlan.Status.ArchiveReject)
                    return new ServiceResultError("Có lỗi xảy ra, vui lòng tải lại trang!");

                //update status profile
                planProfile.Status = (int)EnumProfilePlan.Status.Reject;
                planProfile.ReasonToReject = note;
                planProfile.ApprovedBy = _iUserPrincipalService.UserId;
                planProfile.ApprovedDate = DateTime.UtcNow;

                //update this profile
                await _dasRepo.PlanProfile.UpdateAsync(planProfile);
                await _dasRepo.SaveAync();

                ////get users by permission
                //var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
                //if (dict.IsEmpty())
                //    return new ServiceResultSuccess("Từ chối hồ sơ thành công!");

                //List<int> userIds = new List<int>();
                //List<UserPermissionModel> existeds = new List<UserPermissionModel>();
                //foreach (var item in dict)
                //{
                //    var per = dict.GetValueOrDefault(item.Key);
                //    if (per.IsEmpty())
                //        continue;
                //    existeds = per.Where(p => p.CodeModule == (int)EnumModule.Code.C3040 && p.Type == (int)EnumPermission.Type.Create).ToList();
                //    if (existeds.IsEmpty())
                //        continue;
                //    userIds.Add(item.Key);
                //}
                //if (userIds.IsEmpty())
                //    return new ServiceResultSuccess("Từ chối hồ sơ thành công!");

                //string content = string.Format("{0} từ chối duyệt kế hoạch thu thập {1}", _iUserPrincipalService.UserName, planProfile.Title);
                //string otherContent = string.Format("{0} từ chối duyệt kế hoạch thu thập {1}", _iUserPrincipalService.UserName, planProfile.Title);

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
                //    return new ServiceResultSuccess("Từ chối hồ sơ thành công!");
                //await _dasNotifyRepo.Notification.InsertAsync(listNoti);
                //await _dasNotifyRepo.SaveAync();

                //foreach (var userId in userIds)
                //{
                //    //send notify to userIds
                //    await _hubNotificationHelper.PushToUser(userId);
                //}

                return new ServiceResultSuccess("Từ chối hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Status 

        #region Private method
        private async Task<VMUpdatePlanProfile> UpdatePlanProfile(int id)
        {
            var profile = await _dasRepo.PlanProfile.GetAsync(id) ?? new PlanProfile();
            var model = Utils.Bind<VMUpdatePlanProfile>(profile.KeyValue());
            model.StartDate = Utils.DateToString(profile.StartDate);
            model.EndDate = Utils.DateToString(profile.EndDate);
            model.VMPlan = _mapper.Map<VMPlan>(await _dasRepo.Plan.GetAsync(profile.IDPlan) ?? new Plan());
            await GetUpdateModel(model);
            return model;
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
            model.DictAgencies = await GetDictAgencies();
            if (string.IsNullOrEmpty(model.Language) || model.Language == "null")
                model.Language = string.Empty;
            else
            {
                if (model.Language.Contains("["))
                {
                    var language = JsonConvert.DeserializeObject<List<string>>(model.Language);
                    model.Language = string.Empty;
                    if (IsExisted(language))
                    {
                        foreach (var item in language)
                        {
                            model.Language += model.DictLangugage.GetValueOrDefault(int.Parse(item)) + ", ";
                        }
                        model.Language = model.Language[0..^2];
                    }
                }
                else
                    model.Language = model.DictLangugage.GetValueOrDefault(int.Parse(model.Language));
            }
            model.DictUsers = await GetDictUsers();
        }

        private async Task<Dictionary<int, string>> GetDictUsers()
        {
            return (await _dasRepo.User.GetAllListAsync(n =>
            n.Status == (int)EnumCommon.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictAgencies()
        {
            return (await _dasRepo.Agency.GetAllListAsync(n =>
            n.Status == (int)EnumAgency.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictCategory(string codeType)
        {
            var cates = (await _dasRepo.Category.GetAllListAsync(n => codeType.IsNotEmpty() && n.CodeType == codeType && n.Status == (int)EnumCategory.Status.Active && n.IDOrgan == _iUserPrincipalService.IDOrgan));

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
            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a =>
            a.Status == (int)EnumOrgan.Status.Active
            //&& a.IDOrgan == _iUserPrincipalService.IDOrgan
            && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
            )).ToDictionary(n => n.ID, n => n.FondName);
        }

        /// <summary>
        /// Lấy các tài liệu trong 1 hồ sơ
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocs(PlanDocCondition condition, bool isExport = false)
        {
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.DocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.Doc.GetAll()
                       join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDDoc
                       where (d.Status == (int)EnumDocCollect.Status.Active || d.Status == (int)EnumDocCollect.Status.Complete)
                       && (d.IDProfile == condition.IDProfile)
                       && (p.IDAgency == condition.IDAgency || condition.IDAgency == 0)
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       &&
                       (p.Status == (int)EnumProfilePlan.Status.WaitApprove
                       || p.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved
                       || p.Status == (int)EnumProfilePlan.Status.ArchiveReject)
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            if (isExport)
            {
                var rs = await temp.ToListAsync();
                return new PaginatedList<VMPlanDoc>(rs, rs.Count(), 1, rs.Count());
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, (int)total, condition.PageIndex, condition.PageSize);
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocsNoPaging(PlanDocCondition condition)
        {
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var idAgency = condition.IDAgency > 0 ? condition.IDAgency : _iUserPrincipalService.IDAgency; //NamNp: thêm đk tìm theo đv, default dv theo ng dùng
            var tempFiled = from df in _dasRepo.DocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.Doc.GetAll()
                       join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDDoc
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && d.IDProfile == condition.IDProfile && p.IDAgency == idAgency
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       && (p.Status == (int)EnumProfilePlan.Status.WaitApprove || p.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved || p.Status == (int)EnumProfilePlan.Status.ArchiveReject)
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            var docs = await temp.ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, 0, condition.PageIndex, condition.PageSize);
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocsListApproved(PlanDocCondition condition, bool isExport = false)
        {
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.DocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.Doc.GetAll()
                       join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDDoc
                       where (d.Status == (int)EnumDocCollect.Status.Active || d.Status == (int)EnumDocCollect.Status.Complete)
                       && (d.IDProfile == condition.IDProfile)
                       && (p.IDAgency == condition.IDAgency || condition.IDAgency == 0)
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       && p.Status == (int)EnumProfilePlan.Status.ArchiveApproved
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            if (isExport)
            {
                var rs = await temp.ToListAsync();
                return new PaginatedList<VMPlanDoc>(rs, rs.Count(), 1, rs.Count());
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, (int)total, condition.PageIndex, condition.PageSize);
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocsListApprovedNoPaging(PlanDocCondition condition)
        {
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var tempFiled = from df in _dasRepo.DocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.Doc.GetAll()
                       join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDDoc
                       let conditionStr = userData.HasOrganPermission ? p.IDOrgan == userData.IDOrgan : (p.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + p.IDAgency + "|"))
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && d.IDProfile == condition.IDProfile && conditionStr
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       && p.Status == (int)EnumProfilePlan.Status.ArchiveApproved && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            var docs = await temp.ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, 0, condition.PageIndex, condition.PageSize);
        }

        /// <summary>
        /// Lấy Hồ sơ và kế hoạch của list thành phần trong hồ sơ
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<VMPlanProfile> GetPlanProfile(int idProfile)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       join p in _dasRepo.Plan.GetAll() on pp.IDPlan equals p.ID
                       where 
                       (pp.Status == (int)EnumProfilePlan.Status.WaitApprove 
                       || pp.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved 
                       || pp.Status == (int)EnumProfilePlan.Status.ArchiveReject) 
                       && userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       && p.Status == (int)EnumPlan.Status.Approved && pp.ID == idProfile
                       select new VMPlanProfile
                       {
                           ID = pp.ID,
                           IDChannel = pp.IDChannel,
                           IDPlan = pp.IDPlan,
                           FileCode = pp.FileCode,
                           IDStorage = pp.IDStorage,
                           IDCodeBox = pp.IDCodeBox,
                           IDProfileList = pp.IDProfileList,
                           IDSecurityLevel = pp.IDSecurityLevel,
                           Identifier = pp.Identifier,
                           IDProfileTemplate = pp.IDProfileTemplate,
                           FileCatalog = pp.FileCatalog,
                           FileNotation = pp.FileNotation,
                           Title = pp.Title,
                           IDExpiryDate = pp.IDExpiryDate,
                           Rights = pp.Rights,
                           Language = pp.Language,
                           StartDate = pp.StartDate,
                           EndDate = pp.EndDate,
                           TotalDoc = pp.TotalDoc,
                           Description = pp.Description,
                           InforSign = pp.InforSign,
                           Keyword = pp.Keyword,
                           Maintenance = pp.Maintenance,
                           PageNumber = pp.PageNumber,
                           Format = pp.Format,
                           Status = pp.Status,
                           IDAgency = pp.IDAgency,
                           CreateDate = pp.CreateDate,
                           PlanName = p.Name,
                           ReasonToReject = pp.ReasonToReject
                       };

            return await temp.FirstOrDefaultAsync();
        }

        private async Task<VMPlanProfile> GetPlanProfileListApproved(int idProfile)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       join p in _dasRepo.Plan.GetAll() on pp.IDPlan equals p.ID
                       where pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved
                       && userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       && p.Status == (int)EnumPlan.Status.Approved && pp.ID == idProfile
                       select new VMPlanProfile
                       {
                           ID = pp.ID,
                           IDChannel = pp.IDChannel,
                           IDPlan = pp.IDPlan,
                           FileCode = pp.FileCode,
                           IDStorage = pp.IDStorage,
                           IDCodeBox = pp.IDCodeBox,
                           IDProfileList = pp.IDProfileList,
                           IDSecurityLevel = pp.IDSecurityLevel,
                           Identifier = pp.Identifier,
                           IDProfileTemplate = pp.IDProfileTemplate,
                           FileCatalog = pp.FileCatalog,
                           FileNotation = pp.FileNotation,
                           Title = pp.Title,
                           IDExpiryDate = pp.IDExpiryDate,
                           Rights = pp.Rights,
                           Language = pp.Language,
                           StartDate = pp.StartDate,
                           EndDate = pp.EndDate,
                           TotalDoc = pp.TotalDoc,
                           Description = pp.Description,
                           InforSign = pp.InforSign,
                           Keyword = pp.Keyword,
                           Maintenance = pp.Maintenance,
                           PageNumber = pp.PageNumber,
                           Format = pp.Format,
                           Status = pp.Status,
                           IDAgency = pp.IDAgency,
                           CreateDate = pp.CreateDate,
                           PlanName = p.Name,
                           ReasonToReject = pp.ReasonToReject
                       };

            return await temp.FirstOrDefaultAsync();
        }

        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        /// <summary>
        /// Lấy list các DocType cho combobox
        /// </summary>
        /// <returns></returns>
        private async Task<List<VMDocType>> GetDocTypes()
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }

        /// <summary>
        /// Lấy list các define khung biên mục
        /// </summary>
        /// <param name="idDoctype"></param>
        /// <returns></returns>
        private async Task<List<VMDocTypeField>> GetDocTypeFields(int idDoctype)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && dtf.IDDocType == idDoctype
                       orderby dtf.Priority
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }

        /// <summary>
        /// Tạo các DocFiled theo list Type Fileds
        /// </summary>
        /// <param name="docTypeFields"></param>
        /// <returns></returns>
        private async Task<List<VMDocField>> GetDocFieldsDefault(List<VMDocTypeField> docTypeFields, VMPlanProfile vMPlanProfile, VMDocType vMDocType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var idOrgan = userData.IDOrgan;
            var organName = (await _dasRepo.Organ.GetAsync(idOrgan)).Name;
            var vmDocFields = new List<VMDocField>();
            if (docTypeFields.IsEmpty())
                return vmDocFields;
            bool isType1 = vMDocType.Type == (int)EnumDocType.Type.Doc;
            foreach (var field in docTypeFields)
            {
                int val = 0;
                string text;
                switch (field.InputType)
                {
                    case (int)EnumDocType.InputType.InpNumber:
                    case (int)EnumDocType.InputType.InpFloat:
                        text = val.ToString();
                        break;
                    case (int)EnumDocType.InputType.InpText:
                        text = string.Empty;
                        break;
                    case (int)EnumDocType.InputType.InpDate:
                        text = Utils.DateToString(DateTime.Now, field.Format ?? "dd-MM-yyyy");
                        break;
                    default:
                        text = val.ToString();
                        break;
                }
                //Giá trị mặc định văn bản theo hồ sơ
                if (isType1)
                {
                    switch (field.Code)
                    {
                        case "FileCode":
                            text = vMPlanProfile.FileCode ?? vMPlanProfile.FileCode;
                            break;
                        case "Identifier":
                            text = vMPlanProfile.Identifier ?? vMPlanProfile.Identifier;
                            break;
                        case "Organld":
                            text = vMPlanProfile.IDProfileTemplate.ToString();
                            break;
                        case "FileCatalog":
                            text = vMPlanProfile.FileCatalog.ToString();
                            break;
                        case "FileNotation":
                            text = vMPlanProfile.FileNotation ?? vMPlanProfile.FileNotation;
                            break;
                        case "OrganName":
                            text = organName;
                            break;
                        default:
                            break;
                    }
                }
                vmDocFields.Add(new VMDocField
                {
                    IDDoc = 0,
                    IDDocTypeField = field.ID,
                    Value = text
                });

            }
            return vmDocFields;

        }

        private void BindDocField(DocField docField, VMDocTypeField field, Hashtable data)
        {
            var fieldName = "Field" + field.ID;
            docField.Value = Utils.GetString(data, fieldName);
        }

        private async Task<List<VMDocField>> GetDocFieldsByID(int IDDoc)
        {
            var temp = from df in _dasRepo.DocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && df.IDDoc == IDDoc
                       select _mapper.Map<VMDocField>(df);
            return await temp.ToListAsync();
        }

        private async Task<List<VMDocTypeField>> GetDocTypeFields(IEnumerable<int> idDoctypes)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && idDoctypes.Contains(dtf.IDDocType)
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }

        private async Task<List<VMDocField>> GetDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.DocField.GetAll()
                       where /*df.Status != (int)EnumCommon.Status.InActive*/
                       //&& 
                       IDDocs.Contains(df.IDDoc)
                       select _mapper.Map<VMDocField>(df);
            return await temp.ToListAsync();
        }

        private async Task<List<VMDocType>> GetDocTypes(IEnumerable<int> IDDocTypes)
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       && IDDocTypes.Contains(dc.ID)
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }

        private async Task<Dictionary<int, int>> GetTotalDocInProfiles(int[] ids)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       join d in _dasRepo.Doc.GetAll() on pp.ID equals d.IDProfile
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(pp.ID)) && d.Status != (int)EnumCommon.Status.InActive)
                       && conditionStr
                       group new { pp, d } by new { pp.ID } into g
                       select new
                       {
                           IDProfile = g.Key.ID,
                           TotalDoc = g.Sum(x => (x.d.Status == (int)EnumDocCollect.Status.Active || x.d.Status == (int)EnumDocCollect.Status.Complete) ? 1 : 0)
                       };
            return (await temp.ToListAsync()).ToDictionary(k => k.IDProfile, v => v.TotalDoc);

        }

        private bool IsExisted(PlanProfile planProfile)
        {
            if (planProfile == null || planProfile.ID == 0 || planProfile.Status == (int)EnumProfilePlan.Status.InActive)
                return false;
            return true;
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }
        #endregion Private method
    }
}
