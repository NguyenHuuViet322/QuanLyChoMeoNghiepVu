using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAS.Web.Models;
using ESD.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ESD.Application.Models.CustomModels;
using ESD.Utility.LogUtils;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Interfaces.DasKTNN;

namespace DAS.Web.Component
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IModuleService _module;
        private readonly IUserBookMarkServices _userBookMark;
        public MenuViewComponent(IModuleService module, IUserBookMarkServices userBookMark)
        {
            _module = module;
            _userBookMark = userBookMark;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listBookMark = ((await _userBookMark.GetBookMark())?.Modules) ?? new List<int>();
            var dictBookmark = new Dictionary<string, int[]>();
            var model = await _module.GetsActive();
            var dicMenu = model.Where(x => x.Url != null && x.Url != "" && x.Url != "#").ToDictionary(x => x.Url.ToLower(), v => v.Name);
            var dicModule = model.Where(x => x.Url != null && x.Url != "" && x.Url != "#").ToDictionary(x => x.Url.ToLower(), v => v.ID);
            foreach (var item in dicMenu)
            {
                string keyurl = item.Key;
                int idModule = dicModule.GetValueOrDefault(keyurl);
                int statusBookMark = listBookMark.Contains(idModule) ? 1 : 0;
                dictBookmark.Add(keyurl, new int[] { idModule, statusBookMark });
            }
            TempData["ModuleBookmark"] = dictBookmark;
            if (ViewData["Breadcrumb"] != null && ViewData["Breadcrumb"] is Dictionary<string, string>)
            {
                var listKeyBreadCrumb = ((Dictionary<string, string>)ViewData["Breadcrumb"]).Keys.ToList();
                foreach (string key in listKeyBreadCrumb)
                {
                    if (dicMenu.ContainsKey(key.ToLower()))
                    {
                        ((Dictionary<string, string>)ViewData["Breadcrumb"])[key] = dicMenu.GetValueOrDefault(key.ToLower());
                    }
                }
            }
            return View(model);
        }
    }
}
