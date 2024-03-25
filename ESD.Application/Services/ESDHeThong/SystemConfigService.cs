using AutoMapper;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Utility;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class SystemConfigService : ISystemConfigServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IDasRepositoryWrapper _dasRepo;
        #endregion Properties

        #region Ctor
        public SystemConfigService(IDasRepositoryWrapper dasRepository, IMapper mapper, ILoggerManager logger) 
        {
            _mapper = mapper;
            _logger = logger;
            _dasRepo = dasRepository;
        }
        #endregion Ctor

        #region Get & List
        public async Task<PaginatedList<VMSystemConfig>> SearchByConditionPagging(SystemConfigCondition condition, bool isExport = false)
        {
            var temp = from s in _dasRepo.SystemConfig.GetAll()
                       where s.Status != (int)EnumCommon.Status.InActive
                       && (condition.Keyword.IsEmpty() || s.Name.Contains(condition.Keyword))
                       select _mapper.Map<VMSystemConfig>(s);
            if (isExport)
            {
                var rs = await temp.ToListAsync();
                return new PaginatedList<VMSystemConfig>(rs, rs.Count(),1,rs.Count());
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var res = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMSystemConfig>(res, (int)total, condition.PageIndex, condition.PageSize);

        }
        public async Task<VMUpdateSystemConfig> GetSystemConfig(int id)
        {
            var rs = await _dasRepo.SystemConfig.GetAsync(id);
            var model = Utils.Bind<VMUpdateSystemConfig>(rs.KeyValue());
            model.DateTimeVal = Utils.DateToString(rs.DateTimeVal);
            return model; 
        }
        public Task<IEnumerable<SystemConfig>> GetsActive()
        {
            return _dasRepo.SystemConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active);
        }
        #endregion Get & List

        #region Create
        public async Task<ServiceResult> CreateSystemConfig(VMUpdateSystemConfig model)
        {
            try
            {
                IEnumerable<SystemConfig> listExist;
                listExist = await _dasRepo.SystemConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active && x.Name == model.Name);
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Tên tham số đã tồn tại");
                }
                listExist = await _dasRepo.SystemConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active && x.Code == model.Code);
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Mã tham số đã tồn tại");
                }
                //SystemConfig systemConfig = _mapper.Map<SystemConfig>(model);
                var systemConfig = Utils.Bind<SystemConfig>(model.KeyValue());
                GetConfigDate(model, systemConfig, out List<object> lstErr);
                await _dasRepo.SystemConfig.InsertAsync(systemConfig);
                await _dasRepo.SaveAync();
                if (systemConfig.ID == 0)
                {
                    return new ServiceResultError("Thêm mới tham số không thành công");
                }
                return new ServiceResultSuccess("Thêm mới tham số thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Create

        #region Update
        public async Task<ServiceResult> UpdateSystemConfig(VMUpdateSystemConfig model)
        {
            try
            {
                var systemConfig = await _dasRepo.SystemConfig.GetAsync(model.ID);
                IEnumerable<SystemConfig> listExist;
                listExist = await _dasRepo.SystemConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active
                && x.Name == model.Name && x.Name != systemConfig.Name);
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Tên tham số đã tồn tại!");
                }
                model.Code = systemConfig.Code;
                var data = model.KeyValue();
                systemConfig.Bind(data);
                GetConfigDate(model, systemConfig, out List<object> lstErr);               
                //_mapper.Map(model, systemConfig);
                await _dasRepo.SystemConfig.UpdateAsync(systemConfig);
                await _dasRepo.SaveAync();
                if (systemConfig.ID == 0)
                {
                    return new ServiceResultError("Cập nhật tham số không thành công!");
                }
                return new ServiceResultSuccess("Cập nhật tham số thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteSystemConfig(int id)
        {
            try
            {
                var systemConfig = await _dasRepo.SystemConfig.GetAsync(id);
                if (systemConfig == null || systemConfig.Status != (int)EnumCommon.Status.Active)
                {
                    return new ServiceResultError("Tham số này hiện không tồn tại hoặc đã bị xóa");
                }
                systemConfig.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.SystemConfig.UpdateAsync(systemConfig);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa tham số thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        public async  Task<ServiceResult> Deletes(IEnumerable<int> ids)
        {
            try
            {
                var systemConfigs = await _dasRepo.SystemConfig.GetAllListAsync(x => ids.Contains(x.ID));
                if(systemConfigs == null || systemConfigs.Count() == 0)
                {
                    return new ServiceResultError("Tham số đã chọn hiện không tồn tại hoặc đã bị xóa");
                }
                foreach (var item in systemConfigs)
                {
                    item.Status = (int)EnumCommon.Status.InActive;
                }
                await _dasRepo.SystemConfig.UpdateAsync(systemConfigs);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa tham số thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
            throw new NotImplementedException();
        }
        public async Task<ServiceResult> Delete(object id)
        {
            try
            {
                var systemConfig = await _dasRepo.SystemConfig.GetAsync(id);
                if (systemConfig == null || systemConfig.Status != (int)EnumCommon.Status.Active)
                {
                    return new ServiceResultError("Tham số này hiện không tồn tại hoặc đã bị xóa");
                }
                systemConfig.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.SystemConfig.UpdateAsync(systemConfig);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa tham số thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        #endregion Delete

        #region Func
        private void GetConfigDate(VMUpdateSystemConfig vMSystemConfig, SystemConfig systemConfig, out List<object> errObj)
        {
            var date =Utils.GetDate(vMSystemConfig.DateTimeVal);
            errObj = new List<object>();
            if (date.HasValue)
            {
                systemConfig.DateTimeVal = date.Value;
            }
            else
            {
                
            }
        }

        public Task<IEnumerable<SystemConfig>> Gets()
        {
            throw new NotImplementedException();
        }

        public Task<SystemConfig> Get(object id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Create(SystemConfig model)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Update(SystemConfig model)
        {
            throw new NotImplementedException();
        }



        #endregion Func

    }
}
