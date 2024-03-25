using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TemplateController : BaseController
    {
        private readonly IStgFileClientService _stgFileClientService;
        private readonly ITemplateServices _templateServices;
        public TemplateController(IStgFileClientService stgFileClientService, ITemplateServices templateServices)
        {
            _stgFileClientService = stgFileClientService;
            _templateServices = templateServices;
        }
        public async Task<IActionResult> Index(TemplateCondition condition)
        {
            var result = await _templateServices.SearchListTemplateConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView(result);
        }

        public async Task<IActionResult> SearchByCondition(TemplateCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;

            var result = await _templateServices.SearchListTemplateConditionPagging(condition);
            return PartialView("Index_Template", result);
        }

        public async Task<IActionResult> CreatePopup()
        {
            return PartialView("_CreateNewTemplate");
        }

        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null || id.Value == 0)
                return NotFound();
            var model = await _templateServices.GetTemplate(id.Value);
            if (model == null)
                return NotFound();
            model.TemplateParam = await _templateServices.GetTemplateParam(id.Value);
            return PartialView("_EditTemplate", model);
        }

        public async Task<IActionResult> CreateNewTemplate(VMTemplate model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            if (!CheckTemplateParamValidate(model, out string mss, out List<object> errObj))
            {
                if (errObj.IsNotEmpty())
                    return JSErrorResult(mss, errObj);
                return JSErrorResult(mss);
            }
            var result = await _templateServices.Create(model);
            return CustJSonResult(result);
        }

        public async Task<IActionResult> UpdateTemplate(VMTemplate model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            if (!CheckTemplateParamValidate(model, out string mss, out List<object> errObj))
            {
                if (errObj.IsNotEmpty())
                    return JSErrorResult(mss, errObj);
                return JSErrorResult(mss);
            }
            var result = await _templateServices.Update(model);
            return CustJSonResult(result);
        }

        #region Private Function
        private bool CheckTemplateParamValidate(VMTemplate model, out string mss, out List<object> errObj)
        {
            errObj = new List<object>();
            mss = string.Empty;
            if (model.TemplateParam.IsNotEmpty())
            {
                var i = 0;
                foreach (var item in model.TemplateParam)
                {
                    if (item.Code.IsEmpty())
                    {
                        errObj.Add(new
                        {
                            Field = $"TemplateParam[{i}].Code",
                            Mss = "Trường thông tin không được để trống"
                        });
                    }

                    i++;
                }

                var codes = model.TemplateParam.Where(n => n.Code.IsNotEmpty() && (n.IDTemplate == model.ID || model.ID == 0)).Select(n => n.Code).ToList();
                if (codes.Count() > codes.Distinct().Count())
                {
                    mss = "Mã trường thông tin đã bị trùng";
                }
            }
            return mss.IsEmpty() && errObj.IsEmpty();
        }
        #endregion
    }
}
