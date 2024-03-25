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
    public class LoaiChoNghiepVuService : BaseMasterService, ILoaiChoNghiepVuServices
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
        public LoaiChoNghiepVuService(IDasRepositoryWrapper dasRepository
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

        public async Task<IEnumerable<LoaiChoNghiepVu>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.LoaiChoNghiepVu.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<LoaiChoNghiepVu> Get(int id)
        {
            return await _dasNghiepVuRepo.LoaiChoNghiepVu.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexLoaiChoNghiepVu> SearchByConditionPagging(LoaiChoNghiepVuCondition condition)
        {
            var model = new VMIndexLoaiChoNghiepVu
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.LoaiChoNghiepVu.GetAll()
                       where (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword) || tb.Code.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMLoaiChoNghiepVu>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            
            model.LoaiChoNghiepVus = new PaginatedList<VMLoaiChoNghiepVu>(result, (int)total, condition.PageIndex, condition.PageSize);



            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateLoaiChoNghiepVu> Create()
        {
            var model = new VMUpdateLoaiChoNghiepVu()
            {
            };
            


            return model;
        }
        

        public async Task<ServiceResult> Save(VMUpdateLoaiChoNghiepVu data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var loaiChoNghiepVu = Utils.Bind<LoaiChoNghiepVu>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.LoaiChoNghiepVu.InsertAsync(loaiChoNghiepVu);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm loại chó nghiệp vụ thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới loại chó nghiệp vụ");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateLoaiChoNghiepVu> Update(int? id)
        {
            var loaiChoNghiepVu = await Get(id ?? 0);
            if (loaiChoNghiepVu == null || loaiChoNghiepVu.ID == 0)
            {
                throw new LogicException("Loại chó nghiệp vụ không còn tồn tại");
            }
            var model = _mapper.Map<LoaiChoNghiepVu,VMUpdateLoaiChoNghiepVu>(loaiChoNghiepVu);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateLoaiChoNghiepVu vmLoaiChoNghiepVu)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var loaiChoNghiepVu = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAsync(vmLoaiChoNghiepVu.ID);
                if (loaiChoNghiepVu == null)
                    return new ServiceResultError("Loại chó nghiệp vụ này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmLoaiChoNghiepVu.ParentPath;


                await ValidateData(vmLoaiChoNghiepVu);
                loaiChoNghiepVu.Bind(vmLoaiChoNghiepVu.KeyValue());

                await _dasNghiepVuRepo.LoaiChoNghiepVu.UpdateAsync(loaiChoNghiepVu);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật loại chó nghiệp vụ thành công");
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
                var LoaiChoNghiepVu = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAsync(id);
                var DongVatNghiepVu = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync();
                if (LoaiChoNghiepVu == null)
                    return new ServiceResultError("Loại chó nghiệp vụ này hiện không tồn tại hoặc đã bị xóa");

                if (DongVatNghiepVu.Where(p => p.IDLoaiChoNghiepVu == id).Count() !=0 )
                    return new ServiceResultError("Loại chó nghiệp vụ đã được sử dụng");

                await _dasNghiepVuRepo.LoaiChoNghiepVu.DeleteAsync(LoaiChoNghiepVu);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa loại chó nghiệp vụ thành công");
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
                var LoaiChoNghiepVuDeletes = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAllListAsync(n => ids.Contains(n.ID));
                var DongVatNghiepVu = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync();

                if (LoaiChoNghiepVuDeletes == null || LoaiChoNghiepVuDeletes.Count() == 0)
                    return new ServiceResultError("Loại chó nghiệp vụ đã chọn hiện không tồn tại hoặc đã bị xóa");

                foreach (var loaiChoNghiepVu in LoaiChoNghiepVuDeletes)
                    if (DongVatNghiepVu.Where(p => p.IDLoaiChoNghiepVu == loaiChoNghiepVu.ID).Count() != 0)
                        return new ServiceResultError("Tồn tại loại chó nghiệp vụ đã được sử dụng");

                await _dasNghiepVuRepo.LoaiChoNghiepVu.DeleteAsync(LoaiChoNghiepVuDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa loại chó nghiệp vụ thành công");
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
        private async Task ValidateData(VMUpdateLoaiChoNghiepVu vmLoaiChoNghiepVu)
        {

            //if (await _dasNghiepVuRepo.LoaiChoNghiepVu.IsNameExist(vmLoaiChoNghiepVu.Name, (int)EnumCommon.Status.InActive, vmLoaiChoNghiepVu.ID))
            //{
            //    throw new LogicException("Tên loại chó nghiệp vụ đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}