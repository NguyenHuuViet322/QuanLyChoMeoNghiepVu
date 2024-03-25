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
    public class NghiepVuDongVatService : BaseMasterService, INghiepVuDongVatServices
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
        public NghiepVuDongVatService(IDasRepositoryWrapper dasRepository
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

        public async Task<IEnumerable<NghiepVuDongVat>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.NghiepVuDongVat.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<NghiepVuDongVat> Get(int id)
        {
            return await _dasNghiepVuRepo.NghiepVuDongVat.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexNghiepVuDongVat> SearchByConditionPagging(NghiepVuDongVatCondition condition)
        {
            var model = new VMIndexNghiepVuDongVat
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.NghiepVuDongVat.GetAll()
                       where (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword) || tb.Code.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMNghiepVuDongVat>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            
            model.NghiepVuDongVats = new PaginatedList<VMNghiepVuDongVat>(result, (int)total, condition.PageIndex, condition.PageSize);



            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateNghiepVuDongVat> Create()
        {
            var model = new VMUpdateNghiepVuDongVat()
            {
            };
            


            return model;
        }
        

        public async Task<ServiceResult> Save(VMUpdateNghiepVuDongVat data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var nghiepVuDongVat = Utils.Bind<NghiepVuDongVat>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.NghiepVuDongVat.InsertAsync(nghiepVuDongVat);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm nghiệp vụ động vật thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới nghiệp vụ động vật");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateNghiepVuDongVat> Update(int? id)
        {
            var nghiepVuDongVat = await Get(id ?? 0);
            if (nghiepVuDongVat == null || nghiepVuDongVat.ID == 0)
            {
                throw new LogicException("Nghiệp vụ động vật không còn tồn tại");
            }
            var model = _mapper.Map<NghiepVuDongVat,VMUpdateNghiepVuDongVat>(nghiepVuDongVat);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateNghiepVuDongVat vmNghiepVuDongVat)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var nghiepVuDongVat = await _dasNghiepVuRepo.NghiepVuDongVat.GetAsync(vmNghiepVuDongVat.ID);
                if (nghiepVuDongVat == null)
                    return new ServiceResultError("Nghiệp vụ động vật này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmNghiepVuDongVat.ParentPath;

                await ValidateData(vmNghiepVuDongVat);
                nghiepVuDongVat.Bind(vmNghiepVuDongVat.KeyValue());

                await _dasNghiepVuRepo.NghiepVuDongVat.UpdateAsync(nghiepVuDongVat);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật nghiệp vụ động vật thành công");
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
                var NghiepVuDongVat = await _dasNghiepVuRepo.NghiepVuDongVat.GetAsync(id);
                var DongVatNghiepVu = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync();

                if (NghiepVuDongVat == null)
                    return new ServiceResultError("Nghiệp vụ động vật này hiện không tồn tại hoặc đã bị xóa");

                if (DongVatNghiepVu.Where(p => p.IDNghiepVuDongVat == id).Count() != 0)
                    return new ServiceResultError("Nghiệp vụ động vật này đã được sử dụng");

                await _dasNghiepVuRepo.NghiepVuDongVat.DeleteAsync(NghiepVuDongVat);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa nghiệp vụ động vật thành công");
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
                var NghiepVuDongVatDeletes = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync(n => ids.Contains(n.ID));
                var DongVatNghiepVu = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync();

                if (NghiepVuDongVatDeletes == null || NghiepVuDongVatDeletes.Count() == 0)
                    return new ServiceResultError("Nghiệp vụ động vật đã chọn hiện không tồn tại hoặc đã bị xóa");

                foreach (var nghiepVuDongVat in NghiepVuDongVatDeletes)
                    if (DongVatNghiepVu.Where(p => p.IDNghiepVuDongVat == nghiepVuDongVat.ID).Count() != 0)
                        return new ServiceResultError("Tồn tại nghiệp vụ động vật đã được sử dụng");

                await _dasNghiepVuRepo.NghiepVuDongVat.DeleteAsync(NghiepVuDongVatDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa nghiệp vụ động vật thành công");
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
        private async Task ValidateData(VMUpdateNghiepVuDongVat vmNghiepVuDongVat)
        {

            //if (await _dasNghiepVuRepo.NghiepVuDongVat.IsNameExist(vmNghiepVuDongVat.Name, (int)EnumCommon.Status.InActive, vmNghiepVuDongVat.ID))
            //{
            //    throw new LogicException("Tên nghiệp vụ động vật đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}