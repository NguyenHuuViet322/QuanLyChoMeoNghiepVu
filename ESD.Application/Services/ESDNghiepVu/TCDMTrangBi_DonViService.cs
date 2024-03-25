using AutoMapper;
using ESD.Application.Enums.DasKTNN;
using ESD.Application.Enums.ESDTieuChuanKiemDinh;
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
    public class TCDMTrangBi_DonViService : BaseMasterService, ITCDMTrangBi_DonViServices
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
        public TCDMTrangBi_DonViService(IDasRepositoryWrapper dasRepository
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

        public async Task<IEnumerable<TCDMTrangBi_DonVi>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.TCDMTrangBi_DonVi.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<TCDMTrangBi_DonVi> Get(int id)
        {
            return await _dasNghiepVuRepo.TCDMTrangBi_DonVi.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexTCDMTrangBi_DonVi> SearchByConditionPagging(TCDMTrangBi_DonViCondition condition)
        {
            var model = new VMIndexTCDMTrangBi_DonVi
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.TCDMTrangBi_DonVi.GetAll()
                           //where (condition.Keyword.IsEmpty() || tb.Name.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMTCDMTrangBi_DonVi>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            model.TCDMTrangBi_DonVis = new PaginatedList<VMTCDMTrangBi_DonVi>(result, (int)total, condition.PageIndex, condition.PageSize);

            return model;
        }

        public async Task<VMIndexTCDMTrangBi_DonVi> TongHopTieuChuan(TCDMTrangBi_DonViCondition condition)
        {

            var model = new VMIndexTCDMTrangBi_DonVi
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.TCDMTrangBi_DonVi.GetAll()
                       where (condition.NienHan  == 0 || tb.NienHan == condition.NienHan)
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMTCDMTrangBi_DonVi>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var trangBi_DonVis = new PaginatedList<VMTCDMTrangBi_DonVi>(result, (int)total, condition.PageIndex, condition.PageSize);

            var donVis = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync(n => n.PhanLoaiDonVi == (int)PhanLoaiDonVi.DVNghiepVu);
            var donViTimKiems = donVis.Where(n => condition.IDDonvi == 0 || condition.IDDonvi == n.ID).ToList();
            var soPhongQlMuiHoi = donViTimKiems.Sum(n => n.SoPhongLuuTruMuiHoi);
            var soPhongXlMuiHoi = donViTimKiems.Sum(n => n.SoPhongXuLyHoi);

            model.DonViTinhs = Utils.GetDescribes<DonViTinh>();
            model.LoaiPhongs = Utils.GetDescribes<LoaiPhong>();
            model.NienHans = Utils.GetDescribes<NienHan>().ToSelectList(condition.NienHan);

            foreach (var trangBi_DonVi in trangBi_DonVis)
            {
                if (trangBi_DonVi.MaPhong == (int)LoaiPhong.PhongQlMuiHoi)
                {
                    trangBi_DonVi.SoLuong = soPhongQlMuiHoi * trangBi_DonVi.SoLuong;
                    trangBi_DonVi.DuTru = trangBi_DonVi.DuTru / (double)100 * trangBi_DonVi.SoLuong;
                }
                else if (trangBi_DonVi.MaPhong == (int)LoaiPhong.PhongXlHoi)
                {
                    trangBi_DonVi.SoLuong = soPhongXlMuiHoi * trangBi_DonVi.SoLuong;
                    trangBi_DonVi.DuTru = trangBi_DonVi.DuTru / (double)100 * trangBi_DonVi.SoLuong;
                }
                else
                {
                    trangBi_DonVi.SoLuong = 0;
                    trangBi_DonVi.DuTru = 0;
                }

                trangBi_DonVi.StrNienHan = model.NienHans.FirstOrNewObj(n => n.Value == trangBi_DonVi.NienHan.ToString()).Text;
                trangBi_DonVi.StrDonViTinh = model.DonViTinhs.GetValueOrDefault(trangBi_DonVi.DonViTinh);
                trangBi_DonVi.StrPhong = model.LoaiPhongs.GetValueOrDefault(trangBi_DonVi.MaPhong);
            }

            model.DonViNghiepVus = donVis;
            model.TCDMTrangBi_DonVis = new PaginatedList<VMTCDMTrangBi_DonVi>(trangBi_DonVis, (int)total, condition.PageIndex, condition.PageSize);

            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateTCDMTrangBi_DonVi> Create()
        {
            var model = new VMUpdateTCDMTrangBi_DonVi()
            {
            };



            return model;
        }


        public async Task<ServiceResult> Save(VMUpdateTCDMTrangBi_DonVi data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var tCDMTrangBi_DonVi = Utils.Bind<TCDMTrangBi_DonVi>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.TCDMTrangBi_DonVi.InsertAsync(tCDMTrangBi_DonVi);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm tcdmtrangbi_donvi thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới tcdmtrangbi_donvi");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateTCDMTrangBi_DonVi> Update(int? id)
        {
            var tCDMTrangBi_DonVi = await Get(id ?? 0);
            if (tCDMTrangBi_DonVi == null || tCDMTrangBi_DonVi.ID == 0)
            {
                throw new LogicException("TCDMTrangBi_DonVi không còn tồn tại");
            }
            var model = _mapper.Map<TCDMTrangBi_DonVi, VMUpdateTCDMTrangBi_DonVi>(tCDMTrangBi_DonVi);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateTCDMTrangBi_DonVi vmTCDMTrangBi_DonVi)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var tCDMTrangBi_DonVi = await _dasNghiepVuRepo.TCDMTrangBi_DonVi.GetAsync(vmTCDMTrangBi_DonVi.ID);
                if (tCDMTrangBi_DonVi == null)
                    return new ServiceResultError("TCDMTrangBi_DonVi này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmTCDMTrangBi_DonVi.ParentPath;

                await ValidateData(vmTCDMTrangBi_DonVi);
                tCDMTrangBi_DonVi.Bind(vmTCDMTrangBi_DonVi.KeyValue());

                await _dasNghiepVuRepo.TCDMTrangBi_DonVi.UpdateAsync(tCDMTrangBi_DonVi);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật tcdmtrangbi_donvi thành công");
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
                var TCDMTrangBi_DonVi = await _dasNghiepVuRepo.TCDMTrangBi_DonVi.GetAsync(id);
                if (TCDMTrangBi_DonVi == null)
                    return new ServiceResultError("TCDMTrangBi_DonVi này hiện không tồn tại hoặc đã bị xóa");


                await _dasNghiepVuRepo.TCDMTrangBi_DonVi.DeleteAsync(TCDMTrangBi_DonVi);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdmtrangbi_donvi thành công");
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
                var TCDMTrangBi_DonViDeletes = await _dasNghiepVuRepo.TCDMTrangBi_DonVi.GetAllListAsync(n => ids.Contains(n.ID));
                if (TCDMTrangBi_DonViDeletes == null || TCDMTrangBi_DonViDeletes.Count() == 0)
                    return new ServiceResultError("TCDMTrangBi_DonVi đã chọn hiện không tồn tại hoặc đã bị xóa");

                await _dasNghiepVuRepo.TCDMTrangBi_DonVi.DeleteAsync(TCDMTrangBi_DonViDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdmtrangbi_donvi thành công");
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
        private async Task ValidateData(VMUpdateTCDMTrangBi_DonVi vmTCDMTrangBi_DonVi)
        {

            //if (await _dasNghiepVuRepo.TCDMTrangBi_DonVi.IsNameExist(vmTCDMTrangBi_DonVi.Name, (int)EnumCommon.Status.InActive, vmTCDMTrangBi_DonVi.ID))
            //{
            //    throw new LogicException("Tên tcdmtrangbi_donvi đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}