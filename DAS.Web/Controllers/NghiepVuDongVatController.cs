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
    public class NghiepVuDongVatController : BaseController
    {
        #region Properties

        private readonly INghiepVuDongVatServices _nghiepVuDongVatService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/NghiepVuDongVat";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public NghiepVuDongVatController(IMapper mapper, INghiepVuDongVatServices nghiepVuDongVatService,  ILoggerManager logger, IExcelServices excel)
        {
            _nghiepVuDongVatService = nghiepVuDongVatService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: NghiepVuDongVatFields
        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(NghiepVuDongVatCondition condition)
        {
            var model = await _nghiepVuDongVatService.SearchByConditionPagging(condition);
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(NghiepVuDongVatCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _nghiepVuDongVatService.SearchByConditionPagging(condition);
                return PartialView("Index_NghiepVuDongVats", model);
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
        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _nghiepVuDongVatService.Create();
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo nghiệp vụ động vật");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateNghiepVuDongVat vmNghiepVuDongVat)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _nghiepVuDongVatService.Save(vmNghiepVuDongVat);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _nghiepVuDongVatService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật nghiệp vụ động vật");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateNghiepVuDongVat nghiepVuDongVat)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _nghiepVuDongVatService.Change(nghiepVuDongVat);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _nghiepVuDongVatService.Delete(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn nghiệp vụ động vật cần xoá!");
            var rs = await _nghiepVuDongVatService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete
         
        #region Details
        [HasPermission((int)EnumModule.Code.NghiepVuDongVat, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _nghiepVuDongVatService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin nghiệp vụ động vật");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(NghiepVuDongVatCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _nghiepVuDongVatService.SearchByConditionPagging(condition);
            var export = new ExportExtend
            {
                Data = model.NghiepVuDongVats.Cast<dynamic>().ToList(),
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
            var rs = await _excelService.ExportExcel(export, "Báo cáo nghiệp vụ động vật");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Báo cáo nghiệp vụ động vật.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion

    }
}
