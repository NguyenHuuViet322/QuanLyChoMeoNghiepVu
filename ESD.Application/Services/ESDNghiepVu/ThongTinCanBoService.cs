using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
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
using ESD.Infrastructure.ContextAccessors;
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
    public class ThongTinCanBoService : BaseMasterService, IThongTinCanBoServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IModuleService _module;
        private readonly IDefaultDataService _defaultDataService;
        private readonly IHostApplicationLifetime _host;
        private readonly IUserPrincipalService _userPrincipalService;
        private ICacheManagementServices _cacheManagementServices;
        private IWebHostEnvironment _env;

        #endregion

        #region Ctor
        public ThongTinCanBoService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IModuleService module
            , IDefaultDataService defaultDataService
            , IHostApplicationLifetime host
            , IUserPrincipalService userPrincipalService
            , ICacheManagementServices cacheManagementServices, IESDNghiepVuRepositoryWrapper dasNghiepVuRepo,
            IWebHostEnvironment env) : base(dasRepository, dasNghiepVuRepo)
        {
            _mapper = mapper;
            _logger = logger;
            _module = module;
            _cacheManagementServices = cacheManagementServices;
            _defaultDataService = defaultDataService;
            _userPrincipalService = userPrincipalService;
            _host = host;
            _env = env;
        }


        #endregion

        #region Gets  

        public async Task<IEnumerable<ThongTinCanBo>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.ThongTinCanBo.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<ThongTinCanBo> Get(int id)
        {
            return await _dasNghiepVuRepo.ThongTinCanBo.FirstOrDefaultAsync(n => n.ID == id);
        }
        public async Task<VMIndexThongTinCanBo> SearchByConditionPagging(ThongTinCanBoCondition condition)
        {
            var model = new VMIndexThongTinCanBo();
            var lstVMThongTinCanBo = new List<VMThongTinCanBo>();
            var temp = from tb in _dasNghiepVuRepo.ThongTinCanBo.GetAll()
                       where ((condition.Keyword.IsEmpty() || tb.TenCanBo.Contains(condition.Keyword) || tb.Code.Contains(condition.Keyword)) 
                                && (condition.IDDonViNghiepVu <= 0 || tb.IDDonViNghiepVu == condition.IDDonViNghiepVu)
                                && (condition.IDChuyenMonKiThuat <= 0 || tb.IDChuyenMonKiThuat == condition.IDChuyenMonKiThuat))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMThongTinCanBo>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            model.SearchParam = condition;
            model.ThongTinCanBos = new PaginatedList<VMThongTinCanBo>(result, (int)total, condition.PageIndex, condition.PageSize) ?? new PaginatedList<VMThongTinCanBo>();
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.ChuyenMonKiThuats = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllListAsync();
            model.LoaiCanBos = Utils.GetDescribes<TypeCanBo>();
            
            foreach (var item in model.ThongTinCanBos)
            {
                var dogs = _dasNghiepVuRepo.DongVatNghiepVu.GetAllList(x => x.IDThongTinCanBo == item.ID && x.PhanLoai == (int)PhanLoaiDVNV.BinhThuong) ?? new List<DongVatNghiepVu>();
                var unit = model.DonViNghiepVus.FirstOrDefault(x => x.ID == item.IDDonViNghiepVu) ?? new DonViNghiepVu();
                var specialize = model.ChuyenMonKiThuats.FirstOrDefault(x => x.ID == item.IDChuyenMonKiThuat) ?? new ChuyenMonKiThuat();
                //item.BirthDay = item.NgaySinh.ToString("dd/MM/yyyy");
                item.TenDonViNghiepVu = unit.Ten ?? "";
                item.TenChuyenMonKyThuat = specialize.Ten ?? "";
                item.Gender = item.GioiTinh == 1 ? "Nam" : "Nữ";
                item.Gender = item.GioiTinh == 1 ? "Nam" : "Nữ";
                item.SoLuongCho = dogs.Count();
            }
            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateThongTinCanBo> Create()
        {
            var model = new VMUpdateThongTinCanBo()
            {
            };

            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.ChuyenMonKiThuats = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllListAsync();

            return model;
        }

        public async Task<ServiceResult> Save(VMUpdateThongTinCanBo data)
        {
            try
            {
                if (!CheckValidate(data, out string mss))
                    return new ServiceResultError(mss);

                var thongTinCanBo = Utils.Bind<ThongTinCanBo>(data.KeyValue());

                thongTinCanBo.CreateDate = DateTime.Now;
                thongTinCanBo.CreatedBy = _userPrincipalService.UserId;
                thongTinCanBo.NgaySinh = DateTime.Now;
                await _dasNghiepVuRepo.ThongTinCanBo.InsertAsync(thongTinCanBo);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm thông tin cán bộ thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới thông tin cán bộ");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateThongTinCanBo> Update(int? id)
        {
            var thongTinCanBo = await Get(id ?? 0);
            if (thongTinCanBo == null || thongTinCanBo.ID == 0)
                throw new LogicException("Thông tin cán bộ không còn tồn tại");

            var model = _mapper.Map<ThongTinCanBo, VMUpdateThongTinCanBo>(thongTinCanBo);
            var dogs = _dasNghiepVuRepo.DongVatNghiepVu.GetAllList(x => x.IDThongTinCanBo == id && x.PhanLoai == (int)PhanLoaiDVNV.BinhThuong) ?? new List<DongVatNghiepVu>();
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.ChuyenMonKiThuats = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllListAsync();
            model.SoLuongCho = dogs.Count();
            return model;
        }
        public async Task<ServiceResult> Change(VMUpdateThongTinCanBo vmThongTinCanBo)
        {
            try
            {
                if (!CheckValidate(vmThongTinCanBo, out string mss))
                    return new ServiceResultError(mss);

                var thongTinCanBo = await _dasNghiepVuRepo.ThongTinCanBo.GetAsync(vmThongTinCanBo.ID);
                if (thongTinCanBo == null)
                    return new ServiceResultError("Thông tin cán bộ này hiện không tồn tại hoặc đã bị xóa");

                thongTinCanBo.Bind(vmThongTinCanBo.KeyValue());

                thongTinCanBo.UpdatedDate = DateTime.Now;
                thongTinCanBo.UpdatedBy = _userPrincipalService.UserId;
                await _dasNghiepVuRepo.ThongTinCanBo.UpdateAsync(thongTinCanBo);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật thông tin cán bộ thành công");
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
                var ThongTinCanBo = await _dasNghiepVuRepo.ThongTinCanBo.GetAsync(id);
                if (ThongTinCanBo == null)
                    return new ServiceResultError("Thông tin cán bộ này hiện không tồn tại hoặc đã bị xóa");

                var checkHuanLuyen = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(x => x.IDThongTinCanBo == id);
                if (checkHuanLuyen.Count() > 0)
                    return new ServiceResultError("Không được xóa cán bộ đang quản lý động vật");

                await _dasNghiepVuRepo.ThongTinCanBo.DeleteAsync(ThongTinCanBo);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa thông tin cán bộ thành công");
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
                var ThongTinCanBoDeletes = await _dasNghiepVuRepo.ThongTinCanBo.GetAllListAsync(n => ids.Contains(n.ID));
                if (ThongTinCanBoDeletes == null || ThongTinCanBoDeletes.Count() == 0)
                    return new ServiceResultError("Thông tin cán bộ đã chọn hiện không tồn tại hoặc đã bị xóa");

                var checkHuanLuyen = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(x => ids.Contains(x.IDThongTinCanBo.Value));
                if (checkHuanLuyen.Count() > 0)
                    return new ServiceResultError("Không được xóa cán bộ đang quản lý động vật");

                await _dasNghiepVuRepo.ThongTinCanBo.DeleteAsync(ThongTinCanBoDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa thông tin cán bộ thành công");
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
        private bool CheckValidate(VMUpdateThongTinCanBo vmThongTinCanBo, out string mss)
        {
            mss = string.Empty;
            //if (vmThongTinCanBo.NgaySinh >= DateTime.Now)
            //    mss = "Ngày sinh không được lớn hơn ngày hiện tại";
            if (vmThongTinCanBo.IDDonViNghiepVu <= 0)
                mss = "Đơn vị nghiệp vụ không được để trống";
            if (vmThongTinCanBo.IDChuyenMonKiThuat <= 0)
                mss = "Chuyên môn kỹ thuật không được để trống";
            //var rs = _dasNghiepVuRepo.ThongTinCanBo.GetAllList(x => x.Code.ToUpper() == vmThongTinCanBo.Code && x.ID != vmThongTinCanBo.ID);
            //if (rs.Count() > 0)
            //    mss = "Mã cán bộ đã tồn tại trong hệ thống";
            return mss.IsEmpty();
        }
        #endregion
    }
}