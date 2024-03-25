using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
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
    public class BackupAndRestoreController : BaseController
    {
        private readonly IStgFileClientService _stgFileClientService;
        public BackupAndRestoreController(IStgFileClientService stgFileClientService)
        {
            _stgFileClientService = stgFileClientService;
        }
        public async Task<IActionResult> Index()
        {
            //var paging = await _stgFileClientService.SearchListConditionPagging(condition);
            ViewBag.lstDB = await GetActiveDB();
            return View();
        }

        //public async Task<IActionResult> SearchByCondition(StgFileCondition condition)
        //{
        //    ViewBag.CreateDateStr = condition.CreateDateStr;
        //    var paging = await _stgFileClientService.SearchListConditionPagging(condition);
        //    return PartialView("List_Database", paging);
        //}

        public async Task<IActionResult> BackupData()
        {
            var result = await _stgFileClientService.BackupData();
            return CustJSonResult(result);
        }

        public async Task<IActionResult> RestoreData(VMStgFile model)
        {
            if (model == null || model.ID == 0)
                return NotFound();

            var result = await _stgFileClientService.RestoreData(model);
            return CustJSonResult(result);
        }

        private async Task<List<SelectListItem>> GetActiveDB()
        {
            var database = await _stgFileClientService.GetActiveDB();
            if (!IsExisted((List<StgFile>)database.Data))
                return new List<SelectListItem>();
            else
                return ((List<StgFile>)database.Data).Select(s => new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.FileName,
                }).ToList();
        }

        private bool IsExisted<T>(IEnumerable<T> entity)
        {
            if (entity == null || entity.Count() == 0)
                return false;
            return true;
        }
    }
}
