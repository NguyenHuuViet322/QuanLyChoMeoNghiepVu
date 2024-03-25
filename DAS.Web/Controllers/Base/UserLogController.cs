using AutoMapper;
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
    public class UserLogController : BaseController
    {
        #region Properties
        private readonly IUserLogServices _systemLog;
        private readonly IExcelServices _excelServices;
        #endregion Properties

        #region Ctor
        public UserLogController(IUserLogServices userLogServices
            , IExcelServices excelServices)
        {
            _systemLog = userLogServices;
            _excelServices = excelServices;
        }
        #endregion Ctor

        #region List & Search
        [HasPermission((int)EnumModule.Code.NKND, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(LogInfoCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/UserLog", "Nhật ký người dùng" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            var model = await _systemLog.GetUserLogByCondition(condition);
            return View(model);
        }

        [HasPermission((int)EnumModule.Code.NKND, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> SearchByConditionPagging(LogInfoCondition condition)
        {
            var model = await _systemLog.GetUserLogByCondition(condition);
            return PartialView("Index_Logs", model);
        }
        #endregion List & Search

        #region Export

        [HasPermission((int)EnumModule.Code.NKND, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> Export(LogInfoCondition condition)
        {
            var list = await _systemLog.GetUserLogByCondition(condition, true);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },

                    new Col("Action"),
                    new Col("CreatedDate"),
                    new Col("UserId"),
                    new Col("Username"),
                    new Col("IPAddress"),

                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Thao tác"),
                    new Header("Thời điểm"),
                    new Header("ID tài khoản"),
                    new Header("Tài khoản"),
                    new Header("Địa chỉ IP"),
                }
            };
            var rs = await _excelServices.ExportExcel(export, "Nhật ký người dùng", false);
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "NhatKyNguoiDung.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Export
    }
}
