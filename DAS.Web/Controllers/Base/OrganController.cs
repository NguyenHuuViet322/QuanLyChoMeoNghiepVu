using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Utility;
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
    public class OrganController : BaseController
    {
        private readonly IOrganServices _organService;
        private readonly IExcelServices _excelService;
        public OrganController(IOrganServices organService, IExcelServices excel)
        {
            _organService = organService;
            _excelService = excel;
        }

        public async Task<IActionResult> GetOrgans(string ids)
        {
            var model = await _organService.GetOrgans(ids);
            return PartialView("_DropdownOrgan", model);
        }

        #region Index & Search
        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(OrganCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/Organ", "Cơ quan" }
            };
            ViewData["Breadcrumb"] = breadcrum;
            var Organs = await _organService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            //ViewBag.Parents = await _organService.GetHierachyOrgan(0);
            return PartialView(Organs);
        }

        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        public async Task<IActionResult> SearchByCondition(OrganCondition condition)
        {
            var paging = await _organService.SearchByConditionPagging(condition);
            return PartialView("_IndexOrgan", paging);
        }

        //[HttpPost]
        //public async Task<IActionResult> GetHierachyOrgan(int id)
        //{
        //    var Organs = await _organService.GetHierachyOrgan(id);
        //    return PartialView("_HierachyIndex", Organs);
        //}

        [HttpGet]
        public async Task<IActionResult> Export(OrganCondition condition)
        {
            var list = await _organService.GetListByCondition(condition);
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
                    new Col("Description")
                },
                Headers = new List<Header>
                {
                    new Header("STT", 8),
                    new Header("Mã cơ quan"),
                    new Header("Tên cơ quan"),
                    new Header("Mô tả")
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục cơ quan");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucCoQuan.xlsx";
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
            var model = await _organService.GetDetail(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("_DetailOrgan", model);
        }
        #endregion Detail

        #region Create
        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Create })]
        public IActionResult CreatePopup()
        {
            VMCreateOrgan vmOrgan = new VMCreateOrgan();
            //vmOrgan.Parents = await _organService.GetParentOrgan(0);

            return PartialView("_CreateOrgan", vmOrgan);
        }

        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        public async Task<IActionResult> Create(VMCreateOrgan vmOrgan)
        {

            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _organService.CreateOrgan(vmOrgan);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Edit & Update
        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _organService.GetOrgan(id.Value);
            if (model == null)
                return NotFound();

            return PartialView("_EditOrgan", model);
        }
        
        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VMEditOrgan vmOrgan)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _organService.UpdateOrgan(vmOrgan);
            return CustJSonResult(rs);
        }
        #endregion Edit & Update

        #region Delete
        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Deleted })]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return JSErrorResult("Vui lòng chọn cơ quan cần xoá!");
            var rs = await _organService.DeleteOrgan(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.M20010, new int[] { (int)EnumPermission.Type.Deleted })]
        [HttpPost]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn cơ quan cần xoá!");
            var rs = await _organService.DeleteMultiOrgan(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Other

        /// <summary>
        /// Dánh sách dm dùng cho cấu hình dm động
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetAgencies()
        {
            var agencies = await _organService.GetActive();
            var options = Utils.RenderOptions(agencies, Utils.GetInt(DATA, "SelectedID"), true, Utils.GetString(DATA, "DefaultText"), Utils.GetString(DATA, "DefaultValue"));
            return JSSuccessResult(string.Empty, options);
        }
        #endregion
    }
}
