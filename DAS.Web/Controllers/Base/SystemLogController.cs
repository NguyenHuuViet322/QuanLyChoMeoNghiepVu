using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Utility;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SystemLogController : BaseController
    {
        #region Properties
        private readonly IUserLogServices _systemLog;
        private readonly IExcelServices _excelServices;
        #endregion Properties

        #region Ctor
        public SystemLogController(IUserLogServices userLogServices
            , IExcelServices excelServices)
        {
            _systemLog = userLogServices;
            _excelServices = excelServices;
        }
        #endregion Ctor

        #region List & Search  
        [HasPermission((int)EnumModule.Code.NKHT, new int[] { (int)EnumPermission.Type.Read })]
      
        public async Task<IActionResult>  Index(LogInfoCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/SystemLog", "Nhật ký hệ thống" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            condition.Type = 0;
            var model = await _systemLog.GetCRUDLogByCondition(condition);
            return View(model);
        }

        [HasPermission((int)EnumModule.Code.NKHT, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> SearchByConditionPagging(LogInfoCondition condition)
        {
            condition.Type = 0;
            var model = await _systemLog.GetCRUDLogByCondition(condition);
            return PartialView("Index_SystemLogs", model);
        }
        #endregion List & Search

        #region Export
        [HasPermission((int)EnumModule.Code.NKHT, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> Export(LogInfoCondition condition)
        {
            condition.Type = 0;
            var list = await _systemLog.GetCRUDLogByCondition(condition,true);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("TagName"),
                    new Col("Entity"),
                    new Col("Action"),
                    new Col("CreatedDate"),
                    new Col("UserId"),
                    new Col("Username"),
                    new Col{
                        Field = "OldValue",
                        isWrapText =false
                    },
                    new Col{
                        Field = "NewValue",
                        isWrapText =false
                    },
                    new Col{
                        Field = "ChangedValue",
                        isWrapText =false
                    },
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("ID bản ghi"),
                    new Header("Bảng"),
                    new Header("Thao tác"),
                    new Header("Thời điểm"),
                    new Header("ID tài khoản"),
                    new Header("Tài khoản"),
                    new Header("Giá trị cũ"),
                    new Header("Giá trị mới"),
                    new Header("Thay đổi"),
                }
            };
            var rs = await _excelServices.ExportExcel(export, "Nhật ký hệ thống", false);
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "NhatKySystemCRUD.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Export

        #region List & Search   Chart
        [HasPermission((int)EnumModule.Code.SYSTEMLOGERROL, new int[] { (int)EnumPermission.Type.Read })]

        public async Task<IActionResult> ChartHistory(LogInfoCondition condition)
        {
            condition.FromDate = Utils.DateToString(Utils.GetFirstDayOfMonth(DateTime.Now));
            condition.ToDate = Utils.DateToString(DateTime.Now);
            ViewBag.StartDate = Utils.DateToString(Utils.GetFirstDayOfMonth(DateTime.Now));
            ViewBag.EndDate = Utils.DateToString(DateTime.Now);
            var model = await _systemLog.GetChartCRUDLogByCondition(condition);
            return View("IndexChart", model);
        }
        [HasPermission((int)EnumModule.Code.SYSTEMLOGERROL, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> SearchByConditionChart(LogInfoCondition condition)
        {
            var model = await _systemLog.GetChartCRUDLogByCondition(condition);
            return PartialView("Index_SystemLogsChart", model);
        }
        [HasPermission((int)EnumModule.Code.NKHT, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> ExportChart(LogInfoCondition condition)
        {
            condition.Type = 0;
            var list = await _systemLog.GetChartCRUDLogByCondition(condition);
            var datas = list.vMLogInfos.ToList();
            var export = new ExportExtend
            {
                Data = datas.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Action"),
                    new Col("Total"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Action"),
                    new Header("Số lượng"),
                }
            };
            var rs = await _excelServices.ExportExcel(export, "ThongKeTruyCap", false);
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "ThongKeTruyCapLichSuHeThong.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion List & Search
    }
}
