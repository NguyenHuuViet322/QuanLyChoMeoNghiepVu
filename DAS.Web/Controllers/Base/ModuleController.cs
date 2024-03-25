using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ModuleController : BaseController
    {
        #region Properties
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public ModuleController(IMapper mapper
            ,IModuleService moduleService)
        {
            _moduleService = moduleService;
            _mapper = mapper;
        }
        #endregion

        #region Get
        public async Task<IActionResult> Index(ModuleCondition condition)
        {
            var breadcrum = new Dictionary<string, string>();
            breadcrum.Add("/Module", "Menu");
            ViewData["Breadcrumb"] = breadcrum;
            var model = await _moduleService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView(model);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var module = await _moduleService.Get(id.Value);
            if (module == null)
                return NotFound();

            var vmModule = _mapper.Map<VMModule>(module);
            ViewBag.Modules = await _moduleService.GetModuleByTree(vmModule);
            //ViewBag.Icons = await _moduleService.GetListIcon(vmModule);
            return PartialView("Index_Detail", vmModule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(ModuleCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _moduleService.SearchByConditionPagging(condition);
            return PartialView("Index_Modules", model);
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            var vmModule = new VMModule();
            ViewBag.Modules = await _moduleService.GetModuleByTree(vmModule);
            //ViewBag.Icons = await _moduleService.GetListIcon(vmModule);
            ViewBag.ModuleCodes = _moduleService.GetModuleCodeDrd(string.Empty);
            return PartialView("Index_Edit", vmModule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMModule dt)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            //CallService
            var rs = await _moduleService.Create(dt);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _moduleService.Delete(id);
            return CustJSonResult(rs);
        }
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn menu cần xóa!");
            var rs = await _moduleService.Deletes(ids);
            return CustJSonResult(rs);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var module = await _moduleService.Get(id.Value);
            if (module == null)
                return NotFound();

            var vmModule = _mapper.Map<VMModule>(module);
            ViewBag.Modules = await _moduleService.GetModuleByTree(vmModule);
            ViewBag.ModuleCodes =  _moduleService.GetModuleCodeDrd(vmModule.Code.ToString());
            //ViewBag.Icons = await _moduleService.GetListIcon(vmModule);
            return PartialView("Index_Edit", vmModule);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VMModule vmModule)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _moduleService.Update(vmModule);
            return CustJSonResult(rs);
        }

        #endregion
    }
}
