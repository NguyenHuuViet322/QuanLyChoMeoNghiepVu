using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Interfaces.ESDNghiepVu;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Repositories.ESDNghiepVu;
using ESD.Utility;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services.ESDNghiepVu
{
    public class ChuyenMonKiThuatService : BaseMasterService, IChuyenMonKiThuatServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IModuleService _module;
        private readonly IDefaultDataService _defaultDataService;
        private readonly IHostApplicationLifetime _host;
        private ICacheManagementServices _cacheManagementServices;
        private IWebHostEnvironment _env;

        #endregion

        #region Ctor
        public ChuyenMonKiThuatService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IModuleService module
            , IDefaultDataService defaultDataService,
            IHostApplicationLifetime host
            , ICacheManagementServices cacheManagementServices, IESDNghiepVuRepositoryWrapper dasNghiepVuRepo,
            IWebHostEnvironment env) : base(dasRepository, dasNghiepVuRepo)
        {
            _mapper = mapper;
            _logger = logger;
            _module = module;
            _cacheManagementServices = cacheManagementServices;
            _defaultDataService = defaultDataService;
            _host = host;
            _env = env;
        }


        #endregion

        #region Gets  

        public async Task<IEnumerable<ChuyenMonKiThuat>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.ChuyenMonKiThuat.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<ChuyenMonKiThuat> Get(int id)
        {
            return await _dasNghiepVuRepo.ChuyenMonKiThuat.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexChuyenMonKiThuat> SearchByConditionPagging(ChuyenMonKiThuatCondition condition)
        {
            var model = new VMIndexChuyenMonKiThuat
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.ChuyenMonKiThuat.GetAll()
                       where (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword) || tb.Code.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMChuyenMonKiThuat>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            
            model.ChuyenMonKiThuats = new PaginatedList<VMChuyenMonKiThuat>(result, (int)total, condition.PageIndex, condition.PageSize);



            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateChuyenMonKiThuat> Create()
        {
            var model = new VMUpdateChuyenMonKiThuat()
            {
            };
            


            return model;
        }
        

        public async Task<ServiceResult> Save(VMUpdateChuyenMonKiThuat data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var chuyenMonKiThuat = Utils.Bind<ChuyenMonKiThuat>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.ChuyenMonKiThuat.InsertAsync(chuyenMonKiThuat);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm danh mục chuyên môm kỹ thuật thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới danh mục chuyên môm kỹ thuật");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateChuyenMonKiThuat> Update(int? id)
        {
            var chuyenMonKiThuat = await Get(id ?? 0);
            if (chuyenMonKiThuat == null || chuyenMonKiThuat.ID == 0)
            {
                throw new LogicException("Danh mục chuyên môm kỹ thuật không còn tồn tại");
            }
            var model = _mapper.Map<ChuyenMonKiThuat,VMUpdateChuyenMonKiThuat>(chuyenMonKiThuat);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateChuyenMonKiThuat vmChuyenMonKiThuat)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var chuyenMonKiThuat = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAsync(vmChuyenMonKiThuat.ID);
                if (chuyenMonKiThuat == null)
                    return new ServiceResultError("Danh mục chuyên môm kỹ thuật này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmChuyenMonKiThuat.ParentPath;


                await ValidateData(vmChuyenMonKiThuat);
                chuyenMonKiThuat.Bind(vmChuyenMonKiThuat.KeyValue());

                await _dasNghiepVuRepo.ChuyenMonKiThuat.UpdateAsync(chuyenMonKiThuat);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật danh mục chuyên môm kỹ thuật thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion


        #region Delete
        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                var ChuyenMonKiThuat = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAsync(id);
                if (ChuyenMonKiThuat == null)
                    return new ServiceResultError("Danh mục chuyên môm kỹ thuật này hiện không tồn tại hoặc đã bị xóa");

              
                await _dasNghiepVuRepo.ChuyenMonKiThuat.DeleteAsync(ChuyenMonKiThuat);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa danh mục chuyên môm kỹ thuật thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
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
                var ChuyenMonKiThuatDeletes = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllListAsync(n => ids.Contains(n.ID));
                if (ChuyenMonKiThuatDeletes == null || ChuyenMonKiThuatDeletes.Count() == 0)
                    return new ServiceResultError("Danh mục chuyên môm kỹ thuật đã chọn hiện không tồn tại hoặc đã bị xóa");

                await _dasNghiepVuRepo.ChuyenMonKiThuat.DeleteAsync(ChuyenMonKiThuatDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa danh mục chuyên môm kỹ thuật thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion


        #region Validate
        private async Task ValidateData(VMUpdateChuyenMonKiThuat vmChuyenMonKiThuat)
        {

            //if (await _dasNghiepVuRepo.ChuyenMonKiThuat.IsNameExist(vmChuyenMonKiThuat.Name, (int)EnumCommon.Status.InActive, vmChuyenMonKiThuat.ID))
            //{
            //    throw new LogicException("Tên danh mục chuyên môm kỹ thuật đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}