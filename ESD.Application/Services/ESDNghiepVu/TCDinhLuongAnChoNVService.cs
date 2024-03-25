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
    public class TCDinhLuongAnChoNVService : BaseMasterService, ITCDinhLuongAnChoNVServices
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
        public TCDinhLuongAnChoNVService(IDasRepositoryWrapper dasRepository
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

        #region Tổng Hợp Định Mức Ăn
        public async Task<VMIndexTCDinhLuongAnChoNV> TongHopDinhMuc(TCDinhLuongAnChoNVCondition condition)
        {
            var model = new VMIndexTCDinhLuongAnChoNV
            {
                SearchParam = condition
            };

            var temp = from tb in _dasNghiepVuRepo.TCDinhLuongAnChoNV.GetAll()
                       where (condition.NienHan == 0 || tb.NienHan == condition.NienHan)
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMTCDinhLuongAnChoNV>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var trangBis = new PaginatedList<VMTCDinhLuongAnChoNV>(result, (int)total, condition.PageIndex, condition.PageSize);

            model.DonViTinhs = Utils.GetDescribes<DonViTinh>();
            model.NienHans = Utils.GetDescribes<NienHan>();
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.NghiepVuDongVats = await _dasNghiepVuRepo.NghiepVuDongVat.GetAllListAsync();
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.DongVatNghiepVus = await _dasNghiepVuRepo.DongVatNghiepVu.GetAllListAsync(x => x.PhanLoai == (int)PhanLoaiDVNV.BinhThuong) ?? new List<DongVatNghiepVu>();

            var lstDongVat = from dog in _dasNghiepVuRepo.DongVatNghiepVu.GetAll().Where(x => x.PhanLoai == (int)PhanLoaiDVNV.BinhThuong)
                         where (condition.IdDonViNghiepVu == 0 || dog.IDDonViNghiepVu == condition.IdDonViNghiepVu)
                         && (condition.DongVat == null || condition.ListDongVat.IsEmpty() || condition.ListDongVat.Contains(dog.ID.ToString()))
                         select dog;

            foreach (var itemTrangBi in trangBis)
            {
                foreach (var dongvat in lstDongVat)
                {
                    var nghiepVuDongVat = model.NghiepVuDongVats.Where(x => x.ID == dongvat.IDNghiepVuDongVat).FirstOrDefault() ?? new NghiepVuDongVat();
                    var soThangSinh = dongvat.NgaySinh != null ? condition.CalculationDate.Subtract(dongvat.NgaySinh.Value).Days / (365.25 / 12) : 0;
                    soThangSinh = soThangSinh > 0 ? soThangSinh : 0;
                    if(soThangSinh > 0)
                    {
                        if (dongvat.TrongLuongToiDa == (int)TrongLuongToiDa.Duoi30Kg)
                        {
                            switch (dongvat.PhanLoaiDongVat)
                            {
                                case (int)PhanLoaiDongVat.ChoSinhSanCon:
                                    {
                                        if (soThangSinh <= 2 && soThangSinh > 0)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Duoi30KgDuoi2Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        if (soThangSinh >= 3 && soThangSinh <= 4)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Duoi30KgTu3Den4Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        if (soThangSinh >= 5 && soThangSinh <= 10)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Duoi30KgTu5Den10Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    }
                                    break;
                                case (int)PhanLoaiDongVat.ChoDuBi:
                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Duoi30KgTu11Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    break;
                                case (int)PhanLoaiDongVat.ChoSinhSanBoMe:
                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.GiongDuoi30Kg_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    break;
                                case (int)PhanLoaiDongVat.ChoNghiepVu:
                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.HuanLuyenDuoi30Kg_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    break;
                                case (int)PhanLoaiDongVat.ChoNhap:
                                    {
                                        if (soThangSinh < 13 && soThangSinh > 0)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiDuoi30Trong12Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        if (soThangSinh >= 13)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiDuoi30Tu13Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    }
                                    break;
                                default:
                                    itemTrangBi.SoLuong = 0;
                                    break;
                            }
                        }
                        if (dongvat.TrongLuongToiDa == (int)TrongLuongToiDa.Tren30Kg)
                        {
                            switch (dongvat.PhanLoaiDongVat)
                            {
                                case (int)PhanLoaiDongVat.ChoSinhSanCon:
                                    {
                                        if (soThangSinh <= 2 && soThangSinh > 0)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Tu30KgDuoi2Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        if (soThangSinh >= 3 && soThangSinh <= 4)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Tu30KgTu3Den4Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        if (soThangSinh >= 5 && soThangSinh <= 10)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Tu30KgTu5Den10Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    }
                                    break;
                                case (int)PhanLoaiDongVat.ChoDuBi:
                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.Duoi30KgTu11Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    break;
                                case (int)PhanLoaiDongVat.ChoSinhSanBoMe:
                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.GiongTu30Kg_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    break;
                                case (int)PhanLoaiDongVat.ChoNghiepVu:
                                    itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.HuanLuyenTu30Kg_DM.Value, 0, MidpointRounding.AwayFromZero);
                                    break;
                                case (int)PhanLoaiDongVat.ChoNhap:
                                    {
                                        if (soThangSinh < 13 && soThangSinh > 0)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiTu30KgTrong12Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
                                        if (soThangSinh >= 13)
                                            itemTrangBi.SoLuong = Math.Round(itemTrangBi.SoLuong + itemTrangBi.NhapNoiTu30KgTu13Thang_DM.Value, 0, MidpointRounding.AwayFromZero);
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

            model.TCDinhLuongAnChoNVs = trangBis;
            return model;
        }
        #endregion

        #region Gets  

        public async Task<IEnumerable<TCDinhLuongAnChoNV>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.TCDinhLuongAnChoNV.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<TCDinhLuongAnChoNV> Get(int id)
        {
            return await _dasNghiepVuRepo.TCDinhLuongAnChoNV.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexTCDinhLuongAnChoNV> SearchByConditionPagging(TCDinhLuongAnChoNVCondition condition)
        {
            var model = new VMIndexTCDinhLuongAnChoNV
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.TCDinhLuongAnChoNV.GetAll()
                           //where (condition.Keyword.IsEmpty() || tb.Name.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMTCDinhLuongAnChoNV>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            model.TCDinhLuongAnChoNVs = new PaginatedList<VMTCDinhLuongAnChoNV>(result, (int)total, condition.PageIndex, condition.PageSize);



            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateTCDinhLuongAnChoNV> Create()
        {
            var model = new VMUpdateTCDinhLuongAnChoNV()
            {
            };



            return model;
        }


        public async Task<ServiceResult> Save(VMUpdateTCDinhLuongAnChoNV data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var tCDinhLuongAnChoNV = Utils.Bind<TCDinhLuongAnChoNV>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.TCDinhLuongAnChoNV.InsertAsync(tCDinhLuongAnChoNV);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm tcdinhluonganchonv thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới tcdinhluonganchonv");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateTCDinhLuongAnChoNV> Update(int? id)
        {
            var tCDinhLuongAnChoNV = await Get(id ?? 0);
            if (tCDinhLuongAnChoNV == null || tCDinhLuongAnChoNV.ID == 0)
            {
                throw new LogicException("TCDinhLuongAnChoNV không còn tồn tại");
            }
            var model = _mapper.Map<TCDinhLuongAnChoNV, VMUpdateTCDinhLuongAnChoNV>(tCDinhLuongAnChoNV);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateTCDinhLuongAnChoNV vmTCDinhLuongAnChoNV)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var tCDinhLuongAnChoNV = await _dasNghiepVuRepo.TCDinhLuongAnChoNV.GetAsync(vmTCDinhLuongAnChoNV.ID);
                if (tCDinhLuongAnChoNV == null)
                    return new ServiceResultError("TCDinhLuongAnChoNV này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmTCDinhLuongAnChoNV.ParentPath;

                await ValidateData(vmTCDinhLuongAnChoNV);
                tCDinhLuongAnChoNV.Bind(vmTCDinhLuongAnChoNV.KeyValue());

                await _dasNghiepVuRepo.TCDinhLuongAnChoNV.UpdateAsync(tCDinhLuongAnChoNV);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật tcdinhluonganchonv thành công");
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
                var TCDinhLuongAnChoNV = await _dasNghiepVuRepo.TCDinhLuongAnChoNV.GetAsync(id);
                if (TCDinhLuongAnChoNV == null)
                    return new ServiceResultError("TCDinhLuongAnChoNV này hiện không tồn tại hoặc đã bị xóa");


                await _dasNghiepVuRepo.TCDinhLuongAnChoNV.DeleteAsync(TCDinhLuongAnChoNV);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdinhluonganchonv thành công");
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
                var TCDinhLuongAnChoNVDeletes = await _dasNghiepVuRepo.TCDinhLuongAnChoNV.GetAllListAsync(n => ids.Contains(n.ID));
                if (TCDinhLuongAnChoNVDeletes == null || TCDinhLuongAnChoNVDeletes.Count() == 0)
                    return new ServiceResultError("TCDinhLuongAnChoNV đã chọn hiện không tồn tại hoặc đã bị xóa");

                await _dasNghiepVuRepo.TCDinhLuongAnChoNV.DeleteAsync(TCDinhLuongAnChoNVDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdinhluonganchonv thành công");
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
        private async Task ValidateData(VMUpdateTCDinhLuongAnChoNV vmTCDinhLuongAnChoNV)
        {

            //if (await _dasNghiepVuRepo.TCDinhLuongAnChoNV.IsNameExist(vmTCDinhLuongAnChoNV.Name, (int)EnumCommon.Status.InActive, vmTCDinhLuongAnChoNV.ID))
            //{
            //    throw new LogicException("Tên tcdinhluonganchonv đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}