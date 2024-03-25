using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using ESD.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using Newtonsoft.Json;
using ESD.Utility;
using ESD.Utility.LogUtils;

namespace ESD.Application.Services
{
    public class ProfileListService : BaseMasterService, IProfileListService
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        #endregion

        #region Ctor
        public ProfileListService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger) : base(dasRepository)
        {
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        #region Get
        public async Task<PaginatedList<VMProfileList>> SearchByConditionPagging(ProfileListCondition condition)
        {
            var temp = from pl in _dasRepo.ProfileList.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active)
                       from st in _dasRepo.Category.GetAll().Where(x => x.CodeType == "DM_Kho" && x.ID == pl.IDStorage).DefaultIfEmpty()
                       from pt in _dasRepo.ProfileTemplate.GetAll().Where(x => x.ID == pl.IDProfileTemplate).DefaultIfEmpty()
                       where ((string.IsNullOrEmpty(condition.Keyword)
                       || pl.Name.Contains(condition.Keyword)
                       || pl.Code.Contains(condition.Keyword)))
                       && (string.IsNullOrEmpty(condition.Storages)
                       || condition.listStoragetr.Contains(pl.IDStorage.ToString()))
                       && (string.IsNullOrEmpty(condition.ProfileTemplates)
                       || condition.listProfileTemplatestr.Contains(pl.IDProfileTemplate.ToString()))
                       orderby pl.ID descending
                       select new VMProfileList
                       {
                           ID = pl.ID,
                           Code = pl.Code,
                           Name = pl.Name,
                           Description = pl.Description,
                           IDStorage = pl.IDStorage,
                           StorageName = st.Name,
                           IDProfileTemplate = pl.IDProfileTemplate,
                           FondName = pt.FondName,
                           CreatedBy = pl.CreatedBy,
                           CreateDate = pl.CreateDate,
                           UpdatedDate = pl.UpdatedDate,
                           UpdatedBy = pl.UpdatedBy
                       };
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            PaginatedList<VMProfileList> model = new PaginatedList<VMProfileList>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }
        public async Task<VMProfileList> GetActive(int id)
        {
            var temp = from r in _dasRepo.ProfileList.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active)
                       where r.ID == id
                       select new VMProfileList
                       {
                           ID = r.ID,
                           Code = r.Code,
                           Name = r.Name,
                           Description = r.Description,
                           IDStorage = r.IDStorage,
                           IDProfileTemplate = r.IDProfileTemplate,
                           Status = r.Status,
                           CreatedBy = r.CreatedBy,
                           CreateDate = r.CreateDate,
                           UpdatedDate = r.UpdatedDate,
                           UpdatedBy = r.UpdatedBy
                       };
            return await temp.FirstOrDefaultAsync();
        } 
        public async Task<IEnumerable<VMProfileList>> GetsActive()
        {
            var temp = from r in _dasRepo.ProfileList.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active) 
                       select new VMProfileList
                       {
                           ID = r.ID,
                           Code = r.Code,
                           Name = r.Name,
                           Description = r.Description,
                           IDStorage = r.IDStorage,
                           IDProfileTemplate = r.IDProfileTemplate,
                           Status = r.Status,
                           CreatedBy = r.CreatedBy,
                           CreateDate = r.CreateDate,
                           UpdatedDate = r.UpdatedDate,
                           UpdatedBy = r.UpdatedBy
                       };
            return await temp.ToListAsync();
        }
        public async Task<IEnumerable<VMProfileList>> GetListByCondition(ProfileListCondition condition)
        {
            var temp = from pl in _dasRepo.ProfileList.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active)
                       from st in _dasRepo.Category.GetAll().Where(x => x.CodeType == "DM_Kho" && x.ID == pl.IDStorage).DefaultIfEmpty()
                       from pt in _dasRepo.ProfileTemplate.GetAll().Where(x => x.ID == pl.IDProfileTemplate).DefaultIfEmpty()
                       where ((string.IsNullOrEmpty(condition.Keyword)
                       || pl.Name.Contains(condition.Keyword)
                       || pl.Code.Contains(condition.Keyword)))
                       && (string.IsNullOrEmpty(condition.Storages)
                       || condition.listStoragetr.Contains(pl.IDStorage.ToString()))
                       && (string.IsNullOrEmpty(condition.ProfileTemplates)
                       || condition.listProfileTemplatestr.Contains(pl.IDProfileTemplate.ToString()))
                       orderby pl.ID descending
                       select new VMProfileList
                       {
                           ID = pl.ID,
                           Code = pl.Code,
                           Name = pl.Name,
                           Description = pl.Description,
                           IDStorage = pl.IDStorage,
                           StorageName = st.Name,
                           IDProfileTemplate = pl.IDProfileTemplate,
                           FondName = pt.FondName,
                           CreatedBy = pl.CreatedBy,
                           CreateDate = pl.CreateDate,
                           UpdatedDate = pl.UpdatedDate,
                           UpdatedBy = pl.UpdatedBy
                       };
            return await temp.ToListAsync();
        }

        #endregion

        #region Create
        public async Task<ServiceResult> CreateProfileList(VMProfileList vmProfileList)
        {
            try
            {
                List<ProfileList> listExistProfileList;
                listExistProfileList = await _dasRepo.ProfileList.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(x => x.Code == vmProfileList.Code).ToListAsync();
                if (listExistProfileList != null && listExistProfileList.Count() > 0)
                    return new ServiceResultError("Mã mục lục đã tồn tại");

                listExistProfileList = await _dasRepo.ProfileList.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(x => x.Name == vmProfileList.Name).ToListAsync();
                if (listExistProfileList != null && listExistProfileList.Count() > 0)
                    return new ServiceResultError("Tên mục lục đã tồn tại");

                var hasStorage = await _dasRepo.Category
                    .AnyAsync(x => x.ID == vmProfileList.IDStorage
                && x.CodeType == "DM_Kho"
                && x.Status == (int)EnumCommon.Status.Active);
                if (!hasStorage)
                    return new ServiceResultError("Kho không tồn tại hoặc đã bị xóa");

                var hasFond = await _dasRepo.ProfileTemplate
                    .AnyAsync(x => x.ID == vmProfileList.IDProfileTemplate
                    && x.IDStorage == vmProfileList.IDStorage
                    && x.Status == (int)EnumCommon.Status.Active);
                if (!hasFond)
                    return new ServiceResultError("Phông không tồn tại hoặc đã bị xóa");

                ProfileList profileList = _mapper.Map<ProfileList>(vmProfileList);
                await _dasRepo.ProfileList.InsertAsync(profileList);
                await _dasRepo.SaveAync();
                if (profileList.ID == 0)
                    return new ServiceResultError("Thêm mới mục lục không thành công");
                return new ServiceResultSuccess("Thêm mới mục lục thành công");

            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
            throw new NotImplementedException();
        }
        #endregion

        #region Delete
        public async Task<ServiceResult> DeleteProfileList(int id)
        {
            try
            {
                var profileList = await _dasRepo.ProfileList.GetAsync(id);
                if (profileList == null || profileList.Status == (int)EnumCommon.Status.InActive)
                    return new ServiceResultError("Mục lục này không tồn tại hoặc đã bị xóa");
                profileList.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.ProfileList.UpdateAsync(profileList);
                await _dasRepo.SaveAync();

                return new ServiceResultSuccess("Xóa mục lục thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }
        public async Task<ServiceResult> DeleteMultiProfileList(IEnumerable<int> ids)
        {
            try
            {
                var profileLists = await _dasRepo.ProfileList.GetAllListAsync(x => ids.Contains(x.ID));
                if (profileLists == null || profileLists.Count() == 0)
                    return new ServiceResultError("Mục lục đã chọn không tồn tại hoặc đã bị xóa");
                foreach (var item in profileLists)
                {
                    item.Status = (int)EnumCommon.Status.InActive;
                }
                await _dasRepo.ProfileList.UpdateAsync(profileLists);
                await _dasRepo.SaveAync();

                return new ServiceResultSuccess("Xóa mục lục thành công");
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        #endregion

        #region Update
        public async Task<ServiceResult> UpdateProfileList(VMProfileList vMProfileList)
        {
            try
            {
                var profileList = await _dasRepo.ProfileList.GetAsync(vMProfileList.ID);

                List<ProfileList> listExsit;
                listExsit = await _dasRepo.ProfileList.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(x => x.Code == vMProfileList.Code && x.Code != profileList.Code)
                    .ToListAsync();
                if (listExsit != null && listExsit.Count() > 0)
                    return new ServiceResultError("Mã mục lục đã tồn tại");

                listExsit = await _dasRepo.ProfileList.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(x => x.Name == vMProfileList.Name && x.Name != profileList.Name)
                    .ToListAsync();
                if (listExsit != null && listExsit.Count() > 0)
                    return new ServiceResultError("Tên mục lục đã tồn tại");

                var hasStorage = await _dasRepo.Category
                    .AnyAsync(x => x.ID == vMProfileList.IDStorage
                && x.CodeType == "DM_Kho"
                && x.Status == (int)EnumCommon.Status.Active);
                if (!hasStorage)
                    return new ServiceResultError("Kho không tồn tại hoặc đã bị xóa");

                var hasFond = await _dasRepo.ProfileTemplate
                    .AnyAsync(x => x.ID == vMProfileList.IDProfileTemplate
                    && x.IDStorage == vMProfileList.IDStorage
                    && x.Status == (int)EnumCommon.Status.Active);
                if (!hasFond)
                    return new ServiceResultError("Phông không tồn tại hoặc đã bị xóa");

                _mapper.Map(vMProfileList,profileList);
                await _dasRepo.ProfileList.UpdateAsync(profileList);
                await _dasRepo.SaveAync();

                if (profileList.ID == 0)
                    return new ServiceResultError("Cập nhật mục lục không thành công");

                return new ServiceResultSuccess("Cập nhật mục lục thành công");
            }
            catch (Exception ex)
            {

                return new ServiceResultError(ex.Message);
            }
        }
        #endregion

        public Task<ServiceResult> Create(ProfileList model)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileList> Get(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProfileList>> Gets()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Update(ProfileList model)
        {
            throw new NotImplementedException();
        }

       
    }
}
