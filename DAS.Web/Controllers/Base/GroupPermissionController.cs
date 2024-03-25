using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Models.DAS;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GroupPermissionController : BaseController
    {
        #region Properties

        private readonly IGroupPermissionService _groupPermissionService;
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        private readonly IExcelServices _excelService;
        private readonly IDistributedCache _cache;
        private readonly IModuleService _module;

        #endregion Properties

        #region Ctor

        public GroupPermissionController(IMapper mapper
          , IGroupPermissionService groupPermissionService
          , IPermissionService permissionService
          , IDistributedCache cache
          , IModuleService module
          , IExcelServices excel)
        {
            _groupPermissionService = groupPermissionService;
            _permissionService = permissionService;
            _mapper = mapper;
            _cache = cache;
            _module = module;
            _excelService = excel;

        }

        #endregion Ctor

        #region List

        // GET: Users
        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(PermissionGroupCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/GroupPermission", "Nhóm quyền" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            var model = await _groupPermissionService.SearchByConditionPagging(condition);
            SetTitle("Quản lý nhóm quyền");
            ViewBag.Keyword = condition.Keyword;
            return PartialView(model);

        }

        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(PermissionGroupCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _groupPermissionService.SearchByConditionPagging(condition);
            return PartialView("Index_GroupPermissions", model);
        }

        // GET: Users/Details/5
        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var groupPermission = await _groupPermissionService.Get(id.Value);
            if (groupPermission == null)
                return NotFound();

            var vmUser = _mapper.Map<VMGroupPermision>(groupPermission);
            vmUser.Permissions = await _permissionService.GetPermissionForEditGroupPer(id.Value);
            return PartialView("Index_Detail", vmUser);
        }

        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> Export(PermissionGroupCondition condition)
        {
            var list = await _groupPermissionService.GetListByCondition(condition);
            var export = new ExportExtend
            {
                Data = list.Select(n => new
                {
                    Name = n.Name,
                    Description = n.Description,
                    Status = n.Status == (int)EnumCommon.Status.Active ? "Hoạt động" : "Ngưng hoạt động"
                }).Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Name"),
                    new Col("Description"),
                    new Col("Status"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Tên nhóm quyền"),
                    new Header("Mô tả"),
                    new Header("Trạng thái"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục nhóm quyền");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucNhomQuyen.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion List

        #region Edit

        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var groupPermission = await _groupPermissionService.Get(id.Value);
            if (groupPermission == null)
                return NotFound();

            var vmUser = _mapper.Map<VMGroupPermision>(groupPermission);
            vmUser.Permissions = await _permissionService.GetPermissionForEditGroupPer(id.Value);
            vmUser.Modules = await _module.GetsActive();

            //var basePermissions = await _permissionService.GetListBasePermission();
            //if (basePermissions.IsEmpty())
            //    ViewBag.ListBaseGroupPermission = new List<SelectListItem>();
            //else
            //    ViewBag.ListBaseGroupPermission = basePermissions.Select(s => new SelectListItem()
            //    {
            //        Value = s.ID.ToString(),
            //        Text = s.Name
            //    }).ToList();
            return PartialView("Index_Update", vmUser);
        }

        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VMGroupPermision vMGroupPermision)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _groupPermissionService.Update(vMGroupPermision);

            //Update cache for user
            if (rs.Code == CommonConst.Success)
            {
                await _permissionService.UpdateCachePermissionByIdGroupPer(vMGroupPermision.ID);
            }

            return CustJSonResult(rs);
        }

        #endregion Edit

        #region Create

        // GET: Users/Create
        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            var vmGroupPermision = new VMGroupPermision
            {
                Permissions = await _permissionService.GetPermissionWithModule(),
                Modules = await _module.GetsActive(),
                Status = (int)EnumCommon.Status.Active
            };
            //var vmGroupPermision = new VMGroupPermision
            //{
            //    Permissions = await _permissionService.GetPermissionWithModule()
            //};
            //var basePermissions = await _permissionService.GetListBasePermission();
            //if (basePermissions.IsEmpty())
            //    ViewBag.ListBaseGroupPermission = new List<SelectListItem>();
            //else
            //    ViewBag.ListBaseGroupPermission = basePermissions.Select(s => new SelectListItem()
            //    {
            //        Value = s.ID.ToString(),
            //        Text = s.Name
            //    }).ToList();
            return PartialView("Index_Update", vmGroupPermision);
        }

        //POST: Users/Create
        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMGroupPermision dt)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _groupPermissionService.Create(dt);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Delete
        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _groupPermissionService.Delete(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn nhóm quyền cần xoá!");
            var rs = await _groupPermissionService.Delete(ids);
            return CustJSonResult(rs);
        }

        #endregion Delete

        #region Common
        [HasPermission((int)EnumModule.Code.S9030, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        public async Task<IActionResult> UpdateGroupPersByTeams(int[] groupPerIds, int[] groupIds, string idName, string type, int idRemoved)
        {
            IEnumerable<GroupPermission> groupPers = await _groupPermissionService.Gets();
            IEnumerable<int> idGroupPers = await _groupPermissionService.UpdateGroupPersByTeams(groupPerIds, groupIds, type, idRemoved);
            if (groupPers == null || groupPers.Count() == 0)
                ViewBag.ListGroupPer = new List<SelectListItem>();
            else
                ViewBag.ListGroupPer = groupPers.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = idGroupPers == null ? false : idGroupPers.Contains(s.ID)
                }).ToList();
            ViewBag.IdName = idName;
            return PartialView("_DropdownGroupPer");
        }
        #endregion Common

        #region Private method
        [HttpPost]
        public async Task<IActionResult> FillDataBaseGroupPermission(int? idGroupPer)
        {
            var groupPermission = await _groupPermissionService.Get(idGroupPer.Value);
            var vmGroupPermission = _mapper.Map<VMGroupPermision>(groupPermission);
            vmGroupPermission.Permissions = await _permissionService.GetPermissionForEditGroupPer(idGroupPer.Value);
            return PartialView("_ModuleTree", vmGroupPermission.Permissions);
        }
        #endregion Private method
    }
}