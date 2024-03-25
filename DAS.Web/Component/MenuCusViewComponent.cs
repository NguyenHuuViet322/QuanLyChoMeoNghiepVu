using AutoMapper;
using ESD.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DAS.Web.Models;
using System.Linq;
using ESD.Application.Interfaces.DasKTNN;

namespace DAS.Web.Component
{
    public class MenuCusViewComponent : ViewComponent
    {
        private readonly IModuleService _module;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public MenuCusViewComponent(IModuleService module, IConfiguration configuration, IMapper mapper)
        {
            _module = module;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var moduleExclude = _configuration["ModuleExclude"].Split("|", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            var model = new List<LeftMenuComponentModel>();
            int maxIdLeftMenu = 0;
            var lstMenu = await _module.GetModuleForCurrentUser();
            if (lstMenu != null)
            {
                foreach (var item in lstMenu)
                {

                    var menu = new LeftMenuComponentModel
                    {
                        Id = item.ID + maxIdLeftMenu,
                        Href = item.Url,
                        Name = item.Name,
                        Controller = item.Controller,
                        Action = "",
                        Icon = "rounded-circle bg-primary is-icons-fix " + item.Icon,
                        ParentId = item.ParentId != 0 ? item.ParentId + maxIdLeftMenu : item.ParentId,
                        Pattern = "",
                        Code=item.Code,
                        //RouteName = !item.Url.Contains("/Position") ? item.RouterName : CustomsConfig.RouterDanhMucChuVu,
                        SortOrder = item.SortOrder
                    };

                    model.Add(menu);
                }
            }

            return View(model.AsEnumerable());
        }
    }
}
