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

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TCDMTrangBi_DonViController : BaseController
    {
        #region Properties

        private readonly ITCDMTrangBi_DonViServices _tCDMTrangBi_DonViService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/TCDMTrangBi_DonVi";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public TCDMTrangBi_DonViController(IMapper mapper, ITCDMTrangBi_DonViServices tCDMTrangBi_DonViService, ILoggerManager logger, IExcelServices excel)
        {
            _tCDMTrangBi_DonViService = tCDMTrangBi_DonViService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: TCDMTrangBi_DonViFields
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(TCDMTrangBi_DonViCondition condition)
        {
            var model = await _tCDMTrangBi_DonViService.SearchByConditionPagging(condition);
            return PartialView(model);
        }
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(TCDMTrangBi_DonViCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _tCDMTrangBi_DonViService.SearchByConditionPagging(condition);
                return PartialView("Index_TCDMTrangBi_DonVis", model);
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

        public async Task<IActionResult> TongHop(TCDMTrangBi_DonViCondition condition)
        {
            var model = await _tCDMTrangBi_DonViService.TongHopTieuChuan(condition);
            return PartialView( model);
        }

        // GET: TCDMTrangBi_DonViFields
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TongHopTieuChuan(TCDMTrangBi_DonViCondition condition)
        {

            var model = await _tCDMTrangBi_DonViService.TongHopTieuChuan(condition);
            return PartialView("TongHop_TCDMTrangBi_DonVis", model);
        }

        #endregion List

        #region Create
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _tCDMTrangBi_DonViService.Create();
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo tcdmtrangbi_donvi");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateTCDMTrangBi_DonVi vmTCDMTrangBi_DonVi)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();


            //CallService
            var rs = await _tCDMTrangBi_DonViService.Save(vmTCDMTrangBi_DonVi);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _tCDMTrangBi_DonViService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật tcdmtrangbi_donvi");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateTCDMTrangBi_DonVi tCDMTrangBi_DonVi)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDMTrangBi_DonViService.Change(tCDMTrangBi_DonVi);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _tCDMTrangBi_DonViService.Delete(id);
            return CustJSonResult(rs);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn tcdmtrangbi_donvi cần xoá!");
            var rs = await _tCDMTrangBi_DonViService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Details
        //[HasPermission((int)EnumModule.Code.TCDMTrangBi_DonVi, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _tCDMTrangBi_DonViService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin tcdmtrangbi_donvi");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> ExportTongHop(TCDMTrangBi_DonViCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _tCDMTrangBi_DonViService.TongHopTieuChuan(condition);
            var export = new ExportExtend
            {
                Data = model.TCDMTrangBi_DonVis.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("StrPhong"),
                    new Col("DanhMucDinhMuc"),
                    new Col("StrDonViTinh"),
                    new Col("StrNienHan"),
                    new Col("SoLuong"),
                    new Col("StrDuTru"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  new Header("Phòng"),
                  new Header("Trang bị"),
                  new Header("Đơn vị tính"),
                  new Header("Niên hạn"),
                  new Header("SL cấp phát"),
                  new Header("SL dự trú"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Báo cáo");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Báo cáo cấp phát hàng nghiệp vụ CSVC cho đơn vị.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        public async Task<IActionResult> Export(TCDMTrangBi_DonViCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _tCDMTrangBi_DonViService.SearchByConditionPagging(condition);
            var export = new ExportExtend
            {
                Data = model.TCDMTrangBi_DonVis.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    //new Col("Name"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  //new Header("Tên"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Báo cáo tcdmtrangbi_donvi");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Báo cáo tcdmtrangbi_donvi.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion

    }
}
