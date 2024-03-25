using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SystemConfigController : BaseController
    {
        #region Properties
        private readonly ISystemConfigServices _systemConfigServices;
        #endregion
        #region Ctor
        public SystemConfigController(ISystemConfigServices systemConfig)
        {
            _systemConfigServices = systemConfig;

        }
        #endregion

        #region List & Search
        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(SystemConfigCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _systemConfigServices.SearchByConditionPagging(condition);
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> SearchByConditionPagging(SystemConfigCondition condition)//[Bind("Keyword")] RoleCondition condition
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _systemConfigServices.SearchByConditionPagging(condition);
            return PartialView("Index_SystemConfigs", model);
        }

        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _systemConfigServices.GetSystemConfig(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("Index_Detail", model);
        }
        #endregion  List & Search

        #region Create
        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> CreatePopup()
        {
            VMUpdateSystemConfig model = new VMUpdateSystemConfig();
            return PartialView("Index_Edit", model);
        }


        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMUpdateSystemConfig model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _systemConfigServices.CreateSystemConfig(model);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Delete
        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Deleted })]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _systemConfigServices.DeleteSystemConfig(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Deleted })]
        [HttpPost]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn tham số cần xóa");
            var rs = await _systemConfigServices.Deletes(ids);
            return CustJSonResult(rs);
        }
        #endregion

        #region Update
        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _systemConfigServices.GetSystemConfig(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("Index_Edit", model);
        }

        [HasPermission((int)EnumModule.Code.CHTS, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VMUpdateSystemConfig model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _systemConfigServices.UpdateSystemConfig(model);
            return CustJSonResult(rs);
        }
        #endregion Update
    }
}
