using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{ 
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReaderOrganManagementController : BaseController
    {
        #region Properties
        private readonly IExcelServices _excelService;
        private readonly IReaderServices _readerServices;
        #endregion Properties

        #region Ctor
        public ReaderOrganManagementController(IExcelServices excelServices
            , IReaderServices readerServices)
        {
            _excelService = excelServices;
            _readerServices = readerServices;
        }
        #endregion Ctor

        #region List 
        public async Task<IActionResult> Index(ReaderCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/ReaderOrganManagement", "Quản lý độc giả" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            ViewBag.Keyword = condition.Keyword;
            var model = await _readerServices.SearchByConditionPaggingOrgan(condition);
            return View(model);
        }

        public async Task<IActionResult> SearchByConditionPagging(ReaderCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _readerServices.SearchByConditionPaggingOrgan(condition);
            return PartialView("Index_Readers", model);
        }
        #endregion List

        //#region Delete
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var rs = await _readerServices.DeleteReader(id);
        //    return CustJSonResult(rs);
        //}
        //[HttpPost]
        //public async Task<IActionResult> DeleteMulti(int[] ids)
        //{
        //    if (ids == null || ids.Length == 0)
        //        return JSErrorResult("Vui lòng chọn độc giả muốn xóa");
        //    var rs = await _readerServices.DeleteReaders(ids);
        //    return CustJSonResult(rs);
        //}
        //#endregion Delete

        #region Create
        public IActionResult CreatePopup()
        {
            VMReaderRegister model = new VMReaderRegister();
            return PartialView("Index_Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(VMReaderRegister model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            if (model.Password.Length > 50 || model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Mật khẩu có tối thiểu 6 ký tự và tối đa 50 ký tự");
                return JSErrorModelStateByLine();
            }
            var rs = await _readerServices.RegisterByOrgan(model);
            return CustJSonResult(rs);

        }
        #endregion

        #region Update
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _readerServices.GetReaderOrgan(id.Value);
            if (model == null)
                return NotFound();
            ViewBag.isDetail = true;
            return PartialView("Index_Update", model);
        }

        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _readerServices.GetReaderOrgan(id.Value);
            if (model == null)
                return NotFound();
            ViewBag.isDetail = false;
            return PartialView("Index_Update", model);
        }
        public async Task<IActionResult> Update(VMReader model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _readerServices.UpdateReaderOrgan(model);
            return CustJSonResult(rs);
        }

        #endregion Update
        #region Export
        [HttpGet]
        public async Task<IActionResult> Export(ReaderCondition condition)
        {
            var enumUser = StringUltils.GetEnumDictionary<EnumCommon.Status>();
            var list = await _readerServices.SearchByConditionPaggingOrgan(condition, true);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("AccountName"),
                    new Col("Name"),
                    new Col("Email"),
                    new Col("Phone"),
                    new Col("Birthplace"),
                    new Col("Address"),
                    new Col{
                        Field = "Status",
                        DataType =2,
                        DefineEnum = enumUser,
                    }
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Tên tài khoản"),
                    new Header("Họ và tên độc giả"),
                    new Header("Email"),
                    new Header("Số điện thoại"),
                    new Header("Cơ quan công tác"),
                    new Header("Địa chỉ liên hệ"),
                    new Header("Trạng thái"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh sách độc giả");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhSachDocGia.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Export
    }
}
