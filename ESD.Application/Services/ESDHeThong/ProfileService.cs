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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profile = ESD.Domain.Models.DAS.Profile;

namespace ESD.Application.Services
{
    public class ProfileService : BaseMasterService, IProfileService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileListService _profileListService;
        private readonly IProfileTemplateServices _profileTemplateServices;
        private readonly ICategoryServices _categoryServices;
        private readonly ISercureLevelServices _sercureLevelServices;
        private readonly IExpiryDateServices _expiryDateServices;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;

        #endregion Properties

        #region Ctor

        public ProfileService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IProfileListService profileListService
            , IProfileTemplateServices profileTemplateServices
            , ICategoryServices categoryServices
            , ISercureLevelServices sercureLevelServices
            , IExpiryDateServices expiryDateServices
            , IUserPrincipalService userPrincipalService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _profileListService = profileListService;
            _profileTemplateServices = profileTemplateServices;
            _categoryServices = categoryServices;
            _sercureLevelServices = sercureLevelServices;
            _expiryDateServices = expiryDateServices;
            _userPrincipalService = userPrincipalService;
            _cacheManagementServices = cacheManagementServices;
        }

        #endregion Ctor

        #region Get

        public async Task<Profile> Get(object id)
        {
            return await _dasRepo.Profile.GetAsync(id);
        }

        public async Task<IEnumerable<Profile>> GetActive()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return await _dasRepo.Profile.GetAllListAsync(n => n.Status == (int)EnumProfile.Status.Active && n.IDOrgan == userData.IDOrgan);
        }

        public async Task<VMIndexProfile> SearchByConditionPagging(ProfileCondition condition)
        {
            var model = new VMIndexProfile();
            model.ProfileCondition = condition;
            model.DictProfileTemplate = (await _profileTemplateServices.GetActive((int)EnumProfileTemplate.Type.Open, 0)).ToDictionary(n => n.ID, n => n.FondName);

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.Profile.GetAll()
                       where p.Status == (int)EnumProfile.Status.Active
                       && p.IDOrgan == userData.IDOrgan
                       && (condition.Keyword.IsEmpty() || (condition.Keyword.IsNotEmpty() && (p.FileNotation.Contains(condition.Keyword) || p.FileCode.Contains(condition.Keyword))))
                       && (condition.Title.IsEmpty() || (condition.Title.IsNotEmpty() && p.Title.Contains(condition.Title)))
                       && (condition.ArrIDProfileTemplates.IsEmpty() || (condition.ArrIDProfileTemplates.IsNotEmpty() && condition.ArrIDProfileTemplates.Contains(p.IDProfileTemplate.ToString())))
                       select new VMProfile
                       {
                           ID = p.ID,
                           IDChannel = p.IDChannel,
                           Description = p.Description,
                           FileCode = p.FileCode,
                           FileNotation = p.FileNotation,
                           FileCatalog = p.FileCatalog,
                           IDProfileTemplate = p.IDProfileTemplate,
                           ProfileTemplateName = model.DictProfileTemplate.GetValueOrDefault(p.IDProfileTemplate),
                           Title = p.Title,
                           Status = p.Status,
                       };


            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMProfiles = new PaginatedList<VMProfile>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<IEnumerable<VMProfile>> GetProfilesInList()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.Profile.GetAll()
                       where p.IDOrgan == userData.IDOrgan
                       //join rgp in _dasRepo.RoleGroupPer.GetAll() on r.ID equals rgp.IDRole
                       //  join gp in _dasRepo.Profile.GetAll() on rgp.IDProfile equals gp.ID
                       select new VMProfile
                       {
                           ID = p.ID,
                           IDChannel = p.IDChannel,
                           Description = p.Description,
                           Title = p.Title,
                           Status = p.Status,
                       };
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMProfile>> GetListByCondition(ProfileCondition Profile)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.Profile.GetAll()
                       where (Profile.Keyword.IsEmpty() || p.Title.Contains(Profile.Keyword)) && p.Status == (int)EnumProfile.Status.Active && p.IDOrgan == userData.IDOrgan
                       select new VMProfile
                       {
                           ID = p.ID,
                           IDChannel = p.IDChannel,
                           Description = p.Description,
                           Title = p.Title,
                           Status = p.Status,
                       };
            return await temp.ToListAsync();
        }

        #endregion Get

        #region Create

        public async Task<VMUpdateProfile> Create()
        {
            var model = new VMUpdateProfile();
            await GetUpdateModel(model);
            return model;
        }

        public async Task<ServiceResult> Create(VMUpdateProfile vmProfile)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                //1. Insert Profile
                var data = vmProfile.KeyValue();
                var profile = Utils.Bind<Profile>(data);
                GetProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);
                profile.Language = Utils.Serialize(Utils.GetStrings(data, nameof(VMUpdateProfile.Language)));
                profile.Status = (int)EnumProfile.Status.Active;
                profile.IDOrgan = userData.IDOrgan;
                profile.Type = (int)EnumProfile.Type.Digital;
                if (await _dasRepo.Profile.IsCodeExist(profile.FileCode, (int)EnumProfile.Status.Active))
                {
                    return new ServiceResultError("Mã hồ sơ đã tồn tại");
                }
                var font = await _dasRepo.ProfileTemplate.GetAsync(profile.IDProfileTemplate) ?? new ProfileTemplate();
                if (font.Type == (int)EnumProfileTemplate.Type.Close)
                {
                    return new ServiceResultError("Phông đang chọn đã bị đóng, vui lòng chọn phông khác");
                }
                await _dasRepo.Profile.InsertAsync(profile);
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

        public async Task<VMUpdateProfile> Update(int? id)
        {
            var profile = await _dasRepo.Profile.GetAsync(id.Value);
            var model = Utils.Bind<VMUpdateProfile>(profile.KeyValue());
            model.StartDate = Utils.DateToString(profile.StartDate);
            model.EndDate = Utils.DateToString(profile.EndDate);
            await GetUpdateModel(model);
            return model;
        }
        public async Task<ServiceResult> Update(VMUpdateProfile vmProfile)
        {
            try
            {
                //1. Update Profile
                var profile = await _dasRepo.Profile.GetAsync(vmProfile.ID);
                if (profile == null)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");
                var data = vmProfile.KeyValue();
                profile.Bind(data);
                GetProfileDates(vmProfile, profile, out List<object> lstErr);
                if (lstErr.IsNotEmpty())
                    return new ServiceResultError("Dữ liệu nhập vào không hợp lệ!", lstErr);


                if (await _dasRepo.Profile.IsCodeExist(profile.FileCode, (int)EnumProfile.Status.Active, vmProfile.ID))
                {
                    return new ServiceResultError("Mã hồ sơ đã tồn tại");
                }
                var font = await _dasRepo.ProfileTemplate.GetAsync(profile.IDProfileTemplate) ?? new ProfileTemplate();
                if (font.Type == (int)EnumProfileTemplate.Type.Close)
                {
                    return new ServiceResultError("Phông đang chọn đã bị đóng, vui lòng chọn phông khác");
                }
                profile.Language = Utils.Serialize(Utils.GetStrings(data, nameof(VMUpdateProfile.Language)));
                await _dasRepo.Profile.UpdateAsync(profile);
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
        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                //Logic delete
                var positionDelete = await _dasRepo.Profile.GetAsync(id);
                if (positionDelete == null || positionDelete.Status == (int)EnumProfile.Status.InActive)
                    return new ServiceResultError("Hồ sơ này hiện không tồn tại hoặc đã bị xóa");
                positionDelete.Status = (int)EnumProfile.Status.InActive;
                await _dasRepo.Profile.UpdateAsync(positionDelete);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa hồ sơ thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> Delete(IEnumerable<int> ids)
        {
            try
            {
                //Logic delete
                var positionDeletes = await _dasRepo.Profile.GetAllListAsync(n => ids.Contains(n.ID));
                if (positionDeletes == null || positionDeletes.Count() == 0)
                    return new ServiceResultError("Hồ sơ đã chọn hiện không tồn tại hoặc đã bị xóa");

                foreach (var pos in positionDeletes)
                {
                    pos.Status = (int)EnumProfile.Status.InActive;
                }
                await _dasRepo.Profile.UpdateAsync(positionDeletes);
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

        #region Functions
        public async Task GetUpdateModel(VMUpdateProfile model)
        {
            model.DictProfileList = (await _profileListService.GetsActive()).ToDictionary(n => n.ID, n => n.Name);
            model.DictProfileTemplate = (await _profileTemplateServices.GetActive((int)EnumProfileTemplate.Type.Open, model.IDProfileTemplate.GetValueOrDefault(0))).ToDictionary(n => n.ID, n => n.FondName);
            model.DictStorage = (await _categoryServices.GetsActive(EnumCategoryType.Code.DM_Kho.ToString())).ToDictionary(n => n.ID, n => n.Name);
            model.DictLangugage = (await _categoryServices.GetsActive(EnumCategoryType.Code.DM_NgonNgu.ToString())).ToDictionary(n => n.ID, n => n.Name);
            model.DictExpiryDate = (await _expiryDateServices.GetsActive()).ToDictionary(n => n.ID, n => n.Name);
            model.DictBox = (await _categoryServices.GetsActive(EnumCategoryType.Code.DM_HopSo.ToString())).ToDictionary(n => n.ID, n => n.Name);
            model.DictSecurityLevel = (await _sercureLevelServices.GetsActive()).ToDictionary(n => n.ID, n => n.Name);
        }

        private void GetProfileDates(VMUpdateProfile vmProfile, Profile profile, out List<object> errObj)
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
        #endregion

    }
}