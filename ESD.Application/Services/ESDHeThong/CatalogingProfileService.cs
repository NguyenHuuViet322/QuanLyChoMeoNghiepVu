using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OneAPI;
using OneAPI.iOneSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class CatalogingProfileService : ICatalogingProfileService
    {
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IStgFileClientService _fileClientService;
        protected ICacheManagementServices _cacheManagementServices;
        protected string ioneSvAddress = ConfigUtils.GetKeyValue("IOneConfigs", "Address") ?? "localhost";

        public CatalogingProfileService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService iUserPrincipalService
            , ILoggerManager logger
            , IStgFileClientService fileClientService
            , ICacheManagementServices cacheManagementServices)
        {
            _dasRepo = dasRepository;
            _mapper = mapper;
            _userPrincipalService = iUserPrincipalService;
            _logger = logger;
            _fileClientService = fileClientService;
            _cacheManagementServices = cacheManagementServices;
        }

        public async Task<VMIndexPlanProfile> SearchByConditionPagging(ArchiveManagementCondition condition)
        {
            if (_userPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.CatalogingProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (pp.Status == (int)EnumCataloging.Status.Active || pp.Status == (int)EnumCataloging.Status.WaitApprove || pp.Status == (int)EnumCataloging.Status.StorageReject)
                       && (condition.IDPlan <= 0 || pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileCode.Contains(condition.Keyword))
                       && conditionStr
                       select _mapper.Map<VMPlanProfile>(pp);
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var model = new VMIndexPlanProfile();
            var list = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMPlanProfiles = new PaginatedList<VMPlanProfile>(list, (int)total, condition.PageIndex, condition.PageSize);

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
                model.DictExpiryDate = (await tempDate.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
            model.DictProfileTemplate = await GetDictProfileTemplate(0);
            model.DictAgency = await GetDictAgencies();
            model.DictProfileType = Utils.EnumToDic<EnumProfile.Type>();
            return model;
        }

        public async Task<VMIndexPlanProfile> SearchListApprovedByConditionPagging(ArchiveManagementCondition condition)
        {
            if (_userPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where pp.Status == (int)EnumCataloging.Status.Approved
                       && (condition.IDPlan <= 0 || pp.IDPlan == condition.IDPlan)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileCode.Contains(condition.Keyword))
                       && conditionStr
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
            return result;
        }
        public async Task<PaginatedList<VMApproveStorage>> SearchListWaitApproveByConditionPagging(ApproveStorageCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from cp in _dasRepo.CatalogingProfile.GetAll()
                       join sl in _dasRepo.SercureLevel.GetAll() on cp.IDSecurityLevel equals sl.ID into joinedSercureLevel
                       from jsl in joinedSercureLevel.DefaultIfEmpty()
                       join ed in _dasRepo.ExpiryDate.GetAll() on cp.IDExpiryDate equals ed.ID into joinedExpiryDate
                       from jed in joinedExpiryDate.DefaultIfEmpty()
                       where cp.Status == (int)EnumCataloging.Status.WaitApprove
                       && cp.IDOrgan == userData.IDOrgan
                       && (condition.Keyword.IsEmpty() || cp.Title.Contains(condition.Keyword)
                       || cp.FileCode.Contains(condition.Keyword)
                       || cp.FileNotation.Contains(condition.Keyword))
                       && (condition.IDExpiryDate == 0 || condition.IDExpiryDate == jed.ID)
                       select new VMApproveStorage
                       {
                           ID = cp.ID,
                           Title = cp.Title,
                           SecurityLevel = jsl.Name,
                           ExpiryDate = jed.Name,
                           IDSecurityLevel = jsl.ID,
                           IDExpiryDate = jed.ID,
                           FileNotation = cp.FileNotation,
                           ProfileNote = cp.Description,
                           NumberPaperAndPage = cp.Maintenance.ToString() + "/" + cp.PageNumber.ToString(),
                           DocumentStartDate = cp.StartDate,
                           DocumentEndDate = cp.EndDate,
                       };
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return new PaginatedList<VMApproveStorage>();

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            if (!IsExisted(result))
                return new PaginatedList<VMApproveStorage>();
            var ids = new int[1];
            foreach (var item in result)
            {
                ids[0] = item.ID;
                var strStartDate = "";
                var strEndDate = "";
                if (item.DocumentStartDate.IsNotEmpty())
                    strStartDate = item.DocumentStartDate.GetValueOrDefault().Day.ToString() + "/" + item.DocumentStartDate.GetValueOrDefault().Month.ToString() + "/" + item.DocumentStartDate.GetValueOrDefault().Year.ToString();
                if (item.DocumentEndDate.IsNotEmpty())
                    strEndDate = item.DocumentEndDate.GetValueOrDefault().Day.ToString() + "/" + item.DocumentEndDate.GetValueOrDefault().Month.ToString() + "/" + item.DocumentEndDate.GetValueOrDefault().Year.ToString();

                item.DocumentTime = strStartDate + " - " + strEndDate;
                item.TotalProfiles = await GetTotalDocInProfiles(ids);
            }

            PaginatedList<VMApproveStorage> model = new PaginatedList<VMApproveStorage>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        #region Status
        public async Task<ServiceResult> ApproveCatalogingProfile(int id)
        {
            try
            {
                //get catalogingProfile by id
                var catalogingProfile = await _dasRepo.CatalogingProfile.GetAsync(id);
                if (!IsExisted(catalogingProfile))
                    return new ServiceResultError("Hồ sơ không tồn tại!");

                //check status catalogingProfile
                if (catalogingProfile.Status != (int)EnumCataloging.Status.WaitApprove)
                    return new ServiceResultError("Có lỗi xảy ra, vui lòng tải lại trang!");

                //update status catalogingProfile
                catalogingProfile.Status = (int)EnumCataloging.Status.Approved;

                //update this catalogingProfile
                await _dasRepo.CatalogingProfile.UpdateAsync(catalogingProfile);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Duyệt hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> RejectCatalogingProfile(int id, string note)
        {
            try
            {
                //get catalogingProfile by id
                var catalogingProfile = await _dasRepo.CatalogingProfile.GetAsync(id);
                if (!IsExisted(catalogingProfile))
                    return new ServiceResultError("Hồ sơ không tồn tại!");

                //check status catalogingProfile
                if (catalogingProfile.Status != (int)EnumCataloging.Status.WaitApprove || catalogingProfile.Status != (int)EnumCataloging.Status.StorageReject)
                    return new ServiceResultError("Có lỗi xảy ra, vui lòng tải lại trang!");

                //update status catalogingProfile
                catalogingProfile.Status = (int)EnumCataloging.Status.Reject;
                catalogingProfile.ReasonToReject = note;

                //update this catalogingProfile
                await _dasRepo.CatalogingProfile.UpdateAsync(catalogingProfile);
                await _dasRepo.SaveAync();
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
        private async Task<Dictionary<int, string>> GetDictProfileTemplate(int idProfileTemplate)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a => a.Status == (int)EnumOrgan.Status.Active
            && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
                       && a.IDOrgan == userData.IDOrgan)).ToDictionary(n => n.ID, n => n.FondName);
        }

        private async Task<Dictionary<int, string>> GetDictAgencies(int parentId = -1)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Agency.GetAllListAsync(n =>
            n.Status == (int)EnumAgency.Status.Active && n.IDOrgan == userData.IDOrgan
            && (parentId < 0 || n.ParentId == parentId)
            )).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictUsers()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.User.GetAllListAsync(n => n.IDOrgan == userData.IDOrgan
            )).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        private bool IsExisted(CatalogingProfile catalogingProfile)
        {
            if (catalogingProfile == null || catalogingProfile.ID == 0 || catalogingProfile.Status == (int)EnumCataloging.Status.InActive)
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


        #region DocumentCataloging

        /// <summary>
        /// index & Search
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<VMIndexCatalogingProfile> DocumentCatalogingSearch(CatalogingProfileCondition condition)
        {

            var model = new VMIndexCatalogingProfile
            {
                DictExpiryDate = await GetDictExpiryDate(),
                DictProfileType = Utils.EnumToDic<EnumProfile.Type>(),
                DictStatus = new Dictionary<int, string> {
                    { (int)EnumCataloging.Status.Active, StringUltils.GetEnumDescription(EnumCataloging.Status.Active)},
                    { (int)EnumCataloging.Status.WaitApprove, StringUltils.GetEnumDescription(EnumCataloging.Status.WaitApprove)},
                    { (int)EnumCataloging.Status.StorageReject, StringUltils.GetEnumDescription(EnumCataloging.Status.Reject)}, //lấy lable của Reject
                },
                Condition = condition
            };

            if (_userPrincipalService == null)
                return model;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.CatalogingProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (pp.Status == (int)EnumCataloging.Status.Active || pp.Status == (int)EnumCataloging.Status.WaitApprove || pp.Status == (int)EnumCataloging.Status.StorageReject)
                       && (condition.IDStatus <= 0 || pp.Status == condition.IDStatus)
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileCode.Contains(condition.Keyword))
                       && conditionStr
                       orderby pp.Status == (int)EnumCataloging.Status.Active descending
                    //, pp.Status == (int)EnumCataloging.Status.Reject descending
                    , pp.Status == (int)EnumCataloging.Status.WaitApprove descending
                    , pp.Status == (int)EnumCataloging.Status.StorageReject descending
                    , pp.Status == (int)EnumCataloging.Status.Approved descending
                    , pp.UpdatedDate ?? pp.CreateDate descending
                       select _mapper.Map<VMCatalogingProfile>(pp);
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return model;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var list = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMCatalogingProfiles = new PaginatedList<VMCatalogingProfile>(list, (int)total, condition.PageIndex, condition.PageSize);
            model.TotalDocs = await GetTotalDocInProfiles(list.Select(n => n.ID).ToArray());
            return model;
        }

        /// <summary>
        /// Send approve
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="approveBy"></param>
        /// <returns></returns>
        public async Task<ServiceResult> SendApprove(int[] ids, int approveBy)
        {
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(n => ids.Contains(n.ID));
            if (profiles.Any(n => n.Status != (int)EnumCataloging.Status.Active && n.Status != (int)EnumCataloging.Status.StorageReject))
            {
                var names = new List<string>();
                foreach (var item in profiles)
                {
                    if (item.Status != (int)EnumCataloging.Status.Active && item.Status != (int)EnumCataloging.Status.StorageReject)
                        names.Add(item.Title);
                }
                return new ServiceResultError($"Hồ sơ {string.Join(", ", names)} không được phép trình duyệt!");
            }

            var dictProfileDOcCount = await GetTotalCatalogingDocProfiles(ids);
            foreach (var item in profiles)
            {
                var profileDOcCount = dictProfileDOcCount?.FirstOrDefault(n => n.ID == item.ID);

                if (Utils.IsEmpty(profileDOcCount) || profileDOcCount.TotalDoc == 0)
                {
                    return new ServiceResultError($"Hồ sơ {item.Title} không có tài liệu, không được phép trình duyệt");
                }
                if (profileDOcCount.TotalCatalogingDoc > 0)
                {
                    return new ServiceResultError($"Hồ sơ {item.Title} có {profileDOcCount.TotalCatalogingDoc} tài liệu chưa hoàn thành biên muc");
                }
                item.ApprovedBy = approveBy;
                item.Status = (int)EnumCataloging.Status.WaitApprove;
            }
            await _dasRepo.CatalogingProfile.UpdateAsync(profiles);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Chuyển lãnh đạo phê duyệt thành công!");
        }

        #region Create
        public async Task<VMUpdateCatalogingProfile> CreateCatalogingProfile()
        {
            var model = new VMUpdateCatalogingProfile()
            {
                //IDPlan = id
            };
            await GetUpdateModel(model);
            return model;
        }

        public async Task<ServiceResult> CreateCatalogingProfile(VMUpdateCatalogingProfile vmProfile)
        {
            try
            {
                var data = vmProfile.KeyValue();
                var profile = Utils.Bind<CatalogingProfile>(data);
                GetCatalogingProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var language = Utils.GetStrings(data, nameof(VMUpdateCatalogingProfile.Language));
                profile.Language = language.IsNotEmpty() ? Utils.Serialize(language) : string.Empty;

                profile.Status = (int)EnumProfile.Status.Active;
                if (await _dasRepo.CatalogingProfile.IsCodeExist(profile.FileCode, (int)EnumProfile.Status.Active))
                {
                    return new ServiceResultError("Mã hồ sơ đã tồn tại");
                }
                var font = await _dasRepo.ProfileTemplate.GetAsync(profile.IDProfileTemplate) ?? new ProfileTemplate();
                if (font.Type == (int)EnumProfileTemplate.Type.Close)
                {
                    return new ServiceResultError("Phông đang chọn đã bị đóng, vui lòng chọn phông khác");
                }
                profile.ID = 0;
                profile.IDOrgan = userData.IDOrgan;
                profile.Type = (int)EnumProfile.Type.Digital;

                await _dasRepo.CatalogingProfile.InsertAsync(profile);
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
        public async Task<VMUpdateCatalogingProfile> UpdateCatalogingProfile(int? id)
        {
            var profile = await _dasRepo.CatalogingProfile.GetAsync(id.Value) ?? new CatalogingProfile();
            var model = Utils.Bind<VMUpdateCatalogingProfile>(profile.KeyValue());
            model.StartDate = Utils.DateToString(profile.StartDate);
            model.EndDate = Utils.DateToString(profile.EndDate);
            model.VMPlan = _mapper.Map<VMPlan>(await _dasRepo.Plan.GetAsync(profile.IDPlan) ?? new Plan());
            await GetUpdateModel(model);
            return model;
        }

        public async Task<ServiceResult> UpdateCatalogingProfile(VMUpdateCatalogingProfile vmProfile)
        {
            try
            {
                //1. Update Profile
                var profile = await _dasRepo.CatalogingProfile.GetAsync(vmProfile.ID);
                if (profile == null)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");
                var data = vmProfile.KeyValue();
                profile.Bind(data);
                GetCatalogingProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);

                if (await _dasRepo.CatalogingProfile.IsCodeExist(profile.FileCode, (int)EnumProfile.Status.Active, vmProfile.ID))
                {
                    return new ServiceResultError("Mã hồ sơ đã tồn tại");
                }
                var font = await _dasRepo.ProfileTemplate.GetAsync(profile.IDProfileTemplate) ?? new ProfileTemplate();
                if (font.Type == (int)EnumProfileTemplate.Type.Close)
                {
                    return new ServiceResultError("Phông đang chọn đã bị đóng, vui lòng chọn phông khác");
                }

                var language = Utils.GetStrings(data, nameof(VMUpdateCatalogingProfile.Language));
                profile.Language = language.IsNotEmpty() ? Utils.Serialize(language) : string.Empty;

                await _dasRepo.CatalogingProfile.UpdateAsync(profile);
                await UpdateDocWhenProfileChange(profile);
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
        public async Task<ServiceResult> DeleteCatalogingProfile(int id)
        {
            try
            {
                //Logic delete
                var positionDelete = await _dasRepo.CatalogingProfile.GetAsync(id);
                if (positionDelete == null || positionDelete.Status == (int)EnumCataloging.Status.InActive)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");

                if (positionDelete.Status != (int)EnumCataloging.Status.Reject && positionDelete.Status != (int)EnumCataloging.Status.Active && positionDelete.Status != (int)EnumCataloging.Status.StorageReject)
                    return new ServiceResultError("Hồ sơ này không được phép xóa");

                positionDelete.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.CatalogingProfile.UpdateAsync(positionDelete);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa hồ sơ thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteCatalogingProfiles(IEnumerable<int> ids)
        {
            try
            {
                //Logic delete
                var positionDeletes = await _dasRepo.CatalogingProfile.GetAllListAsync(n => ids.Contains(n.ID));
                if (positionDeletes == null || positionDeletes.Count() == 0)
                    return new ServiceResultError("Hồ sơ đã chọn hiện không tồn tại hoặc đã bị xóa");

                if (positionDeletes.Any(n => n.Status != (int)EnumCataloging.Status.Active && n.Status != (int)EnumCataloging.Status.Reject && n.Status != (int)EnumCataloging.Status.StorageReject))
                {
                    var names = new List<string>();
                    foreach (var item in positionDeletes)
                    {
                        if (item.Status != (int)EnumCataloging.Status.Active && item.Status != (int)EnumCataloging.Status.StorageReject && item.Status != (int)EnumCataloging.Status.Reject)
                            names.Add(item.Title);
                    }
                    return new ServiceResultError($"Hồ sơ {string.Join(", ", names)} không được phép xoá!");
                }

                foreach (var pos in positionDeletes)
                {
                    pos.Status = (int)EnumCommon.Status.InActive;

                }
                await _dasRepo.CatalogingProfile.UpdateAsync(positionDeletes);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa hồ sơ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete

        #region GetCatalingDOcs
        public async Task<VMIndexCatalogingDoc> CatalogingDocIndex(CatalogingDocCondition condition, bool isExport = false)
        {
            //lấy list Doc
            var pagDoc = await GetCatalogingDocs(condition, isExport);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var catalogingDocField = await GetCatalogingDocFieldsByIDs(pagDoc.Select(x => x.ID));
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                //Dictionary
                doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
            }
            var profile = await GetCatalogingProfile(condition.IDProfile);
            return new VMIndexCatalogingDoc
            {
                VMCatalogingProfile = profile,
                VMCatalogingDocs = pagDoc,
                DictUsers = await GetDictUsers(),
                DictExpiryDate = await GetDictExpiryDate(),
                DictAgencies = await GetDictAgencies(),
                DictProfileTemplate = await GetDictProfileTemplate(profile.IDProfileTemplate),
                DictLanguage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString()),
                DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString()),
                CatalogingDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }
        public async Task<VMCatalogingDocCreate> GetDocCollect(int IDDoc)
        {
            var temp = from doc in _dasRepo.CatalogingDoc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                       && doc.ID == IDDoc
                       select _mapper.Map<VMCatalogingDocCreate>(doc);
            var model = await temp.FirstOrDefaultAsync();
            model.VMDocTypes = await GetDocTypes();
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile));
            model.VMCatalogingProfile = await GetCatalogingProfile(model.IDCatalogingProfile);
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMCatalogingDocFields = await GetDocFieldsByID(IDDoc);

            foreach (var item in model.VMCatalogingDocFields)
            {
                var field = model.VMDocTypeFields.FirstOrDefault(n => n.ID == item.IDDocTypeField);
                if (field == null)
                    continue;
                switch (field.Code)
                {
                    case "FileCode":
                    case "Identifier":
                    case "Organld":
                    case "FileCatalog":
                    case "FileNotation":
                    case "OrganName":
                        item.IsReadonly = true;
                        break;
                }
            }
            return model;
        }

        #region Create

        /// <summary>
        /// Get CreateCatalogingDoc
        /// </summary>
        /// <param name="IDProfile">Profile</param>
        /// <param name="IDDocType">IDDocType, default 0</param>
        /// <returns></returns>
        public async Task<VMCatalogingDocCreate> CreateCatalogingDoc(int IDProfile, int IDDocType = 0)
        {
            var type = IDDocType == 0 ? (int)EnumDocType.Type.Doc : 0; //Chưa có lấy theo type
            var model = new VMCatalogingDocCreate
            {
                IDCatalogingProfile = IDProfile,

                VMDocTypes = await GetDocTypes(),
                VMCatalogingProfile = await GetCatalogingProfile(IDProfile)
            };
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == IDDocType || n.Type == type);
            model.IDDocType = model.VMDocType.ID;
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMCatalogingDocFields = await GetDocFieldsDefault(model.VMDocTypeFields, model.VMCatalogingProfile, model.VMDocType);
            return model;
        }

        public async Task<ServiceResult> CreateCatalogingDoc(Hashtable data, bool isComplete)
        {
            var doc = Utils.Bind<CatalogingDoc>(data);

            doc.Status = isComplete ? (int)EnumDocCollect.Status.Complete : (int)EnumDocCollect.Status.Active;
            if (doc.ID > 0)
            {
                await _dasRepo.CatalogingDoc.UpdateAsync(doc);
            }
            else
            {
                await _dasRepo.CatalogingDoc.InsertAsync(doc);
            }
            await _dasRepo.SaveAync();
            if (doc.ID > 0)
            {
                await UpdateProfileTotalDoc(doc.IDCatalogingProfile);

                var docFields = new List<CatalogingDocField>();
                var docTypeFields = await (from dtf in _dasRepo.DocTypeField.GetAll()
                                           where dtf.IDDocType == doc.IDDocType
                                           orderby dtf.Priority
                                           select _mapper.Map<VMDocTypeField>(dtf)).ToListAsync();
                if (docTypeFields.IsNotEmpty())
                {
                    foreach (var field in docTypeFields)
                    {
                        var docField = new CatalogingDocField
                        {
                            IDCatalogingDoc = doc.ID,
                            IDDocTypeField = field.ID,
                            Status = (int)EnumDocCollect.Status.Active,
                            CreatedBy = doc.CreatedBy,
                            CreateDate = DateTime.Now,
                        };
                        BindCatalogingDocField(docField, field, data);
                        docFields.Add(docField);
                    }
                }
                if (docFields.IsNotEmpty())
                {
                    await _dasRepo.CatalogingDocField.InsertAsync(docFields);
                    await _dasRepo.SaveAync();

                }
            }
            return new ServiceResultSuccess("Thêm mới tài liệu thành công thành công", doc.ID);
        }
        #endregion Create

        #region Update
        public async Task<ServiceResult> UpdateCatalogingDoc(Hashtable data, bool isComplete)
        {
            var doc = Utils.Bind<CatalogingDoc>(data);
            if (isComplete)
            {
                doc.Status = (int)EnumDocCollect.Status.Complete;
            }
            else
            {
                doc.Status = (int)EnumDocCollect.Status.Active;
            }
            await _dasRepo.CatalogingDoc.UpdateAsync(doc);
            await _dasRepo.SaveAync();

            var inserts = new List<CatalogingDocField>();
            var updates = new List<CatalogingDocField>();
            var deletes = new List<CatalogingDocField>();

            var docTypeFields = await (from dtf in _dasRepo.DocTypeField.GetAll()
                                       where dtf.IDDocType == doc.IDDocType
                                       orderby dtf.Priority
                                       select _mapper.Map<VMDocTypeField>(dtf)).ToListAsync();
            var catalogingDocFields = (await (_dasRepo.CatalogingDocField.GetAllListAsync(x => x.IDCatalogingDoc == doc.ID)));
            if (catalogingDocFields.IsNotEmpty())
            {
                foreach (var field in docTypeFields)
                {
                    var catalogingDocField = catalogingDocFields.FirstOrDefault(n => n.IDDocTypeField == field.ID);
                    if (Utils.IsEmpty(catalogingDocField))
                    {
                        catalogingDocField = new CatalogingDocField
                        {
                            IDCatalogingDoc = doc.ID,
                            IDDocTypeField = field.ID,
                            Status = 1,
                            CreatedBy = doc.CreatedBy,
                            CreateDate = DateTime.Now,
                        };
                        inserts.Add(catalogingDocField);
                    }
                    else
                    {
                        catalogingDocField.UpdatedBy = doc.UpdatedBy;
                        catalogingDocField.UpdatedDate = DateTime.Now;
                        updates.Add(catalogingDocField);
                    }
                    BindCatalogingDocField(catalogingDocField, field, data);
                }
            }

            if (Utils.IsNotEmpty(catalogingDocFields))
            {
                var updateIDs = updates.Select(x => x.ID).ToArray();
                deletes = catalogingDocFields.Where(n => !updateIDs.Contains(n.ID)).ToList();
            }
            if (deletes.IsNotEmpty() || updates.IsNotEmpty() || inserts.IsNotEmpty())
            {
                if (deletes.IsNotEmpty())
                {
                    await _dasRepo.CatalogingDocField.DeleteAsync(deletes);
                }
                if (updates.IsNotEmpty())
                {
                    await _dasRepo.CatalogingDocField.UpdateAsync(updates);
                }
                if (inserts.IsNotEmpty())
                {
                    await _dasRepo.CatalogingDocField.InsertAsync(inserts);
                }
                await _dasRepo.SaveAync();
            }
            //update Profile status
            //if (isComplete)
            //{
            //    var profile = await _dasRepo.CatalogingProfile.GetAsync(doc.IDCatalogingProfile);
            //    var catalogingdocs = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == profile.ID && x.Status == (int)EnumDocCollect.Status.Active);
            //    if (catalogingdocs == null || catalogingdocs.Count() == 0)
            //    {
            //        profile.Status = (int)EnumProfilePlan.Status.CollectComplete;
            //        await _dasRepo.CatalogingProfile.UpdateAsync(profile);
            //        await _dasRepo.SaveAync();
            //    }
            //}
            return new ServiceResultSuccess("Cập nhật tài liệu thành công", doc.ID);
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteDoc(int id)
        {
            try
            {
                var docDelete = await _dasRepo.CatalogingDoc.GetAsync(id);
                if (docDelete == null || docDelete.Status == (int)EnumDocCollect.Status.InActive)
                {
                    return new ServiceResultError("Tài liệu này hiện không tồn tại hoặc đã bị xóa");
                }
                var profile = await _dasRepo.CatalogingProfile.GetAsync(docDelete.IDCatalogingProfile);
                if (profile == null || (profile.Status != (int)EnumProfilePlan.Status.CollectComplete && profile.Status != (int)EnumProfilePlan.Status.Active))
                {
                    return new ServiceResultError("Không thể xóa tài liệu trong hồ sơ này");
                }
                docDelete.Status = (int)EnumDocCollect.Status.InActive;
                await _dasRepo.CatalogingDoc.UpdateAsync(docDelete);
                var childs = await _dasRepo.CatalogingDocField.GetAllListAsync(x => x.IDCatalogingDoc == id);
                foreach (var child in childs)
                {
                    child.Status = (int)EnumDocCollect.Status.InActive;
                }

                await _dasRepo.CatalogingDocField.UpdateAsync(childs);
                await _dasRepo.SaveAync();

                await UpdateProfileTotalDoc(docDelete.IDCatalogingProfile);

                return new ServiceResultSuccess("Xóa tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteDocs(IEnumerable<int> ids)
        {
            try
            {
                var docDeletes = await _dasRepo.CatalogingDoc.GetAllListAsync(x => ids.Contains(x.ID));
                if (docDeletes == null || docDeletes.Count() == 0)
                {
                    return new ServiceResultError("Tài liệu này hiện không tồn tại hoặc đã bị xóa");
                }
                var idProfiles = docDeletes.Select(x => x.IDCatalogingProfile).ToList();

                var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(x => idProfiles.Contains(x.ID));
                var check = false;
                foreach (var item in profiles)
                {
                    if (item.Status != (int)EnumProfilePlan.Status.CollectComplete && item.Status != (int)EnumProfilePlan.Status.Active)
                    {
                        check = true;
                        break;
                    }
                }
                if (check)
                {
                    return new ServiceResultError("Không thể xóa tài liệu trong hồ sơ này");
                }
                foreach (var doc in docDeletes)
                {
                    doc.Status = (int)EnumDocCollect.Status.InActive;
                }

                await _dasRepo.CatalogingDoc.UpdateAsync(docDeletes);
                var childs = await _dasRepo.CatalogingDocField.GetAllListAsync(x => ids.Contains(x.IDCatalogingDoc));
                foreach (var child in childs)
                {
                    child.Status = (int)EnumDocCollect.Status.InActive;
                }

                await _dasRepo.CatalogingDocField.UpdateAsync(childs);
                await _dasRepo.SaveAync();

                await UpdateProfileTotalDoc(docDeletes.FirstOrNewObj().IDCatalogingProfile);

                return new ServiceResultSuccess("Xóa tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete

        #endregion


        #region ApproveCatalogingDocument


        public async Task<ServiceResult> ApproveCatalogingDocument(int id)
        {
            var ctgd = await _dasRepo.CatalogingProfile.GetAsync(id);
            if (!IsExisted(ctgd))
                return new ServiceResultError("Không tìm thấy tài liệu phù hợp");

            if (!(ctgd.Status == (int)EnumCataloging.Status.WaitApprove))
                return new ServiceResultError("Không tìm thấy tài liệu phù hợp");

            if (_userPrincipalService == null || _userPrincipalService.UserId == 0 || _userPrincipalService.UserId != ctgd.ApprovedBy)
                return new ServiceResultError("Bạn không có quyền thực hiện hành động này");

            ctgd.Status = (int)EnumCataloging.Status.StorageApproved;
            ctgd.ApprovedDate = DateTime.Now;
            ctgd.IsPublic = true;

            await _dasRepo.CatalogingProfile.UpdateAsync(ctgd);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Duyệt lưu kho thành công");
        }

        public async Task<ServiceResult> RejectCatalogingDocument(int id, string reasonToReject)
        {
            var ctgd = await _dasRepo.CatalogingProfile.GetAsync(id);
            if (!IsExisted(ctgd))
                return new ServiceResultError("Không tìm thấy tài liệu phù hợp");

            if (!(ctgd.Status == (int)EnumCataloging.Status.WaitApprove))
                return new ServiceResultError("Không tìm thấy tài liệu phù hợp");

            if (_userPrincipalService == null || _userPrincipalService.UserId == 0 || _userPrincipalService.UserId != ctgd.ApprovedBy)
                return new ServiceResultError("Bạn không có quyền thực hiện hành động này");

            ctgd.Status = (int)EnumCataloging.Status.StorageReject;
            ctgd.ReasonToReject = reasonToReject;
            await _dasRepo.CatalogingProfile.UpdateAsync(ctgd);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Từ chối duyệt thành công");
        }


        #endregion ApproveCatalogingDocument

        #region Funtions
        private async Task<List<VMCatalogingDocField>> GetDocFieldsByID(int IDDoc)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && df.IDCatalogingDoc == IDDoc
                       select _mapper.Map<VMCatalogingDocField>(df);
            return await temp.ToListAsync();
        }

        private async Task<List<VMCatalogingDocField>> GetDocFieldsDefault(List<VMDocTypeField> docTypeFields, VMCatalogingProfile vMPlanProfile, VMDocType vMDocType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var idOrgan = userData.IDOrgan;
            var organName = (await _dasRepo.Organ.GetAsync(idOrgan)).Name;
            var vmDocFields = new List<VMCatalogingDocField>();
            if (docTypeFields.IsEmpty())
                return vmDocFields;

            var val = 0;
            var text = string.Empty;
            bool isReadonly;
            bool isType1 = vMDocType.Type == (int)EnumDocType.Type.Doc;
            foreach (var field in docTypeFields)
            {
                isReadonly = false;
                val = 0;
                text = string.Empty;
                switch (field.InputType)
                {
                    case (int)EnumDocType.InputType.InpNumber:
                    case (int)EnumDocType.InputType.InpFloat:
                        text = val.ToString();
                        break;
                    case (int)EnumDocType.InputType.InpText:
                    case (int)EnumDocType.InputType.InpTextArea:
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
                            isReadonly = true;
                            text = vMPlanProfile.FileCode ?? vMPlanProfile.FileCode;
                            break;
                        case "Identifier":
                            isReadonly = true;
                            text = vMPlanProfile.Identifier ?? vMPlanProfile.Identifier;
                            break;
                        case "Organld":
                            isReadonly = true;
                            text = vMPlanProfile.IDProfileTemplate.ToString();
                            break;
                        case "FileCatalog":
                            isReadonly = true;
                            text = vMPlanProfile.FileCatalog.ToString();
                            break;
                        case "FileNotation":
                            isReadonly = true;
                            text = vMPlanProfile.FileNotation ?? vMPlanProfile.FileNotation;
                            break;
                        case "OrganName":
                            isReadonly = true;
                            text = organName;
                            break;
                        default:
                            break;
                    }
                }
                vmDocFields.Add(new VMCatalogingDocField
                {
                    IDCatalogingDoc = 0,
                    IDDocTypeField = field.ID,
                    Value = text,
                    IsReadonly = isReadonly
                });

            }
            return vmDocFields;
        }

        private async Task<List<VMDocTypeField>> GetDocTypeFields(int idDoctype)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && dtf.IDDocType == idDoctype
                       orderby dtf.Priority
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }

        private void BindCatalogingDocField(CatalogingDocField docField, VMDocTypeField field, Hashtable data)
        {
            var fieldName = "Field" + field.ID;
            docField.Value = Utils.GetString(data, fieldName);
        }

        private async Task<List<VMDocType>> GetDocTypes()
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       where dc.IDOrgan == _userPrincipalService.IDOrgan
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }

        private async Task GetUpdateModel(VMUpdateCatalogingProfile model)
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

        private void GetCatalogingProfileDates(VMUpdateCatalogingProfile vmProfile, CatalogingProfile profile, out List<object> errObj)
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
        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        public async Task<ServiceResult> ViewDetailCatalogingDocument(int id)
        {
            return new ServiceResultSuccess();
        }

        public async Task<ServiceResult> ApproveListCatalogingDocument(int[] ids)
        {
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(n => ids.Contains(n.ID));

            if (!IsExisted(profiles))
                return new ServiceResultError("Không tìm thấy tài liệu phù hợp");

            if (_userPrincipalService == null || _userPrincipalService.UserId == 0)
                return new ServiceResultError("Bạn không có quyền thực hiện hành động này");

            var names = new List<string>();
            if (profiles.Any(n => n.Status != (int)EnumCataloging.Status.WaitApprove))
            {
                foreach (var item in profiles)
                {
                    if (item.Status != (int)EnumCataloging.Status.WaitApprove)
                        names.Add(item.Title);
                }
                if (names.Count > 0)
                    return new ServiceResultError($"Hồ sơ {string.Join(", ", names)} không được phép duyệt!");
            }
            else
            {
                foreach (var item in profiles)
                {
                    if (_userPrincipalService.UserId != item.ApprovedBy)
                        names.Add(item.Title);
                }
                if (names.Count > 0)
                    return new ServiceResultError($"Bạn không có quyền duyệt hồ sơ {string.Join(", ", names)} !");
            }

            foreach (var item in profiles)
            {
                item.Status = (int)EnumCataloging.Status.StorageApproved;
                item.ApprovedDate = DateTime.Now;
                item.IsPublic = true;
            }
            await _dasRepo.CatalogingProfile.UpdateAsync(profiles);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Phê duyệt lưu kho thành công!");
        }

        /// <summary>
        /// Lấy các tài liệu trong 1 hồ sơ
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMCatalogingDoc>> GetCatalogingDocs(CatalogingDocCondition condition, bool isExport)
        {
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };
            string[] arrCodeType2 = { "Identifier" };
            string[] arrCodeType3 = { "Identifier" };
            var tempFiled = from df in _dasRepo.CatalogingDocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == 1 && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == 2 && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == 3 && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.CatalogingDoc.GetAll()
                       join p in _dasRepo.CatalogingProfile.GetAll() on d.IDCatalogingProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDCatalogingDoc
                       where (d.Status == (int)EnumDocCollect.Status.Active || d.Status == (int)EnumDocCollect.Status.Complete)
                     && (d.IDCatalogingProfile == condition.IDProfile)
                    && (p.IDAgency == condition.IDAgency || condition.IDAgency == 0)
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       &&
                       (p.Status == (int)EnumCataloging.Status.Active
                       || p.Status == (int)EnumCataloging.Status.CollectComplete
                       || p.Status == (int)EnumCataloging.Status.Reject
                       || p.Status == (int)EnumCataloging.Status.WaitApprove
                       || p.Status == (int)EnumCataloging.Status.WaitApprove
                       || p.Status == (int)EnumCataloging.Status.StorageReject
                       || p.Status == (int)EnumCataloging.Status.StorageApproved)
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDCatalogingProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby (g.Key.UpdatedDate ?? g.Key.CreateDate) descending
                       select new VMCatalogingDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDCatalogingProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };

            if (isExport)
            {
                var docs = await temp.ToListAsync();
                var vmDocs = _mapper.Map<List<VMCatalogingDoc>>(docs);
                return new PaginatedList<VMCatalogingDoc>(vmDocs, vmDocs.Count(), 1, vmDocs.Count());
            }
            else
            {
                var total = await temp.LongCountAsync();

                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                var vmDocs = _mapper.Map<List<VMCatalogingDoc>>(docs);
                return new PaginatedList<VMCatalogingDoc>(vmDocs, (int)total, condition.PageIndex, condition.PageSize);
            }
        }
        private async Task<List<VMDocType>> GetDocTypes(IEnumerable<int> IDDocTypes)
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       && IDDocTypes.Contains(dc.ID)
                       select _mapper.Map<VMDocType>(dc);

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
        private async Task<List<VMCatalogingDocField>> GetCatalogingDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && IDDocs.Contains(df.IDCatalogingDoc)
                       select _mapper.Map<VMCatalogingDocField>(df);
            return await temp.ToListAsync();
        }
        /// <summary>
        /// Lấy Hồ sơ và kế hoạch của list thành phần trong hồ sơ
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<VMCatalogingProfile> GetCatalogingProfile(int idProfile)
        {
            var pp = (await _dasRepo.CatalogingProfile.GetAsync(idProfile)) ?? new CatalogingProfile();
            var plan = (await _dasRepo.Plan.GetAsync(pp.IDPlan)) ?? new Plan();
            var rs = _mapper.Map<VMCatalogingProfile>(pp);
            rs.PlanName = plan.Name;
            return rs;
        }

        /// <summary>
        /// Lấy số tl đang bien mục của hồ sơ 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<IEnumerable<VMCatalogingProfile>> GetTotalCatalogingDocProfiles(int[] ids)
        {
            var tempStatistic = (await (from d in _dasRepo.CatalogingDoc.GetAll()
                                        where (ids.IsNotEmpty() && ids.Contains(d.IDCatalogingProfile))
                                        && (d.Status == (int)EnumDocCollect.Status.Active || d.Status == (int)EnumDocCollect.Status.Complete)
                                        select new
                                        {
                                            d.ID,
                                            d.IDCatalogingProfile,
                                            d.Status
                                        }).ToListAsync());

            if (tempStatistic.IsEmpty())
                return null;

            var catalogingProfileIds = tempStatistic.Select(n => n.IDCatalogingProfile).Distinct();

            return catalogingProfileIds.Select(idCatalogingProfile => new VMCatalogingProfile
            {
                ID = idCatalogingProfile,
                TotalDoc = tempStatistic.Count(x => x.IDCatalogingProfile == idCatalogingProfile),
                TotalCatalogingDoc = tempStatistic.Count(x => x.IDCatalogingProfile == idCatalogingProfile && x.Status == (int)EnumDocCollect.Status.Active)
            });
        }

        /// <summary>
        /// Lấy số tài liệu theo hồ sơ và đơn vj
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="idAgency"></param>
        /// <returns></returns>
        private async Task<Dictionary<int, int>> GetTotalDocInProfiles(int[] ids, int idAgency = 0)
        {
            var tempStatistic = (await (from pp in _dasRepo.CatalogingProfile.GetAll()
                                        join d in _dasRepo.CatalogingDoc.GetAll() on pp.ID equals d.IDCatalogingProfile
                                        where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(pp.ID)) && d.Status != (int)EnumCommon.Status.InActive)
                                        && (idAgency == 0 || (idAgency > 0 && idAgency == pp.IDAgency))
                                        group pp by pp.ID into g
                                        select new
                                        {
                                            IDProfile = g.Key,
                                            TotalDoc = g.Count()
                                        }).ToListAsync()).ToDictionary(n => n.IDProfile, n => n.TotalDoc);
            return tempStatistic;

        }
        private async Task<ServiceResult> UpdateDocWhenProfileChange(CatalogingProfile profile)
        {
            List<string> listFieldChange = new List<string> { "FileCode", "Identifier", "Organld", "FileCatalog", "FileNotation" };
            var temp = from d in _dasRepo.CatalogingDoc.GetAll()
                       from dt in _dasRepo.DocType.GetAll().Where(x => x.ID == d.IDDocType).DefaultIfEmpty()
                       from dtf in _dasRepo.DocTypeField.GetAll().Where(x => x.IDDocType == dt.ID).DefaultIfEmpty()
                       from df in _dasRepo.CatalogingDocField.GetAll().Where(x => x.IDCatalogingDoc == d.ID && x.IDDocTypeField == dtf.ID).DefaultIfEmpty()
                       where d.Status != (int)EnumDocCollect.Status.InActive
                       && d.IDCatalogingProfile == profile.ID
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
            await _dasRepo.CatalogingDocField.UpdateAsync(listDocField);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật tài liệu thành công");
        }

        #endregion Funtions

        #region Files

        public async Task<ServiceResult> SaveFile(VMCatalogingDoc vMCatalogingDoc, string urlViewFile)
        {
            try
            {
                // upload avatar
                if (vMCatalogingDoc.File != null)
                {
                    var stgFile = new VMStgFile
                    {
                        File = vMCatalogingDoc.File,
                        FileName = vMCatalogingDoc.File.FileName,
                        FileType = (int)EnumFile.Type.Doc,
                        IsTemp = false
                    };
                    var resultUpload = await _fileClientService.Upload(stgFile);
                    if (resultUpload.Code == null || resultUpload.Data == null || !resultUpload.Code.Equals(CommonConst.Success))
                    {
                        return new ServiceResultError("Tải file lên không thành công!");
                    }
                    else
                    {
                        var objUpload = Utils.Deserialize<VMStgFile>(resultUpload.Data.ToString());
                        var doc = await _dasRepo.CatalogingDoc.GetAsync(vMCatalogingDoc.ID);
                        if (Utils.IsNotEmpty(doc))
                        {
                            //Đánh dáu file cũ là file temp
                            if (doc.IDFile > 0)
                                await _fileClientService.MarkFileTemp(doc.IDFile);

                            if (Utils.IsNotEmpty(objUpload))
                            {
                                doc.IDFile = objUpload.ID;
                                await _dasRepo.CatalogingDoc.UpdateAsync(doc);
                                await _dasRepo.SaveAync();

                            }
                        }
                        else
                        {
                            //Tạo doc mới
                            doc = new CatalogingDoc
                            {
                                IDFile = vMCatalogingDoc.IDFile
                            };
                            await _dasRepo.CatalogingDoc.InsertAsync(doc);
                            await _dasRepo.SaveAync();
                        }
                        return new ServiceResultSuccess("Tải file lên thành công",
                                  new
                                  {
                                      UrlFile = urlViewFile + FileUltils.EncryptPathFile(objUpload.PhysicalPath, _userPrincipalService.UserId),
                                      IDFile = objUpload.ID
                                  });
                    }
                }
                return new ServiceResultError("Vui lòng chọn file tài lên");
            }
            catch (Exception)
            {
                return new ServiceResultError("Có lỗi trong quá trình tài lên");
            }
        }

        public async Task<ServiceResult> AutoOCR(long idFile)
        {
            try
            {
                var physicalPath = await _fileClientService.GetPhysicalPathById(idFile);
                if (Utils.IsNotEmpty(physicalPath))
                {
                    APIs.SetServerAddress(ioneSvAddress);
                    VBHCResult vbhc = APIs.ExtractInfoAPI.GetVBHCInfo(physicalPath.Data.ToString());
                    if (vbhc != null && vbhc.VBHCInfo != null)
                    {
                        var info = vbhc.VBHCInfo;
                        var docInfo = new
                        {
                            //KinhGui = info.KinhGui,
                            //NoiNhan = info.NoiNhan,
                            //DieuKhoan = info.DieuKhoan,
                            CodeNotation = info.KyHieu,//Ký hiệu của văn bản
                            OrganName = info.NoiBanHanh, //Noi ban hành
                            CodeNumber = info.SoKyHieu,//Số của văn bản
                                                       // SoQuyetDinh = info.SoQuyetDinh,
                            Subject = info.VeViec, //Trích yếu nội dung
                                                   //NoiDung = info.NoiDung,
                            TypeName = info.LoaiVB, //ten loai vb
                            IssuedDate = info.NgayKy, //Ngày vb
                                                      //NguoiKy = info.NguoiKy,
                        };
                        return new ServiceResultSuccess("Đã nhận dạng xong", docInfo);

                    }
                }
                return new ServiceResultError("Không tìm thấy file");

            }
            catch (Exception)
            {
                return new ServiceResultError("Lỗi nhận dạng, vui lòng thử lại");

            }
        }

        public async Task<ServiceResult> SaveScanFile(VMCatalogingDoc vMCatalogingDoc, string urlViewFile)
        {
            try
            {
                if (vMCatalogingDoc.IDFile > 0)
                {
                    var stgFile = await _dasRepo.StgFile.GetAsync(vMCatalogingDoc.IDFile);
                    if (stgFile == null)
                        return new ServiceResultError("Có lỗi trong quá trình upload file scan");

                    var doc = await _dasRepo.CatalogingDoc.GetAsync(vMCatalogingDoc.ID);

                    if (Utils.IsNotEmpty(doc))
                    {
                        //Đánh dáu file cũ là file temp
                        if (doc.IDFile > 0)
                            await _fileClientService.MarkFileTemp(doc.IDFile);

                        doc.IDFile = vMCatalogingDoc.IDFile;
                        await _dasRepo.CatalogingDoc.UpdateAsync(doc);
                        await _dasRepo.SaveAync();

                    }
                    else
                    {
                        //Tạo doc mới
                        //doc = new CatalogingDoc
                        //{
                        //    IDFile = vMCatalogingDoc.IDFile
                        //};
                        //await _dasRepo.CatalogingDoc.InsertAsync(doc);
                        //await _dasRepo.SaveAync();
                    }
                    return new ServiceResultSuccess("Lưu file scan thành công", new
                    {
                        UrlFile = urlViewFile + FileUltils.EncryptPathFile(stgFile.PhysicalPath, stgFile.CreatedBy.GetValueOrDefault(0)),
                        vMCatalogingDoc.IDFile,
                        //IDDoc = doc.ID
                    });
                }
                return new ServiceResultError("Chưa có file scan");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi trong quá trình lưu file scan");
            }
        }

        /// <summary>
        /// Cập nhật sps văn bản trong hồ sơ
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        private async Task UpdateProfileTotalDoc(int idProfile)
        {
            var profile = await _dasRepo.CatalogingProfile.GetAsync(idProfile);
            if (profile != null)
            {
                profile.TotalDoc = await _dasRepo.CatalogingDoc.CountAsync(n => n.Status != (int)EnumDocCollect.Status.InActive && n.IDCatalogingProfile == idProfile);
                await _dasRepo.CatalogingProfile.UpdateAsync(profile);
                await _dasRepo.SaveAync();
            }
        }

        #endregion Files

        #endregion DocumentCataloging

        #region ProfileStoring
        public async Task<VMIndexCatalogingProfile> ProfileStoringIndex(CatalogingProfileCondition condition)
        {
            var model = new VMIndexCatalogingProfile
            {
                //model.DictExpiryDate = await GetDictExpiryDate();
                //model.DictProfileType = Utils.EnumToDic<EnumProfile.Type>();
                Condition = condition
            };
            if (_userPrincipalService == null)
                return model;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.CatalogingProfile.GetAll()
                       where (pp.Status == (int)EnumCataloging.Status.StorageApproved)
                       //&& (condition.IDStatus <= 0 || pp.Status == condition.IDStatus)
                       && (condition.IsStoraged == 0 || pp.IDStorage == condition.IDStorage)
                       && (condition.IsStoraged == 0 || pp.IDShelve == condition.IDShelve)
                       && (condition.IsStoraged == 0 || pp.IDBox == condition.IDBox)
                       && (condition.ExcludeIds.IsEmpty() || !condition.ExcludeIds.Contains(pp.ID))
                       && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileCode.Contains(condition.Keyword))
                       && pp.IDOrgan == userData.IDOrgan
                       && (condition.IsStoraged == null || (condition.IsStoraged != null && pp.IsStoraged == (condition.IsStoraged == 1)))
                       orderby pp.UpdatedDate ?? pp.CreateDate descending
                       select _mapper.Map<VMCatalogingProfile>(pp);

            //var total = await temp.LongCountAsync();
            //if (total <= 0)
            //    return model;

            //int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //if (totalPage < condition.PageIndex)
            //    condition.PageIndex = 1;
            var list = await temp.ToListAsync();
            model.VMCatalogingProfiles = new PaginatedList<VMCatalogingProfile>(list, list.Count(), 1, list.Count());
            //model.TotalDocs = await GetTotalDocInProfiles(list.Select(n => n.ID).ToArray());
            return model;
        }
        public async Task<ServiceResult> SaveProfileStoring(Hashtable data)
        {
            var idsNotStored = Utils.GetInts(data, "IDProfile0");
            var idsStoring = Utils.GetInts(data, "IDProfile1");


            var idStorage = Utils.GetInt(data, nameof(VMCatalogingProfile.IDStorage));
            var idShelve = Utils.GetInt(data, nameof(VMCatalogingProfile.IDShelve));
            var idBox = Utils.GetInt(data, nameof(VMCatalogingProfile.IDBox));

            if (idsStoring.IsNotEmpty())
            {
                //Nếu có bản ghi bên phải mới bắt validate
                var err = new List<object>();
                if (idStorage == 0)
                    err.Add(new { Field = nameof(VMCatalogingProfile.IDStorage), Mss = "Kho không được để trống" });
                if (idShelve == 0)
                    err.Add(new { Field = nameof(VMCatalogingProfile.IDShelve), Mss = "Giá/kệ không được để trống" });
                if (idBox == 0)
                    err.Add(new { Field = nameof(VMCatalogingProfile.IDBox), Mss = "Hộp/cặp không được để trống" });
                if (err.IsNotEmpty())
                    return new ServiceResultError("Vui lòng nhập đầy đủ thông tin", err);
            }

            //update chưa xếp hộp cặp
            if (idsNotStored.IsNotEmpty())
            {
                var profileNotStored = await _dasRepo.CatalogingProfile.GetAllListAsync(n => idsNotStored.Contains(n.ID));
                foreach (var item in profileNotStored)
                {
                    item.IsStoraged = false;
                    item.IDStorage = 0;
                    item.IDShelve = 0;
                    item.IDBox = 0;
                }
                await _dasRepo.CatalogingProfile.UpdateAsync(profileNotStored);
            }
            //update đã xếp hộp cặp
            if (idsStoring.IsNotEmpty())
            {
                var profileNotStored = await _dasRepo.CatalogingProfile.GetAllListAsync(n => idsStoring.Contains(n.ID));
                foreach (var item in profileNotStored)
                {
                    item.IsStoraged = true;
                    item.IDStorage = idStorage;
                    item.IDShelve = idShelve;
                    item.IDBox = idBox;
                }
                await _dasRepo.CatalogingProfile.UpdateAsync(profileNotStored);
            }
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Xếp hộp cặp thành công");
        }
        #endregion ProfileStoring 
    }
}
