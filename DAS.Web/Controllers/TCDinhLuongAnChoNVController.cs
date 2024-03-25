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
    public class TCDinhLuongAnChoNVController : BaseController
    {
        #region Properties

        private readonly ITCDinhLuongAnChoNVServices _tCDinhLuongAnChoNVService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/TCDinhLuongAnChoNV";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public TCDinhLuongAnChoNVController(IMapper mapper, ITCDinhLuongAnChoNVServices tCDinhLuongAnChoNVService,  ILoggerManager logger, IExcelServices excel)
        {
            _tCDinhLuongAnChoNVService = tCDinhLuongAnChoNVService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: TCDinhLuongAnChoNVFields
        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(TCDinhLuongAnChoNVCondition condition)
        {
            var model = await _tCDinhLuongAnChoNVService.TongHopDinhMuc(condition);
            return PartialView(model);
        }

        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchTongHopDinhMuc(TCDinhLuongAnChoNVCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _tCDinhLuongAnChoNVService.TongHopDinhMuc(condition);
                return PartialView("Index_TCDinhLuongAnChoNVs", model);
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

        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(TCDinhLuongAnChoNVCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _tCDinhLuongAnChoNVService.SearchByConditionPagging(condition);
                return PartialView("Index_TCDinhLuongAnChoNVs", model);
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
        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _tCDinhLuongAnChoNVService.Create();
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo tcdinhluonganchonv");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateTCDinhLuongAnChoNV vmTCDinhLuongAnChoNV)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDinhLuongAnChoNVService.Save(vmTCDinhLuongAnChoNV);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _tCDinhLuongAnChoNVService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật tcdinhluonganchonv");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateTCDinhLuongAnChoNV tCDinhLuongAnChoNV)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDinhLuongAnChoNVService.Change(tCDinhLuongAnChoNV);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _tCDinhLuongAnChoNVService.Delete(id);
            return CustJSonResult(rs);
        }

        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn tcdinhluonganchonv cần xoá!");
            var rs = await _tCDinhLuongAnChoNVService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete
         
        #region Details
        //[HasPermission((int)EnumModule.Code.TCDinhLuongAnChoNV, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _tCDinhLuongAnChoNVService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin tcdinhluonganchonv");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(TCDinhLuongAnChoNVCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _tCDinhLuongAnChoNVService.TongHopDinhMuc(condition);
            var export = new ExportExtend
            {
                Data = model.TCDinhLuongAnChoNVs.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("DanhMucDinhMuc"),
                    new Col("DonViTinh", model.DonViTinhs),
                    new Col("NienHan", model.NienHans),
                    new Col("SoLuong")
                },
                Headers = new List<Header>
                {
                    new Header("STT",5),
                    new Header("Danh mục"),
                    new Header("Đơn vị tính"),
                    new Header("Niên hạn"),
                    new Header("Số lượng cấp phát")
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh sách");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Định mức cấp phát thức ăn cho động vật.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion
    }
}
