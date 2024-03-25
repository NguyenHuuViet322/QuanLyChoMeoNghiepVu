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
using Newtonsoft.Json;
using ESD.Utility.LogUtils;
using ESD.Application.Enums;
using ESD.Domain.Enums;
using ESD.Application.Constants;
using ESD.Utility;

namespace ESD.Application.Services
{
    public class ExpiryDateService : BaseMasterService, IExpiryDateServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        #endregion

        #region Ctor
        public ExpiryDateService(IDasRepositoryWrapper dasRepository
            , IMapper mapper, ILoggerManager logger) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Get
        public async Task<ExpiryDate> Get(object id)
        {
            return await _dasRepo.ExpiryDate.GetAsync(id);
        }
        public async Task<IEnumerable<ExpiryDate>> Gets()
        {
            return await _dasRepo.ExpiryDate.GetAllListAsync();
        }
        public async Task<VMExpiryDate> GetExpiryDate(int id)
        {
            var temp = from r in _dasRepo.ExpiryDate.GetAll()
                       where r.ID == id
                       select new VMExpiryDate
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Description = r.Description,
                           Status = r.Status,
                           Value = r.Value
                       };
            var result = await temp.FirstOrDefaultAsync();
            if (result == null)
                return null;
            result.MaxValueExpiryDate = int.TryParse((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_VALUE_EXPIRYDATE)).ToString(), out int max) ? max : 1000;
            return result;
        }

        public async Task<VMExpiryDate> GetNewExpiryDate()
        {
            var result = new VMExpiryDate();
            result.MaxValueExpiryDate = int.TryParse((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_VALUE_EXPIRYDATE)).ToString(), out int max) ? max : 1000;
            return result;
        }
        public async Task<PaginatedList<VMExpiryDate>> SearchByConditionPagging(ExpiryDateCondition condition)
        {
            var temp = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                       where (string.IsNullOrEmpty(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Code.Contains(condition.Keyword))
                       orderby r.Value, r.ID descending
                       select new VMExpiryDate
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description,
                           Value = r.Value
                       };
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            PaginatedList<VMExpiryDate> model = new PaginatedList<VMExpiryDate>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }
        public async Task<IEnumerable<VMExpiryDate>> GetListByCondition(ExpiryDateCondition condition)
        {
            var temp = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                       where (string.IsNullOrEmpty(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Code.Contains(condition.Keyword))
                       orderby r.Value, r.ID descending
                       select new VMExpiryDate
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description,
                           Value = r.Value
                       };
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMExpiryDate>> GetsActive()
        {
            var temp = from r in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                       select new VMExpiryDate
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description,
                           Value = r.Value
                       };
            return await temp.ToListAsync();
        }
        #endregion

        #region Create
        public async Task<ServiceResult> Create(ExpiryDate model)
        {
            await _dasRepo.ExpiryDate.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }

        public async Task<ServiceResult> CreateExpiryDate(VMExpiryDate vmExpiryDate)
        {
            try
            {
                List<ExpiryDate> listExistExpiryDate;
                listExistExpiryDate = await _dasRepo.ExpiryDate.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active && (x.Code == vmExpiryDate.Code || x.Name == vmExpiryDate.Name)).ToListAsync();
                if (listExistExpiryDate != null && listExistExpiryDate.Count() > 0)
                {
                    if(listExistExpiryDate.Where(x => x.Code == vmExpiryDate.Code).IsNotEmpty())
                        return new ServiceResultError("Mã thời hạn bảo quản đã tồn tại");
                    else
                        return new ServiceResultError("Thời hạn bảo quản đã tồn tại");
                }
                    
                ExpiryDate expiryDate = _mapper.Map<ExpiryDate>(vmExpiryDate);
                await _dasRepo.ExpiryDate.InsertAsync(expiryDate);
                await _dasRepo.SaveAync();
                if (expiryDate.ID == 0)
                    return new ServiceResultError("Thêm mới thời hạn bảo quản không thành công");

                return new ServiceResultSuccess("Thêm mới thời hạn bảo quản thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }           
        }
        #endregion

        #region Update
        public async Task<ServiceResult> Update(ExpiryDate model)
        {
            await _dasRepo.ExpiryDate.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật thời hạn bảo quản thành công");
        }
        public async Task<ServiceResult> UpdateExpiryDate(VMExpiryDate vmExpiryDate)
        {
            try
            {
                var expiryDate = await _dasRepo.ExpiryDate.GetAsync(vmExpiryDate.ID);
                List<ExpiryDate> listExistExpiryDate;
                listExistExpiryDate = await _dasRepo.ExpiryDate.GetAll()
                    .Where(m => m.Status == (int)EnumCommon.Status.Active && ((m.Code == vmExpiryDate.Code && m.Code != expiryDate.Code)
                    || (m.Name == vmExpiryDate.Name && m.Name != expiryDate.Name))).ToListAsync();
                if (listExistExpiryDate != null && listExistExpiryDate.Count() > 0)
                {
                    if (listExistExpiryDate.Where(x => x.Code == vmExpiryDate.Code).IsNotEmpty())
                        return new ServiceResultError("Mã thời hạn bảo quản đã tồn tại");
                    else
                        return new ServiceResultError("Thời hạn bảo quản đã tồn tại");
                }
                _mapper.Map(vmExpiryDate, expiryDate);
                await _dasRepo.ExpiryDate.UpdateAsync(expiryDate);
                await _dasRepo.SaveAync();
                if (expiryDate.ID == 0)
                    return new ServiceResultError("Cập nhật thời hạn bảo quản không thành công");

                return new ServiceResultSuccess("Cập nhật thời hạn bảo quản thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }            
        }
        #endregion

        #region Delete
        public async Task<ServiceResult> Delete(object id)
        {
            try
            {
                var expiryDate = await _dasRepo.ExpiryDate.GetAsync(id);
                if (expiryDate == null || expiryDate.Status == (int)EnumExpiryDate.Status.InActive)
                    return new ServiceResultError("Thời hạn bảo quản này hiện không tồn tại hoặc đã bị xóa");
                expiryDate.Status = (int)EnumExpiryDate.Status.InActive;
                await _dasRepo.ExpiryDate.UpdateAsync(expiryDate);
                await _dasRepo.SaveAync();
                if (expiryDate == null)
                    return new ServiceResultError("Thời hạn bảo quản không tồn tại");
                return new ServiceResultSuccess("Xóa thời hạn bảo quản thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }           
        }
        public async Task<ServiceResult> DeleteExpiryDate(int id)
        {
            try
            {

                var expiryDate = await _dasRepo.ExpiryDate.GetAsync(id);
                if (expiryDate == null || expiryDate.Status == (int)EnumExpiryDate.Status.InActive)
                    return new ServiceResultError("Thời hạn bảo quản này hiện không tồn tại hoặc đã bị xóa");
                expiryDate.Status = (int)EnumExpiryDate.Status.InActive;

                var countProfile = await _dasRepo.Profile.AnyAsync(x => x.Status == (int)EnumCommon.Status.Active
                 && x.IDExpiryDate == id);
                if (countProfile)
                {
                    return new ServiceResultError("Bạn không được xóa dữ liệu đang được sử dụng");
                }
                var countProfilePlan  = await _dasRepo.PlanProfile.AnyAsync(x => x.Status == (int)EnumCommon.Status.Active
                 && x.IDExpiryDate == id);
                if (countProfilePlan)
                {
                    return new ServiceResultError("Bạn không được xóa dữ liệu đang được sử dụng");
                }
                var countProfileCatalog = await _dasRepo.CatalogingProfile.AnyAsync(x => x.Status == (int)EnumCommon.Status.Active
                && x.IDExpiryDate == id);
                if (countProfileCatalog)
                {
                    return new ServiceResultError("Bạn không được xóa dữ liệu đang được sử dụng");
                }

                await _dasRepo.ExpiryDate.UpdateAsync(expiryDate);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa thời hạn bảo quản thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        public async Task<ServiceResult> DeleteMultiExpiryDate(IEnumerable<int> ids)
        {
            try
            {
                var expiryDates = await _dasRepo.ExpiryDate.GetAllListAsync(n => ids.Contains(n.ID) && n.Status==(int)EnumCommon.Status.Active);
                if (expiryDates == null || expiryDates.Count() == 0)
                    return new ServiceResultError("Thời hạn bảo quản không tồn tại hoặc đã bị xóa");

                var countProfile = await _dasRepo.Profile.AnyAsync(x => x.Status == (int)EnumCommon.Status.Active
                 && ids.Contains(x.IDExpiryDate));
                if (countProfile)
                {
                    return new ServiceResultError("Bạn không được xóa dữ liệu đang được sử dụng");
                }
                var countProfilePlan = await _dasRepo.PlanProfile.AnyAsync(x => x.Status == (int)EnumCommon.Status.Active
                && ids.Contains(x.IDExpiryDate));
                if (countProfilePlan)
                {
                    return new ServiceResultError("Bạn không được xóa dữ liệu đang được sử dụng");
                }
                var countProfileCatalog = await _dasRepo.CatalogingProfile.AnyAsync(x => x.Status == (int)EnumCommon.Status.Active
                && ids.Contains(x.IDExpiryDate));
                if (countProfileCatalog)
                {
                    return new ServiceResultError("Bạn không được xóa dữ liệu đang được sử dụng");
                }

                foreach (var item in expiryDates)
                {
                    item.Status = (int)EnumExpiryDate.Status.InActive;
                }
                await _dasRepo.ExpiryDate.UpdateAsync(expiryDates);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa thời hạn bảo quản thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }       
        }

        #endregion

        #region Function
        public async Task<bool> IsCodeExist(string code)
        {
            return await _dasRepo.ExpiryDate.AnyAsync(s => s.Code == code);
        }


        #endregion
    }
}
