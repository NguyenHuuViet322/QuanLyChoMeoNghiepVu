using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAS.Web.Models;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using ESD.Application.Models.CustomModels;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RoleController : BaseController
    {
        private readonly IRoleServices _roleService;
        private readonly IMapper _mapper;
        private readonly IExcelServices _excelService;
        private readonly IGroupPermissionService _groupPermissionService;

        public RoleController(IRoleServices roleService
            , IGroupPermissionService groupPermissionService
            , IMapper mapper
            , IExcelServices excel)
        {
            _roleService = roleService;
            _groupPermissionService = groupPermissionService;
            _mapper = mapper;
            _excelService = excel;
        }

        #region Index & Search
        public async Task<IActionResult> Index(RoleCondition condition)
        {
            var breadcrum = new Dictionary<string, string>();
            breadcrum.Add("/Role", "Vai trò");
            ViewData["Breadcrumb"] = breadcrum;
            var model = await _roleService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> SearchByCondition(RoleCondition condition)//[Bind("Keyword")] RoleCondition condition
        {
            PaginatedList<VMRole> paging = await _roleService.SearchByConditionPagging(condition);
            return PartialView("_IndexRole", paging);
        }

        [HttpGet]
        public async Task<IActionResult> Export(RoleCondition condition)
        {
            var list = await _roleService.GetListByCondition(condition);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("GroupPermissionName"),
                    new Col("Description"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Tên vai trò"),
                    new Header("Nhóm quyền"),
                    new Header("Mô tả",70),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục vai trò");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucRole.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Index & Search

        #region Detail
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _roleService.GetRoleDetail(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("Details", model);
        }
        #endregion Detail

        #region Create
        public async Task<IActionResult> CreatePopup()
        {
            VMCreateRole vMCreateRole = new VMCreateRole();
            var listGroupPermission = await _groupPermissionService.Gets();
            if (listGroupPermission == null || listGroupPermission.Count() == 0)
            {
                ViewBag.ListGroupPermission = null;
            }
            else
            {
                ViewBag.ListGroupPermission = listGroupPermission.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                });
            }
            return PartialView("Create", vMCreateRole);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ID, Name, Description, IDGroupPermissionStrs")] VMCreateRole vmRole)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _roleService.CreateRole(vmRole);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Edit & Update
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _roleService.GetRole(id.Value);
            if (model == null)
                return NotFound();

            var listGroupPermission = await _groupPermissionService.Gets();
            if (listGroupPermission == null || listGroupPermission.Count() == 0)
                ViewBag.ListGroupPermission = null;
            else
            {
                ViewBag.ListGroupPermission = listGroupPermission.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = model.IDGroupPermissionStrs == null ? false : model.IDGroupPermissionStrs.Contains(s.ID.ToString())
                });
            }
            return PartialView("Edit", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("ID, Name, Description, IDGroupPermissionStrs")] VMCreateRole vmRole)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _roleService.UpdateRole(vmRole);
            return CustJSonResult(rs);
        }
        #endregion Edit & Update

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _roleService.DeleteRole(id);
            return CustJSonResult(rs);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn vai trò cần xoá!");
            var rs = await _roleService.DeleteMultiRole(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        //#region Common
        //[HttpPost]
        //public async Task<IActionResult> UpdateRolesByTeams(int[] roleIds, int[] groupIds, string idName, string type)
        //{
        //    IEnumerable<Role> roles = await _roleService.GetActive();
        //    IEnumerable<int> idRoles = await _roleService.UpdateRolesByTeams(roleIds, groupIds, type);
        //    if (roles == null || roles.Count() == 0)
        //        ViewBag.ListRole = new List<SelectListItem>();
        //    else
        //        ViewBag.ListRole = roles.Select(s => new SelectListItem()
        //        {
        //            Value = s.ID.ToString(),
        //            Text = s.Name,
        //            Selected = idRoles.Contains(s.ID)
        //        }).ToList();
        //    ViewBag.IdName = idName;
        //    return PartialView("_DropdownRole");
        //}
        //#endregion Common
    }
}
