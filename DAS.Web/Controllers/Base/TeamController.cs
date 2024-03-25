using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ESD.Application.Models.CustomModels;
using ESD.Application.Constants;
using ESD.Application.Enums;
using DAS.Web.Attributes;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TeamController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        //private readonly IRoleServices _groupPerService;
        private readonly IGroupPermissionService _groupPerService;
        private readonly IMapper _mapper;
        private readonly IExcelServices _excelService;
        private readonly IPermissionService _permissionService;

        public TeamController(ITeamService teamService
            , IUserService userService
            //, IRoleServices roleService
            , IGroupPermissionService groupPerService
            , IMapper mapper
            , IExcelServices excel, IPermissionService permissionService)
        {
            _teamService = teamService;
            _userService = userService;
            //_groupPerService = roleService;
            _groupPerService = groupPerService;
            _mapper = mapper;
            _excelService = excel;
            _permissionService = permissionService;
        }

        #region Index & Search
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(TeamCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/Team", "Nhóm người dùng" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            PaginatedList<VMTeam> paging = await _teamService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView(paging);
        }

        [HttpPost]
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> SearchByCondition(TeamCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            PaginatedList<VMTeam> paging = await _teamService.SearchByConditionPagging(condition);

            return PartialView("_IndexTeam", paging);
        }

        [HttpGet]
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Export })]
        public async Task<IActionResult> Export(TeamCondition condition)
        {
            var list = await _teamService.GetListByCondition(condition);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Name"),
                    new Col("Description"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Tên nhóm người dùng"),
                    new Header("Mô tả",70),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục nhóm người dùng");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucNhomNguoiDung.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Index & Search

        #region Detail
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _teamService.GetTeamDetail(id.Value);
            if (model == null)
                return NotFound();

            if (model.GroupPers == null || model.GroupPers.Count() == 0)
                ViewBag.ListGroupPer = new List<SelectListItem>();
            else
                ViewBag.ListGroupPer = model.GroupPers.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = true
                }).ToList();

            var users = await _userService.GetActive();
            if (model.Users == null || model.Users.Count() == 0)
                ViewBag.ListUser = new List<SelectListItem>();
            else
                ViewBag.ListUser = model.Users.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = true
                }).ToList();

            return PartialView("_DetailTeam", model);
        }
        #endregion Detail

        #region Create
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> CreatePopup()
        {
            VMCreateTeam model = new VMCreateTeam();
            var groupPers = await _groupPerService.Gets();
            if (groupPers == null || groupPers.Count() == 0)
                ViewBag.ListGroupPer = new List<SelectListItem>();
            else
                ViewBag.ListGroupPer = groupPers.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                }).ToList();

            var users = await _userService.GetActive();
            if (users == null || users.Count() == 0)
                ViewBag.ListUser = new List<SelectListItem>();
            else
                ViewBag.ListUser = users.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                }).ToList();

            return PartialView("_CreateTeam", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMCreateTeam vmGroup)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            var rs = await _teamService.CreateTeam(vmGroup);
            
            if (rs.Code == CommonConst.Success)
            {
                int idTeam = (int)rs.Data;
                var userIds = await _teamService.GetUserOfTeam(idTeam);
                await _permissionService.UpdateCachePermission(userIds);
            }

            return CustJSonResult(rs);
        }
        #endregion Create

        #region Edit & Update
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Update })]
        // Edit: Users/Edit/5
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var vmGroup = await _teamService.GetTeam(id.Value);
            if (vmGroup == null)
                return NotFound();

            //get data 
            ViewBag.ListGroupPer = await GetSelectListItem("groupPer", vmGroup.IDGroupPerStrs?.ToList());
            ViewBag.ListUser = await GetSelectListItem("user", vmGroup.IDUserStrs?.ToList());

            return PartialView("_EditTeam", vmGroup);
        }

        //POST: Team/Update
        [HttpPost]
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Update(VMEditTeam vmgroup)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            var rs = await _teamService.UpdateTeam(vmgroup);
            if (rs.Code == CommonConst.Success)
            {
                var userIds = await _teamService.GetUserOfTeam(vmgroup.ID);
                await _permissionService.UpdateCachePermission(userIds);
            }

            return CustJSonResult(rs);
        }
        #endregion Edit & Update

        #region Delete
        [HttpPost]
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return JSErrorResult("Vui lòng chọn nhóm người dùng cần xoá!");

            var rs = await _teamService.DeleteTeam(id);
            return CustJSonResult(rs);
        }

        [HttpPost]
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn nhóm người dùng cần xoá!");

            var rs = await _teamService.DeleteMultiTeam(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Common
        [HttpPost]
        [HasPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> UpdateTeamsByGroupPers(int[] groupPerIds, int[] groupIds, string idName, int idRemoved)
        {
            var groups = await _teamService.GetActive();
            IEnumerable<int> idGroups = await _teamService.UpdateTeamsByGroupPers(groupPerIds, groupIds, idRemoved);
            if (groups == null || groups.Count() == 0)
                ViewBag.ListGroup = new List<SelectListItem>();
            else
                ViewBag.ListGroup = groups.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = idGroups != null ? idGroups.Contains(s.ID) : false
                }).ToList();
            ViewBag.IdName = idName;
            return PartialView("_DropdownTeam");
        }
        #endregion Common

        #region Private method
        private async Task<List<SelectListItem>> GetSelectListItem(string type, List<string> selectedValues = null)
        {
            var result = new List<SelectListItem>();
            switch (type)
            {
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
                case "user":
                    var users = await _userService.GetActive();
                    if (!IsExisted(users))
                        return new List<SelectListItem>();
                    else
                        return users.Select(s => new SelectListItem()
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
        #endregion Private mehtod
    }
}