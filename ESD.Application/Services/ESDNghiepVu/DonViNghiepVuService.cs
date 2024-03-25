using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.VariantTypes;
using ESD.Application.Enums.DasKTNN;
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
    public class DonViNghiepVuService : BaseMasterService, IDonViNghiepVuServices
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
        public DonViNghiepVuService(IDasRepositoryWrapper dasRepository
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

        public async Task<IEnumerable<DonViNghiepVu>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.DonViNghiepVu.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<DonViNghiepVu> Get(int id)
        {
            return await _dasNghiepVuRepo.DonViNghiepVu.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexDonViNghiepVu> SearchByConditionPagging(DonViNghiepVuCondition condition)
        {
            var model = new VMIndexDonViNghiepVu
            {
                SearchParam = condition
            };

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.DonViNghiepVu.GetAll()
                       where (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword))
                       && (condition.PhanLoai == 0 ||  condition.PhanLoai == tb.PhanLoaiDonVi)
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMDonViNghiepVu>(tb);

            var lstDongVat = _dasNghiepVuRepo.DongVatNghiepVu.GetAll();
            var lstCanBo = _dasNghiepVuRepo.ThongTinCanBo.GetAll();
            var CSVC_DonVi = _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll();

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            foreach (var item in result)
            {
                item.SoDongVatNghiepVu = lstDongVat.Where(p => p.IDDonViQuanLy == item.ID && p.PhanLoai == (int)PhanLoaiDVNV.BinhThuong).Count();
                item.SoCanBo = lstCanBo.Where(p => p.IDDonViNghiepVu == item.ID).Count();
                item.SoCSVC = CSVC_DonVi.Where(p => p.IDDonViNghiepVu == item.ID).Count();
            }

            model.DonViNghiepVus = new PaginatedList<VMDonViNghiepVu>(result, (int)total, condition.PageIndex, condition.PageSize);

            return model;
        }

        public async Task<VMReportDonViNghiepVu> SearchReportByConditionPagging(DonViNghiepVuCondition condition)
        {
            var model = new VMReportDonViNghiepVu
            {
                SearchParam = condition
            };

            if (condition.PhanLoai != (int)PhanLoaiDonVi.TraiGiam)
                condition.PhanLoai = (int)PhanLoaiDonVi.DVNghiepVu;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.DonViNghiepVu.GetAll()
                       where (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword))
                       && (condition.PhanLoai == tb.PhanLoaiDonVi)
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMDonViNghiepVu>(tb);

            var lstDongVat = _dasNghiepVuRepo.DongVatNghiepVu.GetAll();
            var lstCanBo = _dasNghiepVuRepo.ThongTinCanBo.GetAll();
            var CSVC_DonVi = _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll();


            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            foreach (var item in result)
            {
                item.SoDongVatNghiepVu = lstDongVat.Where(p => p.IDDonViQuanLy == item.ID && p.PhanLoai == (int)PhanLoaiDVNV.BinhThuong).Count();
                item.Chet = lstDongVat.Where(p => (p.IDDonViQuanLy == item.ID && p.PhanLoai == (int)PhanLoaiDVNV.Mat)).Count();
                item.ThaiLoai = lstDongVat.Where(p => (p.IDDonViQuanLy == item.ID && p.PhanLoai == (int)PhanLoaiDVNV.Loai)).Count();
            }

            model.DonViNghiepVus = new PaginatedList<VMDonViNghiepVu>(result, (int)total, condition.PageIndex, condition.PageSize);

            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateDonViNghiepVu> Create(int phanLoai)
        {
            if (phanLoai != (int)PhanLoaiDonVi.TraiGiam)
                phanLoai = (int)PhanLoaiDonVi.DVNghiepVu;


            var model = new VMUpdateDonViNghiepVu()
            {
                PhanLoaiDonVi = phanLoai
            };
            return model;
        }


        public async Task<ServiceResult> Save(VMUpdateDonViNghiepVu data)
        {
            var tenPhanLoai = GetTenPhanLoai(data.PhanLoaiDonVi).ToLower();
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var donViNghiepVu = Utils.Bind<DonViNghiepVu>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.DonViNghiepVu.InsertAsync(donViNghiepVu);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess($"Thêm {tenPhanLoai} thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError($"Có lỗi khi thêm mới {tenPhanLoai}");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateDonViNghiepVu> Update(int? id)
        {
            var donViNghiepVu = await Get(id ?? 0);
            if (donViNghiepVu == null || donViNghiepVu.ID == 0)
            {
                throw new LogicException("Dữ liệu không còn tồn tại");
            }
            var model = _mapper.Map<DonViNghiepVu, VMUpdateDonViNghiepVu>(donViNghiepVu);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateDonViNghiepVu data)
        {
            var tenPhanLoai = GetTenPhanLoai(data.PhanLoaiDonVi);
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var donViNghiepVu = await _dasNghiepVuRepo.DonViNghiepVu.GetAsync(data.ID);
                if (donViNghiepVu == null)
                    return new ServiceResultError($"{tenPhanLoai} này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmDonViNghiepVu.ParentPath;

                await ValidateData(data);
                donViNghiepVu.Bind(data.KeyValue());
                await _dasNghiepVuRepo.DonViNghiepVu.UpdateAsync(donViNghiepVu);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess($"Cập nhật {tenPhanLoai} thành công");
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
        public async Task<ServiceResult> Delete(long id)
        {
            try
            {
                var DonViNghiepVu = await _dasNghiepVuRepo.DonViNghiepVu.GetAsync(id);
                if (DonViNghiepVu == null)
                    return new ServiceResultError("Đơn vị nghiệp vụ này hiện không tồn tại hoặc đã bị xóa");

                var isUsed = await _dasNghiepVuRepo.DongVatNghiepVu.AnyAsync(n => n.IDDonViQuanLy == id || n.IDDonViNghiepVu == id);
                if (isUsed)
                    return new ServiceResultError("Đơn vị nghiệp vụ này hiện đang được sử dụng, không được phép xoá");

                await _dasNghiepVuRepo.DonViNghiepVu.DeleteAsync(DonViNghiepVu);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa đơn vị nghiệp vụ thành công");
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
        public async Task<ServiceResult> Delete(IEnumerable<long> ids)
        {
            try
            {
                var donViNghiepVuDeletes = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync(n => ids.Contains(n.ID));
                if (donViNghiepVuDeletes == null || donViNghiepVuDeletes.Count() == 0)
                    return new ServiceResultError("Đơn vị nghiệp vụ đã chọn hiện không tồn tại hoặc đã bị xóa");


                var categoryUsed = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(n => ids.Contains(n.IDDonViQuanLy ?? 0) || ids.Contains(n.IDDonViNghiepVu ?? 0));
                if (categoryUsed.IsNotEmpty())
                {
                    var idDonViQls = categoryUsed.Where(n => n.IDDonViQuanLy > 0).Select(n => n.IDDonViQuanLy.Value).Distinct().ToList();

                    var iidDonViNVs = categoryUsed.Where(n => n.IDDonViNghiepVu > 0).Select(n => n.IDDonViNghiepVu.Value).Distinct().ToList();

                    var deletedNames = donViNghiepVuDeletes.Where(m => idDonViQls.Contains(m.ID) || iidDonViNVs.Contains(m.ID)).Select(n => n.Ten);
                    return new ServiceResultError("Đơn vị nghiệp vụ " + string.Join(", ", deletedNames) + " hiện đang được sử dụng, không được phép xoá");
                }

                await _dasNghiepVuRepo.DonViNghiepVu.DeleteAsync(donViNghiepVuDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa đơn vị nghiệp vụ thành công");
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
        private async Task ValidateData(VMUpdateDonViNghiepVu vmDonViNghiepVu)
        {
            var tenPhanLoai = GetTenPhanLoai(vmDonViNghiepVu.PhanLoaiDonVi).ToLower();
            
            if (await _dasNghiepVuRepo.DonViNghiepVu.AnyAsync(n => n.Ten.ToLower() == vmDonViNghiepVu.Ten.ToLower()
            && n.PhanLoaiDonVi == vmDonViNghiepVu.PhanLoaiDonVi
            && n.ID != vmDonViNghiepVu.ID))
                {
                    throw new LogicException($"Tên {tenPhanLoai} đã tồn tại");
                }

            if (vmDonViNghiepVu.Code != null)
                if (await _dasNghiepVuRepo.DonViNghiepVu.AnyAsync(n => n.Code.ToLower() == vmDonViNghiepVu.Code.ToLower()
            && n.PhanLoaiDonVi == vmDonViNghiepVu.PhanLoaiDonVi
            && n.ID != vmDonViNghiepVu.ID))
            {
                throw new LogicException("Mã định danh  đã tồn tại");
            }
        }
        #endregion

        #region Funtions

        private string GetTenPhanLoai(int phanloai)
        {
            if (phanloai == (int)PhanLoaiDonVi.TraiGiam)
                return "Trại giam";
            return "Đơn vị nghiệp vụ";
        }

        #endregion

        #region Export
        public async Task<VMExportReportDonViNghiepVu> ExportReport(int id)
        {
            var donViNghiepVu = await Get(id);
            if (donViNghiepVu == null || donViNghiepVu.ID == 0)
            {
                throw new LogicException("Dữ liệu không còn tồn tại");
            }
            var donvi = _mapper.Map<DonViNghiepVu, VMUpdateDonViNghiepVu>(donViNghiepVu);
            var dongVats = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(n => n.IDDonViQuanLy == id && n.PhanLoai == (int)PhanLoaiDVNV.BinhThuong);
            var canBos = await _dasNghiepVuRepo.ThongTinCanBo.GetAllListAsync(n => n.IDDonViNghiepVu == id);
            var chuyenMonKiThuats = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllListAsync();

            var loaiChoNghiepVus = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAllListAsync();
            var nghiepVuDongVats = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync();

            var model = new VMExportReportDonViNghiepVu
            {
                DonVi = donvi,
                DongVats = _mapper.Map<IEnumerable<DongVatNghiepVu>, IEnumerable<VMDongVatNghiepVu>>(dongVats),
                CanBos = _mapper.Map<IEnumerable<ThongTinCanBo>, IEnumerable<VMThongTinCanBo>>(canBos),
                ChuyenMonKiThuats = _mapper.Map<IEnumerable<ChuyenMonKiThuat>, IEnumerable<VMChuyenMonKiThuat>>(chuyenMonKiThuats),
                NghiepVuDongVats = _mapper.Map<IEnumerable<NghiepVuDongVat>, IEnumerable<VMNghiepVuDongVat>>(nghiepVuDongVats),
                LoaiChoNghiepVus = _mapper.Map<IEnumerable<LoaiChoNghiepVu>, IEnumerable<VMLoaiChoNghiepVu>>(loaiChoNghiepVus),
            };
            return model;
        }

        #endregion
    }
}