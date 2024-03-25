using AutoMapper;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Interfaces.ESDNghiepVu;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using System.Collections.Generic;
using System.Linq;
using ESD.Application.Enums.ESDTieuChuanKiemDinh;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TCDMTrangBiCBCS_ChoNVController : BaseController
    {
        #region Properties

        private readonly ITCDMTrangBiCBCS_ChoNVServices _tCDMTrangBiCBCS_ChoNVService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/TCDMTrangBiCBCS_ChoNV";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public TCDMTrangBiCBCS_ChoNVController(IMapper mapper, ITCDMTrangBiCBCS_ChoNVServices tCDMTrangBiCBCS_ChoNVService,  ILoggerManager logger, IExcelServices excel)
        {
            _tCDMTrangBiCBCS_ChoNVService = tCDMTrangBiCBCS_ChoNVService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: TCDMTrangBiCBCS_ChoNVFields
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(TCDMTrangBiCBCS_ChoNVCondition condition)
        {
            var model = await _tCDMTrangBiCBCS_ChoNVService.SearchByConditionPagging(condition);
            return PartialView(model);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(TCDMTrangBiCBCS_ChoNVCondition condition)
        {
            try
            {
                var model = await _tCDMTrangBiCBCS_ChoNVService.SearchByConditionPagging(condition);
                model.SearchParam = condition;
                return PartialView("Index_TCDMTrangBiCBCS_ChoNVs", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy dữ liệu");
            }
            return Redirect(defaultPath);
        }
        #endregion List

        #region Create
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _tCDMTrangBiCBCS_ChoNVService.Create();
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo tcdmtrangbicbcs_chonv");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateTCDMTrangBiCBCS_ChoNV vmTCDMTrangBiCBCS_ChoNV)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDMTrangBiCBCS_ChoNVService.Save(vmTCDMTrangBiCBCS_ChoNV);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _tCDMTrangBiCBCS_ChoNVService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật tcdmtrangbicbcs_chonv");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateTCDMTrangBiCBCS_ChoNV tCDMTrangBiCBCS_ChoNV)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDMTrangBiCBCS_ChoNVService.Change(tCDMTrangBiCBCS_ChoNV);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _tCDMTrangBiCBCS_ChoNVService.Delete(id);
            return CustJSonResult(rs);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn tcdmtrangbicbcs_chonv cần xoá!");
            var rs = await _tCDMTrangBiCBCS_ChoNVService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete
         
        #region Details
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiCBCS_ChoNV, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _tCDMTrangBiCBCS_ChoNVService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin tcdmtrangbicbcs_chonv");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(TCDMTrangBiCBCS_ChoNVCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _tCDMTrangBiCBCS_ChoNVService.SearchByConditionPagging(condition);
            var lstDonVi = await _tCDMTrangBiCBCS_ChoNVService.GetsListDonVi(condition.IdDonViNghiepVu);

            var export = new ExportExtend3
            {
                Data = model.TCDMTrangBiCBCS_ChoNVs.Cast<dynamic>().ToList(),
                title = "Cấp phát hàng nghiệp vụ CSVC cho cán bộ",
                description = lstDonVi,
                RowStart = 1,
                ColStart = 1,
                IsCreateHeader = true,
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("DanhMucDinhMuc"),
                    new Col("DonViTinh", new Dictionary<int, string>()
                    {
                        {1, "Chiếc"}, {2, "Bộ" }, {3, "Hộp" }, {4, "Cái"}
                    }),
                    new Col("NienHan", new Dictionary<int, string>()
                    {
                        {1, "1 năm" }, {2, "1 tháng"}
                    }),
                    new Col("CapPhat")
                },
                Headers = new List<Header>
                {
                  new Header("STT", 10),
                  new Header("Danh mục", 35),
                  new Header("Đơn vị tính", 15),
                  new Header("Niên hạn", 20),
                  new Header("Số lượng cấp phát", 25)
                }
            };
            var rs = await _excelService.ExportExcelCus2(export, "Báo cáo");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Báo cáo cấp phát hàng nghiệp vụ CSVC cho cán bộ.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion

    }
}
