using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OrganConfigController : BaseController
    {
        #region Properties
        private readonly IOrganConfigServices _OrganConfigServices;
        #endregion
        #region Ctor
        public OrganConfigController(IOrganConfigServices OrganConfig)
        {
            _OrganConfigServices = OrganConfig;

        }
        #endregion

        #region List & Search
        public async Task<IActionResult> Index(OrganConfigCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _OrganConfigServices.SearchByConditionPagging(condition);
            return PartialView(model);
        }
        public async Task<IActionResult> SearchByConditionPagging(OrganConfigCondition condition)//[Bind("Keyword")] RoleCondition condition
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _OrganConfigServices.SearchByConditionPagging(condition);
            return PartialView("Index_OrganConfigs", model);
        }
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _OrganConfigServices.GetOrganConfig(id.Value);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("Index_Detail", model);
        }
        #endregion  List & Search

        #region Create
        public async Task<IActionResult> CreatePopup()
        {
            VMUpdateOrganConfig model = new VMUpdateOrganConfig();
            return PartialView("Index_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMUpdateOrganConfig model)
        {
            if (!ModelState.IsValid)
            {
                return JSErrorModelStateByLine();
            }

            var rs = await _OrganConfigServices.CreateOrganConfig(model);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _OrganConfigServices.DeleteOrganConfig(id);
            return CustJSonResult(rs);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return JSErrorResult("Vui lòng chọn tham số cần xóa");
            }

            var rs = await _OrganConfigServices.Deletes(ids);
            return CustJSonResult(rs);
        }
        #endregion

        #region Update
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _OrganConfigServices.GetOrganConfig(id.Value);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("Index_Edit", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VMUpdateOrganConfig model)
        {
            if (!ModelState.IsValid)
            {
                return JSErrorModelStateByLine();
            }

            var rs = await _OrganConfigServices.UpdateOrganConfig(model);
            return CustJSonResult(rs);
        }
        #endregion Update
    }
}
