using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SystemLogErrolController : BaseController
    {
        #region Properties
        private readonly IUserLogServices _systemLog;
        private readonly IExcelServices _excelServices;
        #endregion Properties

        #region Ctor
        public SystemLogErrolController(IUserLogServices userLogServices
            , IExcelServices excelServices)
        {
            _systemLog = userLogServices;
            _excelServices = excelServices;
        }
        #endregion Ctor

        #region List & Search  
        [HasPermission((int)EnumModule.Code.SYSTEMLOGERROL, new int[] { (int)EnumPermission.Type.Read })]
      
        public async Task<IActionResult>  Index(LogInfoCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/SystemLogErrol", "Báo cáo thống kê về lỗi hệ thống" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            condition.Type = 1;
            condition.ActionCRUD = "Lỗi";
            var model = await _systemLog.GetCRUDLogByConditionErrol(condition);
            return View(model);
        }

        [HasPermission((int)EnumModule.Code.SYSTEMLOGERROL, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> SearchByConditionPagging(LogInfoCondition condition)
        {
            condition.Type = 1;
            condition.ActionCRUD = "Lỗi";
            var model = await _systemLog.GetCRUDLogByConditionErrol(condition);
            return PartialView("Index_SystemLogs", model);
        }
        #endregion List & Search

        #region Export
        [HasPermission((int)EnumModule.Code.SYSTEMLOGERROL, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> Export(LogInfoCondition condition)
        {
            condition.Type = 1;
            condition.ActionCRUD = "Lỗi";
            var list = await _systemLog.GetCRUDLogByConditionErrol(condition,true);
            var datalit = list.Tables.ToList();
            var export = new ExportExtend
            {
                Data = datalit.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Action"),
                    new Col("CreatedDate"),
                    new Col("UserId"),
                    new Col("Username"),
                    new Col{
                        Field = "OldValue",
                        isWrapText =false
                    },
                    
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Thao tác"),
                    new Header("Thời điểm"),
                    new Header("ID tài khoản"),
                    new Header("Tài khoản"),
                    new Header("Mô tả"),
                }
            };
            var rs = await _excelServices.ExportExcel(export, "ThongKeLoiHeThong", false);
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Báo cáo thống kê về lỗi hệ thống.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Export
    }
}
