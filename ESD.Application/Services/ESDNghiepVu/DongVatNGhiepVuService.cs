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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services.ESDNghiepVu
{
    public class DongVatNghiepVuService : BaseMasterService, IDongVatNghiepVuServices
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
        public DongVatNghiepVuService(IDasRepositoryWrapper dasRepository
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

        public async Task<IEnumerable<DongVatNghiepVu>> GetsList(int? type, long id = 0)
        {
            var temp = from ct in _dasNghiepVuRepo.DongVatNghiepVu.GetAll()
                       where (!type.HasValue || type == ct.PhanLoai || ct.ID == id)
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<DongVatNghiepVu> Get(long id)
        {
            return await _dasNghiepVuRepo.DongVatNghiepVu.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexDongVatNghiepVu> SearchByConditionPagging(DongVatNghiepVuCondition condition)
        {
            var model = new VMIndexDongVatNghiepVu
            {
                SearchParam = condition
            };
            if (condition.PhanLoai.IsNotEmpty())
                condition.PhanLoai = condition.PhanLoai.Where(n => n > 0).ToArray();

            var isKhaiBaoMat = condition.Type == (int)MenuDVNV.Loai;
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.DongVatNghiepVu.GetAll()
                       where
                       (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword) || tb.Code.Contains(condition.Keyword))
                      && ((condition.IDDonViQuanLy ?? 0) == 0 || tb.IDDonViQuanLy == condition.IDDonViQuanLy)
                      && ((condition.Loai ?? 0) == 0 || tb.PhanLoai == condition.Loai)
                      && ((condition.IDCanBo ?? 0) == 0 || tb.IDThongTinCanBo == condition.IDCanBo)
                       && (condition.PhanLoai.IsEmpty() || condition.PhanLoai.Contains(tb.PhanLoai))   //Lọc theo phân loại
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMDongVatNghiepVu>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            model.DongVatNghiepVus = new PaginatedList<VMDongVatNghiepVu>(result, (int)total, condition.PageIndex, condition.PageSize);
            model.LoaiChoNghiepVus = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAllListAsync();
            model.ThongTinCanBos = await GetCanBoHuanLuyen();
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.IsKhaiBaoMat = isKhaiBaoMat;
            return model;
        }
          
        public async Task<VMReportDongVatNghiepVu> SearchReportByConditionPagging(DongVatMat_BiLoaiCondition condition)
        {
            var model = new VMReportDongVatNghiepVu
            {
                SearchParam = condition
            };

            var isKhaiBaoMat = condition.Type == (int)MenuDVNV.Loai;
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.DongVatNghiepVu.GetAll()
                       where tb.PhanLoai != (int)PhanLoaiDVNV.BinhThuong
                       && ((condition.IDDonViQuanLy == null) || (condition.IDDonViQuanLy == 0) || (condition.IDDonViQuanLy == tb.IDDonViQuanLy))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMDongVatNghiepVu>(tb);

            var listDV = temp.ToList();
            foreach (var item in listDV)
            {
                if (item.KhaiBaoDate != null)
                    item.khaiBaoDateStr = ((DateTime)item.KhaiBaoDate).ToString("yyyy");
            }
            if (condition.Year != null && condition.Year.Length == 4)
                listDV = listDV.Where(p => p.khaiBaoDateStr == condition.Year).ToList();

            var total = listDV.LongCount();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = listDV.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();

            var lstDv = temp.ToList();
            List<string> lstYear = new List<string>();
            foreach (var item in lstDv)
            {
                string year = DateTime.Now.ToString("yyyy");
                if (item.KhaiBaoDate != null)
                    year = ((DateTime)item.KhaiBaoDate).ToString("yyyy");
                if (!lstYear.Contains(year))
                {
                    lstYear.Add(year);
                };
            }

            model.Years = lstYear;
            model.DongVatNghiepVus = new PaginatedList<VMDongVatNghiepVu>(result, (int)total, condition.PageIndex, condition.PageSize);
            model.LoaiChoNghiepVus = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAllListAsync();
            model.ThongTinCanBos = await GetCanBoHuanLuyen();
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.NghiepVuDongVats = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync();
            model.IsKhaiBaoMat = isKhaiBaoMat;
            return model;
        }
        public async Task<IEnumerable<ThongTinCanBo>> GetCanBoHuanLuyen(long? idDonvi = null)
        {
            var cmkt_hd = await _dasNghiepVuRepo.ChuyenMonKiThuat.FirstOrDefaultAsync(n => n.Code == ConfigUtils.GetAppSetting<string>("CMKT_HuongDan"));
            if (cmkt_hd == null || cmkt_hd.ID == 0)
                return new List<ThongTinCanBo>();

            return await _dasNghiepVuRepo.ThongTinCanBo
                .GetAllListAsync(n => n.IDChuyenMonKiThuat == cmkt_hd.ID && (idDonvi == null || n.IDDonViNghiepVu == idDonvi));
        }
        #endregion

        #region Create

        public async Task<VMUpdateDongVatNghiepVu> Create(int type)
        {
            var isKhaiBaoMat = type == (int)MenuDVNV.Loai;
            var model = new VMUpdateDongVatNghiepVu()
            {
                PhanLoai = (int)PhanLoaiDVNV.BinhThuong
            };

            model.LoaiChoNghiepVus = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAllListAsync();
            model.NghiepVuDongVats = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync();
            model.ThongTinCanBos = new List<ThongTinCanBo>(); //await GetCanBoHuanLuyen(); //Được lấy từ danh mục đơn vị quản lý
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync(n => n.PhanLoaiDonVi == (int)PhanLoaiDonVi.DVNghiepVu);
            model.IsKhaiBaoMat = isKhaiBaoMat;
            if (isKhaiBaoMat)
            {
                model.DongVats = await GetsList((int)PhanLoaiDVNV.BinhThuong);
            }
            else
            {
                model.DongVats = await GetsList(null);
            }
            return model;
        }


        public async Task<ServiceResult> Save(VMUpdateDongVatNghiepVu data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var dongVatNghiepVu = Utils.Bind<DongVatNghiepVu>(data.KeyValue());
                await ValidateData(data);
                dongVatNghiepVu.NgaySinh = Utils.GetDate(data.StrNgaySinh);
                if (dongVatNghiepVu.IDDonViNghiepVu == null)
                    dongVatNghiepVu.IDDonViNghiepVu = 0;
                if (dongVatNghiepVu.IDDonViQuanLy == null)
                    dongVatNghiepVu.IDDonViQuanLy = 0;
                if(dongVatNghiepVu.IDNghiepVuDongVat == null)
                    dongVatNghiepVu.IDNghiepVuDongVat = 0;

                await _dasNghiepVuRepo.DongVatNghiepVu.InsertAsync(dongVatNghiepVu);
                await _dasNghiepVuRepo.SaveAync();

                if (dongVatNghiepVu.ID > 0)
                {
                    await SaveFileDinhKem(dongVatNghiepVu, data);
                    await _dasNghiepVuRepo.SaveAync();

                }

                return new ServiceResultSuccess("Thêm thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới");
            }
        }


        #endregion

        #region Update
        public async Task<VMUpdateDongVatNghiepVu> Update(int type, long? id)
        {
            var dongVatNghiepVu = await Get(id ?? 0);
            if (dongVatNghiepVu == null || dongVatNghiepVu.ID == 0)
            {
                throw new LogicException("Động vật nghiệp vụ không còn tồn tại");
            }
            var model = _mapper.Map<DongVatNghiepVu, VMUpdateDongVatNghiepVu>(dongVatNghiepVu);

            var isKhaiBaoMat = type == (int)MenuDVNV.Loai;
            model.LoaiChoNghiepVus = await _dasNghiepVuRepo.LoaiChoNghiepVu.GetAllListAsync();
            model.NghiepVuDongVats = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync();
            model.ThongTinCanBos = await GetCanBoHuanLuyen(dongVatNghiepVu.IDDonViQuanLy); //Chỉ lấy cán bộ thuộc đơn vi quản lý hiện tại
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync(n => n.PhanLoaiDonVi == (int)PhanLoaiDonVi.DVNghiepVu);
            model.DinhKems = await GetFileDinhKem(dongVatNghiepVu.ID);
            model.IsKhaiBaoMat = isKhaiBaoMat;
            if (isKhaiBaoMat)
            {
                model.IsUpdateKhaiBaoMat = dongVatNghiepVu.PhanLoai != (int)PhanLoaiDVNV.BinhThuong;

                if (model.PhanLoai == (int)PhanLoaiDVNV.BinhThuong)
                {
                    model.PhanLoai = (int)PhanLoaiDVNV.Loai;
                }
                model.DongVats = await GetsList((int)PhanLoaiDVNV.BinhThuong, id ?? 0);

            }
            else
            {
                model.DongVats = await GetsList(null);
            }
            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateDongVatNghiepVu data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var dongVatNghiepVu = await _dasNghiepVuRepo.DongVatNghiepVu.GetAsync(data.ID);
                if (dongVatNghiepVu == null)
                    return new ServiceResultError("Động vật nghiệp vụ này hiện không tồn tại hoặc đã bị xóa");

                await ValidateData(data);
                dongVatNghiepVu.Bind(data.KeyValue());
                dongVatNghiepVu.NgaySinh = Utils.GetDate(data.StrNgaySinh);
                dongVatNghiepVu.KhaiBaoDate = Utils.GetDate(data.StrKhaiBaoDate);

                if (dongVatNghiepVu.ID > 0)
                {
                    await SaveFileDinhKem(dongVatNghiepVu, data);
                }
                if (dongVatNghiepVu.PhanLoai == (int)PhanLoaiDVNV.BinhThuong)
                    dongVatNghiepVu.LyDo = string.Empty;

                await _dasNghiepVuRepo.DongVatNghiepVu.UpdateAsync(dongVatNghiepVu);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess(data.IsNew == 1 ? "Thêm mới thành công" : "Cập nhật thành công");
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

        public async Task<ServiceResult> LuuDieuChuyen(VMUpdateDongVatNghiepVu data)
        {
            try
            {
                var dongVatNghiepVu = await _dasNghiepVuRepo.DongVatNghiepVu.GetAsync(data.ID);
                if (dongVatNghiepVu == null)
                    return new ServiceResultError("Động vật nghiệp vụ này hiện không tồn tại hoặc đã bị xóa");

                dongVatNghiepVu.LyDoDieuChuyen = data.LyDoDieuChuyen;
                dongVatNghiepVu.IDDonViQuanLy = data.IDDonViQuanLy;

                await _dasNghiepVuRepo.DongVatNghiepVu.UpdateAsync(dongVatNghiepVu);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Điều chuyển thành công");
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
        public async Task<ServiceResult> Delete(int type, long id)
        {
            try
            {
                var dongVat = await _dasNghiepVuRepo.DongVatNghiepVu.GetAsync(id);
                if (dongVat == null)
                    return new ServiceResultError("Động vật nghiệp vụ này hiện không tồn tại hoặc đã bị xóa");

                if (dongVat.PhanLoai != (int)PhanLoaiDVNV.BinhThuong)
                {
                    //ĐV đang thuộc trạng thái thuộc loại/mất 
                    if (type == (int)MenuDVNV.Loai)
                    {
                        //Cập nhật về bt
                        dongVat.GhiChu = string.Empty;
                        dongVat.PhanLoai = (int)PhanLoaiDVNV.BinhThuong;
                        await _dasNghiepVuRepo.DongVatNghiepVu.UpdateAsync(dongVat);
                    }
                    else
                    {
                        //Nếu ở màn khai báo bt
                        return new ServiceResultError("Vui lòng xóa động vật ở danh sách động vật chết, bị loại trước");
                    }
                }
                else
                {
                    await _dasNghiepVuRepo.DongVatNghiepVu.DeleteAsync(dongVat);
                }
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa thành công");
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
        public async Task<ServiceResult> Delete(int type, IEnumerable<long> ids)
        {
            try
            {
                var dongVats = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(n => ids.Contains(n.ID));
                if (dongVats == null || dongVats.Count() == 0)
                    return new ServiceResultError("Động vật nghiệp vụ đã chọn hiện không tồn tại hoặc đã bị xóa");

                if (type == (int)MenuDVNV.Loai)
                {
                    foreach (var item in dongVats)
                    {
                        item.GhiChu = string.Empty;
                        item.PhanLoai = (int)PhanLoaiDVNV.BinhThuong;
                    }
                    await _dasNghiepVuRepo.DongVatNghiepVu.UpdateAsync(dongVats);
                }
                else
                {
                    var donVatBiLoais = dongVats.Where(n => n.PhanLoai != (int)PhanLoaiDVNV.BinhThuong);
                    if (donVatBiLoais.IsNotEmpty())
                    {
                        return new ServiceResultError($"Vui lòng xóa động vật \"{string.Join(", ", donVatBiLoais.Select(n => n.Ten))}\" ở danh sách động vật chết, bị loại trước");
                    }
                    await _dasNghiepVuRepo.DongVatNghiepVu.DeleteAsync(dongVats);
                }
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Xóa thành công");
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
        private async Task ValidateData(VMUpdateDongVatNghiepVu vmDongVatNghiepVu)
        {

            if (await _dasNghiepVuRepo.DongVatNghiepVu.AnyAsync(n => n.Code == vmDongVatNghiepVu.Code && n.ID != vmDongVatNghiepVu.ID))
            {
                throw new LogicException("Mã định danh đã tồn tại");
            }
        }
        #endregion

        #region Funtions
        private async Task<bool> SaveFileDinhKem(DongVatNghiepVu dongVatNghiepVu, VMUpdateDongVatNghiepVu data)
        {
            var deleteOlds = await _dasNghiepVuRepo.NghiepVuDongVat_DinhKem.GetAllListAsync(n => n.IDDongVatNghiepVu == dongVatNghiepVu.ID);
            if (deleteOlds.IsNotEmpty())
            {
                await _dasNghiepVuRepo.NghiepVuDongVat_DinhKem.DeleteAsync(deleteOlds);
            }

            var files = new List<NghiepVuDongVat_DinhKem>();
            var imgNames = Utils.GetStrings(data.ImgName);
            var imgPaths = Utils.GetStrings(data.ImgPath);

            if (imgNames.IsNotEmpty() && imgPaths.IsNotEmpty())
            {
                for (int i = 0; i < imgNames.Length; i++)
                {
                    var name = imgNames[i];
                    var path = imgPaths[i];

                    files.Add(new NghiepVuDongVat_DinhKem
                    {
                        IDDongVatNghiepVu = dongVatNghiepVu.ID,
                        PathFile = path,
                        TenFile = name,
                        Extension = Utils.GetExtension(path),
                        PhanLoai = (int)PhanLoaiDinhKem.HinhAnh
                    });
                }
            }

            var fileNames = Utils.GetStrings(data.FileName);
            var filePaths = Utils.GetStrings(data.FilePath);

            if (fileNames.IsNotEmpty() && filePaths.IsNotEmpty())
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    var name = fileNames[i];
                    var path = filePaths[i];

                    files.Add(new NghiepVuDongVat_DinhKem
                    {
                        IDDongVatNghiepVu = dongVatNghiepVu.ID,
                        PathFile = path,
                        TenFile = name,
                        Extension = Utils.GetExtension(path),
                        PhanLoai = (int)PhanLoaiDinhKem.File
                    });
                }
            }
            if (files.IsNotEmpty())
            {
                await _dasNghiepVuRepo.NghiepVuDongVat_DinhKem.InsertAsync(files);
                return true;
            }
            return false;
        }

        private async Task<IEnumerable<NghiepVuDongVat_DinhKem>> GetFileDinhKem(long idDongVat)
        {
            return await _dasNghiepVuRepo.NghiepVuDongVat_DinhKem.GetAllListAsync(n => n.IDDongVatNghiepVu == idDongVat);
        }

        #endregion
    }
}