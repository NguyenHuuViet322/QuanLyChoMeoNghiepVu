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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ESD.Application.Services.ESDNghiepVu
{
    public class TCDMTrangBiChoNVService : BaseMasterService, ITCDMTrangBiChoNVServices
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
        public TCDMTrangBiChoNVService(IDasRepositoryWrapper dasRepository
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

        public async Task<VMIndexTCDMTrangBiChoNV> TongHopTieuChuan(TCDMTrangBiChoNVCondition condition)
        {
            try
            {
                var model = new VMIndexTCDMTrangBiChoNV();

                var temp = from tb in _dasNghiepVuRepo.TCDMTrangBiChoNV.GetAll()
                           where (condition.NienHan == 0 || tb.NienHan == condition.NienHan)
                           orderby tb.UpdatedDate ?? tb.CreateDate descending
                           select _mapper.Map<VMTCDMTrangBiChoNV>(tb);

                var total = await temp.LongCountAsync();
                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;

                var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                var trangBis = new PaginatedList<VMTCDMTrangBiChoNV>(result, (int)total, condition.PageIndex, condition.PageSize) ?? new PaginatedList<VMTCDMTrangBiChoNV>();

                model.SearchParam = condition;
                model.DonViTinhs = Utils.GetDescribes<DonViTinh>();
                model.NienHans = Utils.GetDescribes<NienHan>();
                model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
                model.NghiepVuDongVats = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync();
                model.DonViNghiepVus = _dasNghiepVuRepo.DonViNghiepVu.GetAllList().Where(p => (p.Code.Contains("K01") || p.Code.Contains("K02") || p.Code.Contains("PK02") || p.Code.Contains("PK02E") || p.Code.Contains("A06"))).ToList();
                model.DongVatNghiepVus = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(x => x.PhanLoai == (int)PhanLoaiDVNV.BinhThuong) ?? new List<DongVatNghiepVu>();

                var lstDongVat = from dog in _dasNghiepVuRepo.DongVatNghiepVu.GetAll().Where(x => x.PhanLoai == (int)PhanLoaiDVNV.BinhThuong)
                                 where (condition.IdDonViNghiepVu == 0 || dog.IDDonViNghiepVu == condition.IdDonViNghiepVu)
                                 && (condition.DongVat == null || condition.ListDongVat.IsEmpty() || condition.ListDongVat.Contains(dog.ID.ToString()))
                                 select dog ?? new DongVatNghiepVu();
                if (lstDongVat.Count() > 0)
                {
                    foreach (var itemTrangBi in trangBis)
                    {
                        foreach (var dongvat in lstDongVat)
                        {
                            var nghiepVuDongVat = model.NghiepVuDongVats.Where(x => x.ID == dongvat.IDNghiepVuDongVat).FirstOrDefault() ?? new NghiepVuDongVat();
                            var soThangSinh = dongvat.NgaySinh != null ? condition.CalculationDate.Subtract(dongvat.NgaySinh.Value).Days / (365.25 / 12) : 0;
                            soThangSinh = soThangSinh > 0 ? soThangSinh : 0;
                            if (soThangSinh > 0)
                            {
                                switch (dongvat.PhanLoaiDongVat)
                                {
                                    case (int)PhanLoaiDongVat.ChoSinhSanCon:
                                        {
                                            if (soThangSinh >= 3 && soThangSinh <= 4)
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Tu3Den4Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            if (soThangSinh >= 5 && soThangSinh <= 10)
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Tu5Den10Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case (int)PhanLoaiDongVat.ChoDuBi:
                                        itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Tren11Thang_DM_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        break;
                                    case (int)PhanLoaiDongVat.ChoSinhSanBoMe:
                                        itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.ChoGiong_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        break;
                                    case (int)PhanLoaiDongVat.ChoNghiepVu:
                                        {
                                            if (nghiepVuDongVat.Code == "BVTTMH")
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.ChuyenTimDauVet_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            if (nghiepVuDongVat.Code == "GBMH")
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.ChuyenGiamBiet_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            if (nghiepVuDongVat.Code == "TKCN")
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.ChuyenCuuNan_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            if (nghiepVuDongVat.Code == "TN")
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.ChuyenThuocNo_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            if (nghiepVuDongVat.Code == "MT")
                                                itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.ChuyenMaTuy_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case (int)PhanLoaiDongVat.ChoNhap:
                                        {
                                            if (dongvat.TrongLuongToiDa == (int)TrongLuongToiDa.Tren30Kg)
                                            {
                                                if (soThangSinh <= 12 && soThangSinh > 0)
                                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiTu30KgTrong12Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                                if (soThangSinh > 12)
                                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiTu30KgTu13Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            }
                                            else if (dongvat.TrongLuongToiDa == (int)TrongLuongToiDa.Duoi30Kg)
                                            {
                                                if (soThangSinh <= 12 && soThangSinh > 0)
                                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiDuoi30Trong12Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                                if (soThangSinh > 12)
                                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiDuoi30Tu13Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        break;
                                    default:
                                        itemTrangBi.SoLuong = 0;
                                        break;
                                }
                            }
                        }
                    }
                }
                model.TCDMTrangBiChoNVs = trangBis;
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogInfo(ex.ToString());
                return new VMIndexTCDMTrangBiChoNV();
            }
        }

        public async Task<IEnumerable<TCDMTrangBiChoNV>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.TCDMTrangBiChoNV.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }

        public async Task<TCDMTrangBiChoNV> Get(int id)
        {
            return await _dasNghiepVuRepo.TCDMTrangBiChoNV.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexTCDMTrangBiChoNV> SearchByConditionPagging(TCDMTrangBiChoNVCondition condition)
        {
            var model = new VMIndexTCDMTrangBiChoNV
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.TCDMTrangBiChoNV.GetAll()
                           //where (condition.Keyword.IsEmpty() || tb.Name.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMTCDMTrangBiChoNV>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            model.TCDMTrangBiChoNVs = new PaginatedList<VMTCDMTrangBiChoNV>(result, (int)total, condition.PageIndex, condition.PageSize);



            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateTCDMTrangBiChoNV> Create()
        {
            var model = new VMUpdateTCDMTrangBiChoNV()
            {
            };



            return model;
        }


        public async Task<ServiceResult> Save(VMUpdateTCDMTrangBiChoNV data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var tCDMTrangBiChoNV = Utils.Bind<TCDMTrangBiChoNV>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.TCDMTrangBiChoNV.InsertAsync(tCDMTrangBiChoNV);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm tcdmtrangbichonv thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới tcdmtrangbichonv");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateTCDMTrangBiChoNV> Update(int? id)
        {
            var tCDMTrangBiChoNV = await Get(id ?? 0);
            if (tCDMTrangBiChoNV == null || tCDMTrangBiChoNV.ID == 0)
            {
                throw new LogicException("TCDMTrangBiChoNV không còn tồn tại");
            }
            var model = _mapper.Map<TCDMTrangBiChoNV, VMUpdateTCDMTrangBiChoNV>(tCDMTrangBiChoNV);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateTCDMTrangBiChoNV vmTCDMTrangBiChoNV)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var tCDMTrangBiChoNV = await _dasNghiepVuRepo.TCDMTrangBiChoNV.GetAsync(vmTCDMTrangBiChoNV.ID);
                if (tCDMTrangBiChoNV == null)
                    return new ServiceResultError("TCDMTrangBiChoNV này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmTCDMTrangBiChoNV.ParentPath;

                await ValidateData(vmTCDMTrangBiChoNV);
                tCDMTrangBiChoNV.Bind(vmTCDMTrangBiChoNV.KeyValue());

                await _dasNghiepVuRepo.TCDMTrangBiChoNV.UpdateAsync(tCDMTrangBiChoNV);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật tcdmtrangbichonv thành công");
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
                var TCDMTrangBiChoNV = await _dasNghiepVuRepo.TCDMTrangBiChoNV.GetAsync(id);
                if (TCDMTrangBiChoNV == null)
                    return new ServiceResultError("TCDMTrangBiChoNV này hiện không tồn tại hoặc đã bị xóa");


                await _dasNghiepVuRepo.TCDMTrangBiChoNV.DeleteAsync(TCDMTrangBiChoNV);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdmtrangbichonv thành công");
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
                var TCDMTrangBiChoNVDeletes = await _dasNghiepVuRepo.TCDMTrangBiChoNV.GetAllListAsync(n => ids.Contains(n.ID));
                if (TCDMTrangBiChoNVDeletes == null || TCDMTrangBiChoNVDeletes.Count() == 0)
                    return new ServiceResultError("TCDMTrangBiChoNV đã chọn hiện không tồn tại hoặc đã bị xóa");

                await _dasNghiepVuRepo.TCDMTrangBiChoNV.DeleteAsync(TCDMTrangBiChoNVDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdmtrangbichonv thành công");
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
        private async Task ValidateData(VMUpdateTCDMTrangBiChoNV vmTCDMTrangBiChoNV)
        {

            //if (await _dasNghiepVuRepo.TCDMTrangBiChoNV.IsNameExist(vmTCDMTrangBiChoNV.Name, (int)EnumCommon.Status.InActive, vmTCDMTrangBiChoNV.ID))
            //{
            //    throw new LogicException("Tên tcdmtrangbichonv đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}