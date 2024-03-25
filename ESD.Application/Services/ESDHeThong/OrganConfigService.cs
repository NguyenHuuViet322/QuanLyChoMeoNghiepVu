using AutoMapper;
using ESD.Application.Constants;
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

namespace ESD.Application.Services
{
    public class OrganConfigService : IOrganConfigServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;
        #endregion Properties

        #region Ctor
        public OrganConfigService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService userPrincipalService
            , ICacheManagementServices cacheManagementServices)
        {
            _mapper = mapper;
            _logger = logger;
            _dasRepo = dasRepository;
            _userPrincipalService = userPrincipalService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion Ctor

        #region Get & List
        public async Task<PaginatedList<VMOrganConfig>> SearchByConditionPagging(OrganConfigCondition condition, bool isExport = false)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from s in _dasRepo.OrganConfig.GetAll()
                       where s.Status != (int)EnumCommon.Status.InActive
                       && (condition.Keyword.IsEmpty() || s.Name.Contains(condition.Keyword))
                       && (s.IDOrgan == userData.IDOrgan)
                       select _mapper.Map<VMOrganConfig>(s);
            if (isExport)
            {
                var rs = await temp.ToListAsync();
                return new PaginatedList<VMOrganConfig>(rs, rs.Count(), 1, rs.Count());
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }

            var res = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMOrganConfig>(res, (int)total, condition.PageIndex, condition.PageSize);

        }
        public async Task<VMUpdateOrganConfig> GetOrganConfig(int id)
        {
            var rs = await _dasRepo.OrganConfig.GetAsync(id);
            var model = Utils.Bind<VMUpdateOrganConfig>(rs.KeyValue());
            model.DateTimeVal = Utils.DateToString(rs.DateTimeVal);
            return model;
        }
        #endregion Get & List

        #region Create
        public async Task<ServiceResult> CreateOrganConfig(VMUpdateOrganConfig model)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                IEnumerable<OrganConfig> listExist;
                listExist = await _dasRepo.OrganConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active && x.Name == model.Name && x.IDOrgan == userData.IDOrgan);
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Tên tham số đã tồn tại");
                }
                listExist = await _dasRepo.OrganConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active && x.Code == model.Code && x.IDOrgan == userData.IDOrgan);
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Mã tham số đã tồn tại");
                }
                //OrganConfig OrganConfig = _mapper.Map<OrganConfig>(model);
                var OrganConfig = Utils.Bind<OrganConfig>(model.KeyValue());
                GetConfigDate(model, OrganConfig, out List<object> lstErr);
                OrganConfig.IDOrgan = userData.IDOrgan;
                await _dasRepo.OrganConfig.InsertAsync(OrganConfig);
                await _dasRepo.SaveAync();
                if (OrganConfig.ID == 0)
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
        public async Task<ServiceResult> UpdateOrganConfig(VMUpdateOrganConfig model)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var organConfig = await _dasRepo.OrganConfig.GetAsync(model.ID);
                IEnumerable<OrganConfig> listExist;
                listExist = await _dasRepo.OrganConfig.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active
                && x.Name == model.Name && x.Name != organConfig.Name && x.IDOrgan == userData.IDOrgan);
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Tên tham số đã tồn tại!");
                }
                model.Code = organConfig.Code;
                var data = model.KeyValue();
                organConfig.Bind(data);
                GetConfigDate(model, organConfig, out List<object> lstErr);
                //_mapper.Map(model, OrganConfig);
                await _dasRepo.OrganConfig.UpdateAsync(organConfig);
                await _dasRepo.SaveAync();
                if (organConfig.ID == 0)
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
        public async Task<ServiceResult> DeleteOrganConfig(int id)
        {
            try
            {
                var OrganConfig = await _dasRepo.OrganConfig.GetAsync(id);
                if (OrganConfig == null || OrganConfig.Status != (int)EnumCommon.Status.Active)
                {
                    return new ServiceResultError("Tham số này hiện không tồn tại hoặc đã bị xóa");
                }
                OrganConfig.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.OrganConfig.UpdateAsync(OrganConfig);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa tham số thành công");
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
                var OrganConfigs = await _dasRepo.OrganConfig.GetAllListAsync(x => ids.Contains(x.ID));
                if (OrganConfigs == null || OrganConfigs.Count() == 0)
                {
                    return new ServiceResultError("Tham số đã chọn hiện không tồn tại hoặc đã bị xóa");
                }
                foreach (var item in OrganConfigs)
                {
                    item.Status = (int)EnumCommon.Status.InActive;
                }
                await _dasRepo.OrganConfig.UpdateAsync(OrganConfigs);
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
                var OrganConfig = await _dasRepo.OrganConfig.GetAsync(id);
                if (OrganConfig == null || OrganConfig.Status != (int)EnumCommon.Status.Active)
                {
                    return new ServiceResultError("Tham số này hiện không tồn tại hoặc đã bị xóa");
                }
                OrganConfig.Status = (int)EnumCommon.Status.InActive;
                await _dasRepo.OrganConfig.UpdateAsync(OrganConfig);
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
        private void GetConfigDate(VMUpdateOrganConfig vMOrganConfig, OrganConfig OrganConfig, out List<object> errObj)
        {
            var date = Utils.GetDate(vMOrganConfig.DateTimeVal);
            errObj = new List<object>();
            if (date.HasValue)
            {
                OrganConfig.DateTimeVal = date.Value;
            }
            else
            {

            }
        }

        public Task<IEnumerable<OrganConfig>> Gets()
        {
            throw new NotImplementedException();
        }

        public Task<OrganConfig> Get(object id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Create(OrganConfig model)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Update(OrganConfig model)
        {
            throw new NotImplementedException();
        }



        #endregion Func

    }
}
