using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Interfaces.DasKTNN;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.DasKTNN;
using ESD.Application.Services;
using ESD.Utility;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    public class NotificationConfigController : BaseController
    {
        #region Properties

        private readonly INotificationConfigService _notificationConfigService;
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        private readonly IExcelServices _excelService;
        private readonly IDistributedCache _cache;
        private readonly IModuleService _module;

        #endregion Properties

        #region Ctor

        public NotificationConfigController(IMapper mapper
          , INotificationConfigService notificationConfigService
          , IPermissionService permissionService
          , IDistributedCache cache
          , IModuleService module
          , IExcelServices excel)
        {
            _notificationConfigService = notificationConfigService;
            _permissionService = permissionService;
            _mapper = mapper;
            _cache = cache;
            _module = module;
            _excelService = excel;
        }

        #endregion Ctor
        public async Task<IActionResult> Index(NotificationConfigCondition condition)
        {
            var model = await _notificationConfigService.SearchByCondition(condition);
            ViewBag.Keyword = condition.Keyword;
            return View(model);
        }

        public async Task<IActionResult> SerchCondition(NotificationConfigCondition condition)
        {
            var model = await _notificationConfigService.SearchByCondition(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView("Index_Records", model);
        }

        [HttpPost]
        public async Task<IActionResult> Update()
        {
            var rs = await _notificationConfigService.Update(DATA);
            return CustJSonResult(rs);
        }
    }
}
