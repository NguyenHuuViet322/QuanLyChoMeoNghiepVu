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
    public class CoSoVatChatController : BaseController
    {
        #region Properties

        private readonly ICoSoVatChatServices _coSoVatChatService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/CoSoVatChat";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public CoSoVatChatController(IMapper mapper, ICoSoVatChatServices coSoVatChatService,  ILoggerManager logger, IExcelServices excel)
        {
            _coSoVatChatService = coSoVatChatService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: CoSoVatChatFields
        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(CoSoVatChatCondition condition)
        {
            var model = await _coSoVatChatService.SearchByConditionPagging(condition);
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(CoSoVatChatCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _coSoVatChatService.SearchByConditionPagging(condition);
                return PartialView("Index_CoSoVatChats", model);
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
        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _coSoVatChatService.Create();
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo danh mục cơ sở vật chất");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateCoSoVatChat vmCoSoVatChat)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _coSoVatChatService.Save(vmCoSoVatChat);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _coSoVatChatService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật danh mục cơ sở vật chất");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateCoSoVatChat coSoVatChat)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _coSoVatChatService.Change(coSoVatChat);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(long id)
        {
            var rs = await _coSoVatChatService.Delete(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(long[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn danh mục cơ sở vật chất cần xoá!");
            var rs = await _coSoVatChatService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete
         
        #region Details
        [HasPermission((int)EnumModule.Code.CoSoVatChat, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _coSoVatChatService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin danh mục cơ sở vật chất");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(CoSoVatChatCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _coSoVatChatService.SearchByConditionPagging(condition);
            var export = new ExportExtend
            {
                Data = model.CoSoVatChats.Cast<dynamic>().ToList(),
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
            var rs = await _excelService.ExportExcel(export, "Báo cáo danh mục cơ sở vật chất");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Báo cáo danh mục cơ sở vật chất.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion

    }
}
