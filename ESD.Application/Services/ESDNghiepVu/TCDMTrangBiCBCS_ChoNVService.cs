using AutoMapper;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Presentation;
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
    public class TCDMTrangBiCBCS_ChoNVService : BaseMasterService, ITCDMTrangBiCBCS_ChoNVServices
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
        public TCDMTrangBiCBCS_ChoNVService(IDasRepositoryWrapper dasRepository
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

        public async Task<IEnumerable<TCDMTrangBiCBCS_ChoNV>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<List<DonViNghiepVu>> GetsListDonVi(int IdDonViNghiepVu)
        {
            List<DonViNghiepVu> lstDonVi = _dasNghiepVuRepo.DonViNghiepVu.GetAllList().Where(p => ( p.Code.StartsWith("K02") || p.Code.StartsWith("PK02")
                                                                                                 || p.Code.StartsWith("PK02E") || p.Code.StartsWith("K01")
                                                                                                 || p.Code.StartsWith("A06"))).ToList();
            if (IdDonViNghiepVu != 0)
                lstDonVi = _dasNghiepVuRepo.DonViNghiepVu.GetAllList().Where(p => (p.ID == IdDonViNghiepVu)).ToList();
            return lstDonVi;
        }
        public async Task<TCDMTrangBiCBCS_ChoNV> Get(int id)
        {
            return await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexTCDMTrangBiCBCS_ChoNV> SearchByConditionPagging(TCDMTrangBiCBCS_ChoNVCondition condition)
        {
            var model = new VMIndexTCDMTrangBiCBCS_ChoNV
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from tb in _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.GetAll()
                       where (condition.NienHan==0 || tb.NienHan == condition.NienHan)
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMTCDMTrangBiCBCS_ChoNV>(tb);

            var total = await temp.LongCountAsync();
            int totalPage =  (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            var lstChuyenMonKyThuat = _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllList();
            var lstDonViNghiepVu = _dasNghiepVuRepo.DonViNghiepVu.GetAllList();
            var lstThongTinCanBo = _dasNghiepVuRepo.ThongTinCanBo.GetAllList();

            Dictionary<int, int> ChuyenMonMapper = new Dictionary<int, int>();

            foreach(var enums in Enum.GetValues(typeof(ChuyenMonKT_CanBo)))
            {
                ChuyenMonMapper[(int)enums] = lstChuyenMonKyThuat.Where(p => p.Ten == ((ChuyenMonKT_CanBo)enums).GetEnumDescription()).FirstOrDefault().ID;
                model.SoLuongCanBo[(ChuyenMonKT_CanBo)enums] = 0;
            }    

            var lstDonVi = lstDonViNghiepVu.Where(p => (p.Code.StartsWith("K02")   || p.Code.StartsWith("PK02") 
                                                     || p.Code.StartsWith("PK02E") || p.Code.StartsWith("K01") 
                                                     || p.Code.StartsWith("A06"))).ToList();
            if (condition.IdDonViNghiepVu != 0)
                lstDonVi = lstDonViNghiepVu.Where(p => (p.ID == condition.IdDonViNghiepVu)).ToList();

            if (condition.IdChuyenMonKiThuat == 0)
            {
                foreach (var item in lstDonVi)
                    foreach(var enums in Enum.GetValues(typeof(ChuyenMonKT_CanBo)))
                    {
                        try
                        {
                            model.SoLuongCanBo[(ChuyenMonKT_CanBo)enums] += lstThongTinCanBo.Where(p => (p.IDDonViNghiepVu    == item.ID
                                                                                                      && p.IDChuyenMonKiThuat == ChuyenMonMapper[(int)enums])).ToList().Count();
                        } catch (NullReferenceException)
                        {}
                    }
            }
            else
            {
                foreach(var item in lstDonVi)
                {
                    try
                    {
                        model.SoLuongCanBo[(ChuyenMonKT_CanBo)condition.IdChuyenMonKiThuat] += lstThongTinCanBo.Where(p => (p.IDDonViNghiepVu == item.ID 
                                                                                                                        && p.IDChuyenMonKiThuat == condition.IdChuyenMonKiThuat)).ToList().Count();
                    }
                    catch (NullReferenceException) { }
                } 
            }

            model.DonViTinhs = Utils.GetDescribes<DonViTinh>();
            model.NienHans = Utils.GetDescribes<NienHan>();

            model.TCDMTrangBiCBCS_ChoNVs = new PaginatedList<VMTCDMTrangBiCBCS_ChoNV>(result, (int)total, condition.PageIndex, condition.PageSize);
            model.ChuyenMonKiThuats = await _dasNghiepVuRepo.ChuyenMonKiThuat.GetAllListAsync();
            model.DonViNghiepVus = lstDonViNghiepVu.Where(p => (   p.Code.StartsWith("K02")   || p.Code.StartsWith("PK02") 
                                                                || p.Code.StartsWith("PK02E") || p.Code.StartsWith("K01") 
                                                                || p.Code.StartsWith("A06"))).ToList();

            foreach(var item in model.TCDMTrangBiCBCS_ChoNVs)
            {
                item.CapPhat = (int)Math.Ceiling((double)(model.SoLuongCanBo[ChuyenMonKT_CanBo.HocVien_DM] * @item.HocVien_DM
                                                         +
                                                          model.SoLuongCanBo[ChuyenMonKT_CanBo.CanBoThuY_DM] * @item.CanBoThuY_DM
                                                         +
                                                          model.SoLuongCanBo[ChuyenMonKT_CanBo.NVCapDuong_DM] * @item.NVCapDuong_DM
                                                         +
                                                          model.SoLuongCanBo[ChuyenMonKT_CanBo.CanBoQL_DM] * @item.CanBoQL_DM
                                                         +
                                                          model.SoLuongCanBo[ChuyenMonKT_CanBo.CanBoQLChoNV_DM] * @item.CanBoQLChoNV_DM
                                                         +
                                                          model.SoLuongCanBo[ChuyenMonKT_CanBo.GiaoVienHD_DM] * @item.GiaoVienHD_DM));
            }

            var lstTCDMchoCanBo = model.TCDMTrangBiCBCS_ChoNVs.Where(p => p.CapPhat != 0).ToList();
            model.TCDMTrangBiCBCS_ChoNVs = new PaginatedList<VMTCDMTrangBiCBCS_ChoNV>(lstTCDMchoCanBo, lstTCDMchoCanBo.Count(), condition.PageIndex, condition.PageSize);

            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateTCDMTrangBiCBCS_ChoNV> Create()
        {
            var model = new VMUpdateTCDMTrangBiCBCS_ChoNV()
            {
            };
            


            return model;
        }
        

        public async Task<ServiceResult> Save(VMUpdateTCDMTrangBiCBCS_ChoNV data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var tCDMTrangBiCBCS_ChoNV = Utils.Bind<TCDMTrangBiCBCS_ChoNV>(data.KeyValue());
                await ValidateData(data);
                await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.InsertAsync(tCDMTrangBiCBCS_ChoNV);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm tcdmtrangbicbcs_chonv thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới tcdmtrangbicbcs_chonv");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateTCDMTrangBiCBCS_ChoNV> Update(int? id)
        {
            var tCDMTrangBiCBCS_ChoNV = await Get(id ?? 0);
            if (tCDMTrangBiCBCS_ChoNV == null || tCDMTrangBiCBCS_ChoNV.ID == 0)
            {
                throw new LogicException("TCDMTrangBiCBCS_ChoNV không còn tồn tại");
            }
            var model = _mapper.Map<TCDMTrangBiCBCS_ChoNV,VMUpdateTCDMTrangBiCBCS_ChoNV>(tCDMTrangBiCBCS_ChoNV);


            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateTCDMTrangBiCBCS_ChoNV vmTCDMTrangBiCBCS_ChoNV)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var tCDMTrangBiCBCS_ChoNV = await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.GetAsync(vmTCDMTrangBiCBCS_ChoNV.ID);
                if (tCDMTrangBiCBCS_ChoNV == null)
                    return new ServiceResultError("TCDMTrangBiCBCS_ChoNV này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmTCDMTrangBiCBCS_ChoNV.ParentPath;

                await ValidateData(vmTCDMTrangBiCBCS_ChoNV);
                tCDMTrangBiCBCS_ChoNV.Bind(vmTCDMTrangBiCBCS_ChoNV.KeyValue());

                await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.UpdateAsync(tCDMTrangBiCBCS_ChoNV);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật tcdmtrangbicbcs_chonv thành công");
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
                var TCDMTrangBiCBCS_ChoNV = await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.GetAsync(id);
                if (TCDMTrangBiCBCS_ChoNV == null)
                    return new ServiceResultError("TCDMTrangBiCBCS_ChoNV này hiện không tồn tại hoặc đã bị xóa");

              
                await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.DeleteAsync(TCDMTrangBiCBCS_ChoNV);

                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdmtrangbicbcs_chonv thành công");
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
                var TCDMTrangBiCBCS_ChoNVDeletes = await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.GetAllListAsync(n => ids.Contains(n.ID));
                if (TCDMTrangBiCBCS_ChoNVDeletes == null || TCDMTrangBiCBCS_ChoNVDeletes.Count() == 0)
                    return new ServiceResultError("TCDMTrangBiCBCS_ChoNV đã chọn hiện không tồn tại hoặc đã bị xóa");

                await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.DeleteAsync(TCDMTrangBiCBCS_ChoNVDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa tcdmtrangbicbcs_chonv thành công");
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
        private async Task ValidateData(VMUpdateTCDMTrangBiCBCS_ChoNV vmTCDMTrangBiCBCS_ChoNV)
        {

            //if (await _dasNghiepVuRepo.TCDMTrangBiCBCS_ChoNV.IsNameExist(vmTCDMTrangBiCBCS_ChoNV.Name, (int)EnumCommon.Status.InActive, vmTCDMTrangBiCBCS_ChoNV.ID))
            //{
            //    throw new LogicException("Tên tcdmtrangbicbcs_chonv đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}