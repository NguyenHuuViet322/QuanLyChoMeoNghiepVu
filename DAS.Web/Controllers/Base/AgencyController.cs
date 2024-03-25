using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.CustomModels;
using DAS.Web.Attributes;
using ESD.Application.Enums;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AgencyController : BaseController
    {
        private readonly IAgencyServices _agencyService;
        private readonly IOrganServices _organService;
        private readonly IExcelServices _excelService;
        private readonly IProfileTemplateServices _profileTemplateService;
        public AgencyController(IAgencyServices agencyService
            , IOrganServices organService
            , IExcelServices excel) 
        {
            _agencyService = agencyService;
            _organService = organService;
            _excelService = excel;
        }

        public async Task<IActionResult> GetByOrganId(int[] organIds, int[] ids, string idName)
        {
            var agencies = await _agencyService.GetAgencys(organIds, ids);
            if (agencies == null || agencies.Count() == 0)
                ViewBag.ListAgency = new List<SelectListItem>();
            else
                ViewBag.ListAgency = agencies.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = ids == null ? false : ids.Contains(s.ID)
                }).ToList();
            ViewBag.IdName = idName;
            return PartialView("_DropdownAgency");
        }

        #region Index & Search
        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(AgencyCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/Agency", "Đơn vị" }
            };
            ViewData["Breadcrumb"] = breadcrum; //Breadcrumb tạm thời
            var agencies = await _agencyService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            ViewBag.Parents = await _agencyService.GetHierachyAgency(0);
            return PartialView(agencies);
        }

        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        public async Task<IActionResult> SearchByCondition(AgencyCondition condition)//[Bind("Keyword")] RoleCondition condition
        {
            var paging = await _agencyService.SearchByConditionPagging(condition);
            return PartialView("_IndexAgency", paging);
        }

        [HttpPost]
        public async Task<IActionResult> GetHierachyAgency(int id)
        {
            var organs = await _agencyService.GetHierachyAgency(id);
            return PartialView("_HierachyIndex", organs);
        }

        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> Export(AgencyCondition condition)
        {
            var list = await _agencyService.GetListByCondition(condition);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("Name"),
                    new Col("ParentName"),
                    new Col("OrganName")
                },
                Headers = new List<Header>
                {
                    new Header("STT", 8),
                    new Header("Mã đơn vị"),
                    new Header("Tên đơn vị"),
                    new Header("Đơn vị cha"),
                    new Header("Cơ quan")
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục đơn vị");
            if (rs is ServiceResultError)
                return NotFound();
            else
            {
                var fileName = "DanhMucDonVi.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion Index & Search

        #region Detail
        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _agencyService.GetDetail(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("_DetailAgency", model);
        }
        #endregion Detail

        #region Create
        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> CreatePopup()
        {
            VMCreateAgency vmAgency = new VMCreateAgency();
            var listOrgan = await _organService.GetActive();
            if (listOrgan == null || listOrgan.Count() == 0)
                ViewBag.ListOrgan = new List<SelectListItem>();
            else
                ViewBag.ListOrgan = listOrgan.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                }).ToList();
            vmAgency.Parents = await _agencyService.GetParentAgency(0);
            return PartialView("_CreateAgency", vmAgency);
        }

        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        public async Task<IActionResult> Create(VMCreateAgency vmAgency)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _agencyService.CreateAgency(vmAgency);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Edit & Update
        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _agencyService.GetAgency(id.Value);
            if (model == null)
                return NotFound();

            var listOrgan = await _organService.GetActive();
            if (listOrgan == null || listOrgan.Count() == 0)
                ViewBag.ListOrgan = new List<SelectListItem>();
            else
                ViewBag.ListOrgan = listOrgan.Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name,
                    Selected = s.ID == (model.IDOrgan ?? 0)
                }).ToList();

            return PartialView("_EditAgency", model);
        }

        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        public async Task<IActionResult> Update(VMEditAgency vmAgency)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _agencyService.UpdateAgency(vmAgency);
            return CustJSonResult(rs);
        }
        #endregion Edit & Update

        #region Delete
        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Deleted })]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return JSErrorResult("Vui lòng chọn đơn vị cần xoá!");
            var rs = await _agencyService.DeleteAgency(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.M20020, new int[] { (int)EnumPermission.Type.Deleted })]
        [HttpPost]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn đơn vị cần xoá!");
            var rs = await _agencyService.DeleteMultiAgency(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete
    }
}
