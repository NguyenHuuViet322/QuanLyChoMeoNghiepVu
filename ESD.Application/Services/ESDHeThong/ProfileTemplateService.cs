using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ESD.Domain.Enums;
using System.Runtime.CompilerServices;
using ESD.Utility;
using DocumentFormat.OpenXml.Office2010.Excel;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;
using ESD.Application.Constants;

namespace ESD.Application.Services
{
    public class ProfileTemplateService : BaseMasterService, IProfileTemplateServices
    {
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;

        public ProfileTemplateService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService userPrincipalService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _userPrincipalService = userPrincipalService;
            _cacheManagementServices = cacheManagementServices;
        }

        #region BaseRepo
        public async Task<IEnumerable<ProfileTemplate>> Gets()
        {
            return await _dasRepo.ProfileTemplate.GetAllListAsync();
        }
        public async Task<ProfileTemplate> Get(object id)
        {
            return await _dasRepo.ProfileTemplate.GetAsync(id);
        }

        public async Task<ServiceResult> Create(ProfileTemplate model)
        {
            await _dasRepo.ProfileTemplate.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }
        public async Task<ServiceResult> Update(ProfileTemplate model)
        {
            await _dasRepo.ProfileTemplate.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add category suceess!");
        }

        public async Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }
        #endregion BaseRepo

        #region Create & Search
        public async Task<ServiceResult> CreateProfileTemplate(VMCreateProfileTemplate vmProfileTemplate)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //check exist unique field
            var listExist = await _dasRepo.ProfileTemplate.GetAllListAsync(m => (m.FondCode == vmProfileTemplate.FondCode && m.Status == (int)EnumCommon.Status.Active && (userData.HasOrganPermission ? m.IDOrgan == userData.IDOrgan : (m.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + m.IDAgency + "|")))));
            if (IsExisted(listExist))
                return new ServiceResultError("Mã phông đã tồn tại!");

            //update data
            UpdateData(vmProfileTemplate);

            var profileTemplate = _mapper.Map<ProfileTemplate>(vmProfileTemplate);
            profileTemplate.IDAgency = userData.IDAgency;
            profileTemplate.IDOrgan = userData.IDOrgan;
            await _dasRepo.ProfileTemplate.InsertAsync(profileTemplate);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Thêm mới phông thành công!");
        }

        public async Task<PaginatedList<ProfileTemplateExportExcel>> SearchByConditionPagging(ProfileTemplateCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.ProfileTemplate.GetAll()
                       where p.Status == (int)EnumCommon.Status.Active
                       && (string.IsNullOrEmpty(condition.Keyword) || p.FondName.Contains(condition.Keyword) || p.FondCode.Contains(condition.Keyword))
                       && (condition.cbbStorage <= 0 || p.IDStorage == condition.cbbStorage)
                       && (condition.Type <= 0 || p.Type == condition.Type)
                       && (p.IDOrgan == userData.IDOrgan || p.IDOrgan <= 0)
                       join a in _dasRepo.Organ.GetAll() on p.IDOrgan equals a.ID
                       where a.Status == (int)EnumCommon.Status.Active
                       //join c in _dasRepo.Category.GetAll() on p.IDStorage equals c.ID
                      // where c.CodeType == EnumCategoryType.Code.DM_Kho.ToString()
                        // && c.Status == (int)EnumCategory.Status.Active
                       orderby p.ID descending
                       select new ProfileTemplateExportExcel
                       {
                           ID = p.ID,
                           OrganName = a.Name,
                           FondHistory = p.FondHistory,
                           FondName = p.FondName,
                           FondCode = p.FondCode,
                           TypeName = p.Type == (int)EnumProfileTemplate.Type.Open ? StringUltils.GetEnumDescription(EnumProfileTemplate.Type.Open)
                           : StringUltils.GetEnumDescription(EnumProfileTemplate.Type.Close),
                           //StorageName = c.Name
                       };

            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new PaginatedList<ProfileTemplateExportExcel>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<IEnumerable<ProfileTemplateExportExcel>> GetByCondition(ProfileTemplateCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from p in _dasRepo.ProfileTemplate.GetAll()
                       where p.Status == (int)EnumCommon.Status.Active
                       && (string.IsNullOrEmpty(condition.Keyword) || p.FondName.Contains(condition.Keyword) || p.FondCode.Contains(condition.Keyword))
                       && (condition.cbbStorage <= 0 || p.IDStorage == condition.cbbStorage)
                       && (condition.Type <= 0 || p.Type == condition.Type)
                       && (p.IDOrgan == userData.IDOrgan || p.IDOrgan <= 0)
                       join a in _dasRepo.Organ.GetAll() on p.IDOrgan equals a.ID
                       where a.Status == (int)EnumCommon.Status.Active
                       //join c in _dasRepo.Category.GetAll() on p.IDStorage equals c.ID
                       //where c.CodeType == EnumCategoryType.Code.DM_Kho.ToString()
                       //  && c.Status == (int)EnumCategory.Status.Active
                       orderby p.ID descending
                       select new ProfileTemplateExportExcel
                       {
                           ID = p.ID,
                           OrganName = a.Name,
                           FondHistory = p.FondHistory,
                           FondName = p.FondName,
                           FondCode = p.FondCode,
                           TypeName = p.Type == (int)EnumProfileTemplate.Type.Open ? StringUltils.GetEnumDescription(EnumProfileTemplate.Type.Open)
                           : StringUltils.GetEnumDescription(EnumProfileTemplate.Type.Close),
                           //StorageName = c.Name
                       };

            return await temp.ToListAsync();
        }
        #endregion Create & Search

        #region Get
        public async Task<VMProfileTemplate> GetDetail(int id)
        {
            var profileTemplate = await _dasRepo.ProfileTemplate.GetAsync(id);
            if (!IsExisted(profileTemplate))
                return null;

            return _mapper.Map<VMProfileTemplate>(profileTemplate);
        }

        public async Task<VMEditProfileTemplate> GetProfileTemplate(int id)
        {
            var profileTemplate = await _dasRepo.ProfileTemplate.GetAsync(id);
            if (!IsExisted(profileTemplate))
                return null;

            return _mapper.Map<VMEditProfileTemplate>(profileTemplate);
        }

        public async Task<IEnumerable<ProfileTemplate>> GetActive(int type = 0, int id = 0)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return await _dasRepo.ProfileTemplate.GetAllListAsync(a => a.Status == (int)EnumOrgan.Status.Active && (type == 0 || (type > 0 && a.Type == type) || id == a.ID) && (userData.HasOrganPermission ? a.IDOrgan == userData.IDOrgan : (a.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + a.IDAgency + "|"))));
        }

        public async Task<IEnumerable<ProfileTemplate>> GetProfileTemplateByStorage(IEnumerable<int> storageIDs)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return await _dasRepo.ProfileTemplate.GetAll()
                .Where(x => x.Status == (int)EnumCommon.Status.Active && storageIDs.Contains(x.IDStorage) && (userData.HasOrganPermission ? x.IDOrgan == userData.IDOrgan : (x.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + x.IDAgency + "|"))))
                .ToListAsync();
        }
        #endregion Get

        #region Update    
        public async Task<ServiceResult> UpdateProfileTemplate(VMEditProfileTemplate vmProfileTemplate)
        {
            var profileTemplate = await _dasRepo.ProfileTemplate.GetAsync(vmProfileTemplate.ID);
            if (!IsExisted(profileTemplate))
                return new ServiceResultError("Không tồn tại phông này!");

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //check exist unique field
            var listExist = await _dasRepo.ProfileTemplate.GetAllListAsync(m => (m.FondCode == vmProfileTemplate.FondCode && m.FondCode != profileTemplate.FondCode && (m.IDOrgan == userData.IDOrgan || m.IDOrgan == 0)));
            if (IsExisted(listExist))
                return new ServiceResultError("Mã phông đã tồn tại!");

            //update data
            UpdateData(vmProfileTemplate, userData.IDOrgan);
            vmProfileTemplate.IDAgency = userData.IDAgency;
            vmProfileTemplate.IDOrgan = userData.IDOrgan;
            _mapper.Map(vmProfileTemplate, profileTemplate);
            await _dasRepo.ProfileTemplate.UpdateAsync(profileTemplate);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật phông thành công!");
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteMultiProfileTemplate(IEnumerable<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    await DeleteProfileTemplate(id);
                }
            }
            catch (Exception)
            {
                return new ServiceResultError("Xóa phông không thành công!");
            }

            return new ServiceResultSuccess("Xóa phông thành công!");
        }

        public async Task<ServiceResult> DeleteProfileTemplate(int id)
        {
            var profileTemplate = await _dasRepo.ProfileTemplate.GetAsync(id);
            if (!IsExisted(profileTemplate))
                return new ServiceResultError("Phông này không tồn tại!");

            //check existed planprofile with IDProfileTemplate
            var existed = await _dasRepo.PlanProfile.GetAllListAsync(p => p.IDProfileTemplate == id && p.Status != (int)EnumProfilePlan.Status.InActive);
            if (IsExisted(existed))
                return new ServiceResultError("Xóa phông không thành công, phông đã sử dụng ở nghiệp vụ!");

            //update status this Organ
            profileTemplate.Status = (int)EnumCommon.Status.InActive;
            await _dasRepo.ProfileTemplate.UpdateAsync(profileTemplate);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Xóa phông thành công!");
        }
        #endregion Delete

        #region Private method
        private void UpdateData(VMCreateProfileTemplate vmProfileTemplate)
        {
            vmProfileTemplate.Status = (int)EnumCommon.Status.Active;
        }

        private void UpdateData(VMEditProfileTemplate vmProfileTemplate, int organId)
        {
            vmProfileTemplate.Status = (int)EnumCommon.Status.Active;
            vmProfileTemplate.IDOrgan = organId;
        }

        private bool IsExisted(ProfileTemplate model)
        {
            if (model == null || model.ID == 0 || model.Status != (int)EnumCommon.Status.Active)
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
