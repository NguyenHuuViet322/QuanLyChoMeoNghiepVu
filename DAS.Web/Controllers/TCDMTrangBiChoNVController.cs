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
    public class TCDMTrangBiChoNVController : BaseController
    {
        #region Properties

        private readonly ITCDMTrangBiChoNVServices _tCDMTrangBiChoNVService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/TCDMTrangBiChoNV";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public TCDMTrangBiChoNVController(IMapper mapper, ITCDMTrangBiChoNVServices tCDMTrangBiChoNVService, ILoggerManager logger, IExcelServices excel)
        {
            _tCDMTrangBiChoNVService = tCDMTrangBiChoNVService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: TCDMTrangBiChoNVFields
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(TCDMTrangBiChoNVCondition condition)
        {
            var model = await _tCDMTrangBiChoNVService.TongHopTieuChuan(condition);
            return PartialView(model);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(TCDMTrangBiChoNVCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _tCDMTrangBiChoNVService.TongHopTieuChuan(condition);
                return PartialView("Index_TCDMTrangBiChoNVs", model);
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
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _tCDMTrangBiChoNVService.Create();
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo tcdmtrangbichonv");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateTCDMTrangBiChoNV vmTCDMTrangBiChoNV)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDMTrangBiChoNVService.Save(vmTCDMTrangBiChoNV);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _tCDMTrangBiChoNVService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật tcdmtrangbichonv");
            }
            return Redirect(defaultPath);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateTCDMTrangBiChoNV tCDMTrangBiChoNV)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _tCDMTrangBiChoNVService.Change(tCDMTrangBiChoNV);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _tCDMTrangBiChoNVService.Delete(id);
            return CustJSonResult(rs);
        }

        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn tcdmtrangbichonv cần xoá!");
            var rs = await _tCDMTrangBiChoNVService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Details
        //[HasPermission((int)EnumModule.Code.TCDMTrangBiChoNV, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _tCDMTrangBiChoNVService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin tcdmtrangbichonv");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(TCDMTrangBiChoNVCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _tCDMTrangBiChoNVService.TongHopTieuChuan(condition);
            //var export = new ExportExtend3
            //{
            //    Data = model.TCDMTrangBiChoNVs.Cast<dynamic>().ToList(),
            //    title = "DANH SÁCH SỐ LƯỢNG CƠ SỞ VẬT CHẤT CẤP PHÁT ĐỊNH KỲ CHO ĐỘNG VẬT THEO ĐƠN VỊ",
            //    RowStart = 1,
            //    ColStart = 1,
            //    IsCreateHeader = true,
            //    Cols = new List<Col>
            //    {
            //        new Col{
            //            DataType = 5
            //        },
            //        new Col("DanhMucDinhMuc"),
            //        new Col("DonViTinh", model.DonViTinhs),
            //        new Col("NienHan", model.NienHans),
            //        new Col("SoLuong")
            //    },
            //    Headers = new List<Header>
            //    {
            //      new Header("STT",5),
            //      new Header("Danh mục"),
            //      new Header("Đơn vị tính"),
            //      new Header("Niên hạn"),
            //      new Header("Số lượng cấp phát")
            //    }
            //};

            var export = new ExportExtend
            {
                Data = model.TCDMTrangBiChoNVs.Cast<dynamic>().ToList(),
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
                return NotFound();
            else
            {
                var fileName = "Định mức cấp phát trang thiết bị cho động vật.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion
    }
}
