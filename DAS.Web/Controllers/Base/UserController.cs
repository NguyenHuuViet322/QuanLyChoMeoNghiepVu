using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using Microsoft.AspNetCore.Authorization;
using ESD.Application.Enums;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using iTextSharp.text.pdf;
//using System.IO;
//using iTextSharp.text;
//using Header = ESD.Application.Models.CustomModels.Header;
//using SelectPdf;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IOrganServices _organService;
        private readonly IAgencyServices _agencyService;
        private readonly IPositionServices _positionService;
        //private readonly IRoleServices _roleService;
        private readonly IGroupPermissionService _groupPerService;
        private readonly ITeamService _teamService;
        private readonly IExcelServices _excelService;
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _iUserPrincipalService;
        private readonly IPermissionService _permissionService;

        public UserController(IUserService userService
            , IOrganServices organService
            , IAgencyServices agencyService
            , IPositionServices positionService
            //, IRoleServices roleService
            , IGroupPermissionService groupPerService
            , ITeamService teamService
            , IMapper mapper
            , IExcelServices excel
            , IUserPrincipalService iUserPrincipalService
            , IPermissionService permissionService)
        {
            _userService = userService;
            _organService = organService;
            _agencyService = agencyService;
            _positionService = positionService;
            //_roleService = roleService;
            _groupPerService = groupPerService;
            _teamService = teamService;
            _excelService = excel;
            _mapper = mapper;
            _iUserPrincipalService = iUserPrincipalService;
            _permissionService = permissionService;
        }

        #region Index & Search
        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(UserCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/User", "Người dùng" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            PaginatedList<VMUser> paging = await _userService.SearchByConditionPagging(condition);

            //set value from url
            ViewBag.Keyword = condition.Keyword;
            ViewBag.ListStatus = await GetSelectListItem("status", condition.ListStatusStr);
            ViewBag.ListOrgan = await GetSelectListItem("Organ", condition.IDOrganStr);
            ViewBag.ListAgency = await GetSelectListItem("Agency", condition.IDAgencyStr, condition.IDOrganStr);
            ViewBag.ListPosition = await GetSelectListItem("position", condition.IDPositionStr);

            return PartialView(paging);
        }

        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        public async Task<IActionResult> SearchByCondition(UserCondition condition)
        {
            //set value from condition
            ViewBag.Keyword = condition.Keyword;
            ViewBag.ListStatus = await GetSelectListItem("status", condition.ListStatusStr);
            ViewBag.ListOrgan = await GetSelectListItem("Organ", condition.IDOrganStr);
            ViewBag.ListAgency = await GetSelectListItem("Agency", condition.IDAgencyStr, condition.IDOrganStr);
            ViewBag.ListPosition = await GetSelectListItem("position", condition.IDPositionStr);

            PaginatedList<VMUser> paging = await _userService.SearchByConditionPagging(condition);
            return PartialView("_IndexUser", paging);
        }

        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Read })]
        [HttpGet]
        public async Task<IActionResult> Export(UserCondition condition)
        {
            //var enumUser = Enum.GetValues(typeof(EnumCommon.Status))
            //    .Cast<object>()
            //    .ToDictionary(k => (int)k, v => StringUltils.GetEnumDescription((EnumCommon.Status)v));

            var enumUser = StringUltils.GetEnumDictionary<EnumCommon.Status>();
            var list = await _userService.GetListByCondition(condition);
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
                    new Col("OrganName"),
                    new Col("AgencyName"),
                    new Col("PositionName"),
                    new Col{
                        Field = "Status",
                        DataType =2,
                        DefineEnum = enumUser,
                    },
                    new Col("Email"),
                    new Col("Phone"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Tên tài khoản"),
                    new Header("Họ và tên"),
                    new Header("Cơ quan"),
                    new Header("Đơn vị"),
                    new Header("Chức vụ"),
                    new Header("Trạng thái"),
                    new Header("Email"),
                    new Header("Số điện thoại"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục người dùng");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucNguoiDung.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
//        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Read })]
//        [HttpGet]
//        public async Task<IActionResult> ExportPdf(UserCondition condition)
//        {
//            //var enumUser = Enum.GetValues(typeof(EnumCommon.Status))
//            //    .Cast<object>()
//            //    .ToDictionary(k => (int)k, v => StringUltils.GetEnumDescription((EnumCommon.Status)v));

//            var enumUser = StringUltils.GetEnumDictionary<EnumCommon.Status>();
//            var list = await _userService.GetListByCondition(condition);
//            var export = new ExportExtend
//            {
//                Data = list.Cast<dynamic>().ToList(),
//                Cols = new List<Col>
//                {
//                    new Col{
//                        DataType = 5
//                    },
//                    new Col("AccountName"),
//                    new Col("Name"),
//                    new Col("OrganName"),
//                    new Col("AgencyName"),
//                    new Col("PositionName"),
//                    new Col{
//                        Field = "Status",
//                        DataType =2,
//                        DefineEnum = enumUser,
//                    },
//                    new Col("Email"),
//                    new Col("Phone"),
//                },
//                Headers = new List<Header>
//                {
//                    new Header("STT",8),
//                    new Header("Tên tài khoản"),
//                    new Header("Họ và tên"),
//                    new Header("Cơ quan"),
//                    new Header("Đơn vị"),
//                    new Header("Chức vụ"),
//                    new Header("Trạng thái"),
//                    new Header("Email"),
//                    new Header("Số điện thoại"),
//                }
//            };
//            HtmlToPdf dekt = new HtmlToPdf();
//            dekt.Options.WebPageWidth = 1920;
//            dekt.Options.MinPageLoadTime = 2;
//            var html = @"<html>
// <body>
//  Hello World from selectpdf.com.
// </body>
//</html>
//";
//            var pdf = dekt.ConvertUrl(@"https://www.roundthecode.com/");
//            //            var pdf =dekt dekt.ConvertHtmlString(@"<> <table>
//            //  <tr>
//            //    <th>Company</th>
//            //    <th>Contact</th>
//            //    <th>Country</th>
//            //  </tr>
//            //  <tr>
//            //    <td>Alfreds Futterkiste</td>
//            //    <td>Maria Anders</td>
//            //    <td>Germany</td>
//            //  </tr>
//            //  <tr>
//            //    <td>Centro comercial Moctezuma</td>
//            //    <td>Francisco Chang</td>
//            //    <td>Mexico</td>
//            //  </tr>
//            //</table>");
//            var pdfbyte = pdf.Save();
//            var fileName = "DanhMucNguoiDung.pdf";
//            var contentType = "application/pdf";
//            return File(pdfbyte, contentType, fileName);
//        }
        #endregion Index & Search

        #region Detail
        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var vmUser = await _userService.GetUserDetail(id.Value);
            if (vmUser == null)
                return NotFound();

            //set value from model
            ViewBag.ListOrgan = await GetSelectListItem("Organ", new List<string>() { vmUser.IDOrgan.ToString() });
            ViewBag.ListAgency = await GetSelectListItem("Agency", new List<string>() { vmUser.IDAgency.ToString() }, new List<string>() { vmUser.IDOrgan.ToString() });
            ViewBag.ListPosition = await GetSelectListItem("position", new List<string>() { vmUser.IDPosition.ToString() });
            ViewBag.ListGroupPer = await GetSelectListItem("groupPer", vmUser.IDGroupPerStrs?.ToList());
            ViewBag.ListTeam = await GetSelectListItem("team", vmUser.IDTeamStrs?.ToList());

            return PartialView("_DetailUser", vmUser);
        }
        #endregion Detail

        #region Create
        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> CreatePopup()
        {
            VMCreateUser model = new VMCreateUser();

            //set value from model
            ViewBag.ListOrgan = await GetSelectListItem("Organ", new List<string>() { _iUserPrincipalService.IDOrgan.ToString() });
            ViewBag.ListAgency = await GetSelectListItem("Agency");
            ViewBag.ListPosition = await GetSelectListItem("position");
            ViewBag.ListGroupPer = await GetSelectListItem("groupPer");
            ViewBag.ListTeam = await GetSelectListItem("team");

            return PartialView("_CreateUser", model);
        }

        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID, AccountName, Password, ConfirmPassword, Name, IdentityNumber, Email, Phone, Address, StartDateStr, EndDateStr, IDOrgan, IDAgency, IDPosition, IDGroupPerStrs, IDTeamStrs, Status, HasOrganPermission")] VMCreateUser vmUser)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            var rs = await _userService.CreateUser(vmUser);

            //Update cache permission for user
            if (rs.Code == CommonConst.Success)
            {
                int userId = (int)rs.Data;
                await _permissionService.UpdateCachePermission(userId);
                await _userService.UpdateCacheUser(userId);
            }

            return CustJSonResult(rs);
        }
        #endregion Create
        #region Delete
        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _userService.Delete(id);
            return CustJSonResult(rs);
        }    
        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            var rs = await _userService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion

        #region Edit & Update
        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Update })]
        // Edit: Users/Edit/5
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var vmUser = await _userService.GetUser(id.Value);
            if (vmUser == null)
                return NotFound();

            //set value from model
            ViewBag.ListOrgan = await GetSelectListItem("Organ", new List<string>() { vmUser.IDOrgan.ToString() });
            ViewBag.ListAgency = await GetSelectListItem("Agency", new List<string>() { vmUser.IDAgency.ToString() }, new List<string>() { vmUser.IDOrgan.ToString() });
            ViewBag.ListPosition = await GetSelectListItem("position", new List<string>() { vmUser.IDPosition.ToString() });
            ViewBag.ListGroupPer = await GetSelectListItem("groupPer", vmUser.IDGroupPerStrs?.ToList());
            ViewBag.ListTeam = await GetSelectListItem("team", vmUser.IDTeamStrs?.ToList());

            return PartialView("_EditUser", vmUser);
        }

        [HasPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Update })]
        //POST: Users/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("ID, AccountName, Name, IdentityNumber, Email, Phone, Address, StartDateStr, EndDateStr, IDOrgan, IDAgency, IDPosition, IDGroupPerStrs, IDTeamStrs, Status, HasOrganPermission")] VMEditUser vmUser)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            var rs = await _userService.UpdateUser(vmUser);

            //Update cache permission for user
            if (rs.Code == CommonConst.Success)
            {
                await _permissionService.UpdateCachePermission(vmUser.ID);
                await _userService.UpdateCacheUser(vmUser.ID);
            }


            return CustJSonResult(rs);
        }
        #endregion Edit & Update
        
        public async Task<IActionResult> Sync()
        {
            var rs = new ServiceResultSuccess("Đồng bộ người dùng thành công!");
            //Todo
            return CustJSonResult(rs);
        }

        #region Private method
        private async Task<List<SelectListItem>> GetSelectListItem(string type, List<string> selectedValues = null, List<string> parentIds = null)
        {
            var result = new List<SelectListItem>();
            switch (type)
            {
                case "Organ":
                    var agencies = await _organService.GetActive();
                    if (!IsExisted(agencies))
                        return new List<SelectListItem>();
                    else
                        return agencies.Select(s => new SelectListItem()
                        {
                            Value = s.ID.ToString(),
                            Text = s.Name,
                            Selected = selectedValues == null ? false : selectedValues.Contains(s.ID.ToString())
                        }).ToList();
                case "Agency":
                    var Agencys = await _agencyService.GetAgencyByUser();
                    if (!IsExisted(Agencys))
                        return new List<SelectListItem>();

                    Agencys = IsExisted(parentIds) ? Agencys.Where(m => parentIds.Contains(m.IDOrgan.ToString())) : null;
                    if (!IsExisted(Agencys))
                        return new List<SelectListItem>();
                    else
                        return Agencys.Select(s => new SelectListItem()
                        {
                            Value = s.ID.ToString(),
                            Text = s.Name,
                            Selected = selectedValues == null ? false : selectedValues.Contains(s.ID.ToString())
                        }).ToList();
                case "position":
                    var positions = await _positionService.GetsActive();
                    if (!IsExisted(positions))
                        return new List<SelectListItem>();
                    else
                        return positions.Select(s => new SelectListItem()
                        {
                            Value = s.ID.ToString(),
                            Text = s.Name,
                            Selected = selectedValues == null ? false : selectedValues.Contains(s.ID.ToString())
                        }).ToList();
                case "groupPer":
                    var groupPers = await _groupPerService.Gets();
                    if (!IsExisted(groupPers))
                        return new List<SelectListItem>();
                    else
                        return groupPers.Select(s => new SelectListItem()
                        {
                            Value = s.ID.ToString(),
                            Text = s.Name,
                            Selected = selectedValues == null ? false : selectedValues.Contains(s.ID.ToString())
                        }).ToList();
                case "status":
                    result = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = ((int)EnumCommon.Status.Active).ToString(),
                            Text = StringUltils.GetEnumDescription(EnumCommon.Status.Active),
                            Selected = selectedValues == null ? false : selectedValues.Contains(((int)EnumCommon.Status.Active).ToString())
                        },
                        new SelectListItem
                        {
                            Value = ((int)EnumCommon.Status.InActive).ToString(),
                            Text = StringUltils.GetEnumDescription(EnumCommon.Status.InActive),
                            Selected = selectedValues == null ? false : selectedValues.Contains(((int)EnumCommon.Status.InActive).ToString())
                        }
                    };
                    return result;
                case "team":
                    var teams = await _teamService.GetActive();
                    if (!IsExisted(teams))
                        return new List<SelectListItem>();
                    else
                        return teams.Select(s => new SelectListItem()
                        {
                            Value = s.ID.ToString(),
                            Text = s.Name,
                            Selected = selectedValues == null ? false : selectedValues.Contains(s.ID.ToString())
                        }).ToList();
                default:
                    break;
            }
            return result;
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }
        #endregion Private method
    }
}