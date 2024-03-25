using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SystemManagementController : BaseController
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

        public SystemManagementController(IUserService userService
            , IOrganServices organService
            , IAgencyServices agencyService
            , IPositionServices positionService
            , IGroupPermissionService groupPerService
            , ITeamService teamService
            , IMapper mapper
            , IExcelServices excel
            , IUserPrincipalService iUserPrincipalService
            , IPermissionService permissionService) {
            _userService = userService;
            _organService = organService;
            _agencyService = agencyService;
            _positionService = positionService;
            _groupPerService = groupPerService;
            _teamService = teamService;
            _excelService = excel;
            _mapper = mapper;
            _iUserPrincipalService = iUserPrincipalService;
            _permissionService = permissionService;
        }

        #region Index & Search
        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> IndexUser(UserCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/SystemManagement/IndexUser", "Người dùng" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            PaginatedList<VMAdminUser> paging = await _userService.SearchAdminUserByConditionPagging(condition);

            //set value from url
            ViewBag.Keyword = condition.Keyword;
            ViewBag.ListStatus = await GetSelectListItem("status", condition.ListStatusStr);
            ViewBag.ListOrgan = await GetSelectListItem("Organ", condition.IDOrganStr);
            ViewBag.ListPosition = await GetSelectListItem("position", condition.IDPositionStr);

            return PartialView(paging);
        }

        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        public async Task<IActionResult> SearchAdminUserByCondition(UserCondition condition)
        {
            //set value from condition
            ViewBag.Keyword = condition.Keyword;
            ViewBag.ListStatus = await GetSelectListItem("status", condition.ListStatusStr);
            ViewBag.ListOrgan = await GetSelectListItem("Organ", condition.IDOrganStr);

            PaginatedList<VMAdminUser> paging = await _userService.SearchAdminUserByConditionPagging(condition);
            return PartialView("_IndexUser", paging);
        }
        #endregion Index & Search

        #region Detail
        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> DetailUserPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var vmUser = await _userService.GetAdminUserDetail(id.Value);
            if (vmUser == null)
                return NotFound();

            //set value from model
            ViewBag.ListOrgan = await GetSelectListItem("Organ", new List<string>() { vmUser.IDOrgan.ToString() });
            ViewBag.ListPosition = await GetSelectListItem("position", new List<string>() { vmUser.IDPosition.ToString() });

            return PartialView("_DetailUser", vmUser);
        }
        #endregion Detail

        #region Create
        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> CreateUserPopup()
        {
            VMCreateAdminUser model = new VMCreateAdminUser();

            //set value from model
            ViewBag.ListOrgan = await GetSelectListItem("Organ", new List<string>() { _iUserPrincipalService.IDOrgan.ToString() });
            ViewBag.ListPosition = await GetSelectListItem("position");

            return PartialView("_CreateUser", model);
        }

        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("ID, AccountName, Password, ConfirmPassword, Name, IdentityNumber, Email, Phone, Address, IDOrgan, IDPosition, Status")] VMCreateAdminUser vmUser)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            var rs = await _userService.CreateAdminUser(vmUser);

            //Update cache permission for user
            if (rs.Code == CommonConst.Success)
            {
                int userId = (int)rs.Data;
                await _permissionService.UpdateCachePermission(userId);
            }

            return CustJSonResult(rs);
        }
        #endregion Create

        #region Edit & Update

        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Update })]
        // Edit: Users/Edit/5
        public async Task<IActionResult> EditUserPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var vmUser = await _userService.GetAdminUser(id.Value);
            if (vmUser == null)
                return NotFound();

            //set value from model
            ViewBag.ListOrgan = await GetSelectListItem("Organ", new List<string>() { vmUser.IDOrgan.ToString() });
            ViewBag.ListPosition = await GetSelectListItem("position", new List<string>() { vmUser.IDPosition.ToString() });

            return PartialView("_EditUser", vmUser);
        }

        //POST: Users/Update
        [HasPermission((int)EnumModule.Code.S9023, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser([Bind("ID, AccountName, Name, IdentityNumber, Email, Phone, Address, IDOrgan, IDPosition, Status")] VMEditAdminUser vmUser)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _userService.UpdateAdminUser(vmUser);

            //Update cache permission for user
            if (rs.Code == CommonConst.Success)
            {
                int userId = (int)rs.Data;
                await _permissionService.UpdateCachePermission(userId);
            }

            return CustJSonResult(rs);
        }
        #endregion Edit & Update

        #region Private method
        private async Task<List<SelectListItem>> GetSelectListItem(string type, List<string> selectedValues = null)
        {
            var result = new List<SelectListItem>();
            switch (type)
            {
                case "Organ":
                    var agencies = await _organService.GetActive(true);
                    if (!IsExisted(agencies))
                        return new List<SelectListItem>();
                    else
                        return agencies.Select(s => new SelectListItem()
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
        #endregion Private mehtod
    }
}