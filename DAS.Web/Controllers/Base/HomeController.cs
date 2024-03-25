using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ESD.Utility.LogUtils;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DASNotify;
using ESD.Application.Models.CustomModels;
using ESD.Application.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using ESD.Domain.Enums;
using ESD.Utility;
using System.Linq;
using ESD.Application.Enums;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : BaseController
    {
        private readonly IHomeServices _homeService;
        private readonly IModuleService _module;
        private readonly IUserBookMarkServices _userBookMark;
        private readonly IUserPrincipalService _userPrincipalService;
        public HomeController(IModuleService module, IHomeServices homeService, IUserBookMarkServices userBookMark, IUserPrincipalService userPrincipalService)
        {
            _module = module;
            _homeService = homeService;
            _userBookMark = userBookMark;
            _userPrincipalService = userPrincipalService;
        }

        public async Task<IActionResult> Index(HomeCondition condition)
        {
            var lstMenu = await _module.GetModuleForCurrentUser();
            if (Utils.IsNotEmpty(lstMenu) && lstMenu.Any(n => n.Code == (int)EnumModule.Code.DongVatNghiepVu))
            {
                return Redirect("/DongVatNghiepVu");
            }
            //var plans = await _homeService.GetProcessingCollectionPlan(condition) ;
            //var storages = await _homeService.GetStatisticalStorageByYear(condition);
            //var profiles = await _homeService.ProfileAndDocByStatus(condition);
            //var expiries = await _homeService.StatisticalExpiryDate(condition);
            ViewBag.CbbTypeDataDashBoard = await _homeService.GetDataTypeDashBoard(condition);
            ViewData["ProcessingCollectionPlan"] =  new List<VMDashBoardPlan>();
            ViewData["ProfileAndDocByStatus"] =  new VMDashBoardProfile(); ;
            ViewData["StatisticalStorageByYear"] =  new VMDashBoardStorage(); ;
            ViewData["StatisticalExpiryDate"] =  new VMDashBoardExpiryDate();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchByCondition(HomeCondition condition)
        {
            ViewBag.CbbTypeDataDashBoard = await _homeService.GetDataTypeDashBoard(condition);
            ViewData["ProcessingCollectionPlan"] = new List<VMDashBoardPlan>();
            ViewData["ProfileAndDocByStatus"] = new VMDashBoardProfile(); ;
            ViewData["StatisticalStorageByYear"] = new VMDashBoardStorage(); ;
            ViewData["StatisticalExpiryDate"] = new VMDashBoardExpiryDate();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBookMark(int id)
        {
            var rs = await _userBookMark.AddBookMark(id);
            return CustJSonResult(rs);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBookMark(int id)
        {
            var rs = await _userBookMark.RemoveBookMark(id);
            return CustJSonResult(rs);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookMark(List<int> ids)
        {
            var rs = await _userBookMark.ChangeBookMark(ids);
            return CustJSonResult(rs);
        }

        #region Notification
        [HttpPost]
        public async Task<IActionResult> PushNotification(int userId)
        {
            VMNotification noti = await _homeService.GetNotificationByUserId(userId);
            return PartialView("_HeaderNotify", noti);
        }

        [HttpPost]
        public async Task<IActionResult> ReadNotification(int id)
        {
            bool isSuccess = await _homeService.ReadNotification(id);
            ServiceResult rs = new ServiceResult();
            if (isSuccess)
                rs.Code = CommonConst.Success;
            else
                rs.Code = CommonConst.Error;
            return CustJSonResult(rs);
        }

        [HttpGet]
        public async Task<IActionResult> ListNotification(NotificationCondition condition)
        {
            condition.UserId = _userPrincipalService.UserId;
            PaginatedList<VMNotification> noti = await _homeService.GetListNotificationPaging(condition);
            return View("ListNotification", noti);
        }

        [HttpPost]
        public async Task<IActionResult> SearchNotificationPaging(NotificationCondition condition)
        {
            PaginatedList<VMNotification> noti = await _homeService.GetListNotificationPaging(condition);
            return PartialView("_ListNotification", noti);
        }
        #endregion Notification
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        //public IActionResult Error()
        //{
        //    return View();
        //}
    }
}
