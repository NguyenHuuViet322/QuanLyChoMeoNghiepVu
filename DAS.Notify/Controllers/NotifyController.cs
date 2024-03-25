using DAS.Notify.Constants;
using DAS.Notify.CustomHub;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DASNotify.Application.Interfaces;
using DASNotify.Application.Models.ViewModels;

namespace DAS.Notify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NotifyController : ControllerBase
    {
        // private readonly IHubNotificationHelper _hubNotificationHelper;
        private readonly Func<string, IHubNotifyHelper> _hubNotificationHelper;
        private readonly INotificationService _notification;
        public NotifyController(Func<string, IHubNotifyHelper> hubNotificationHelper, INotificationService notification)
        {
            _hubNotificationHelper = hubNotificationHelper;
            _notification = notification;
        }

        [HttpPost]
        [Route("PushToUser")]
        public async Task<IActionResult> PushToUser([FromForm] int userId)
        {
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasWeb.ToString()).PushToUser(userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("PushToUsers")]
        public async Task<IActionResult> PushToUsers(VMSendNotification model)
        {
            await _notification.PushToUsers(model);
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasWeb.ToString()).PushToUsers(model.idsUser);
            return new OkResult();
        }
        [HttpPost]
        [Route("PushToUsersPortal")]
        public async Task<IActionResult> PushToUsersPortal(VMSendNotificationPortal model)
        {
            await _notification.PushToUsersPortal(model);
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToUsersPortal(model.idsUser);
            return new OkResult();
        }
        [HttpPost("PushToAll")]
        public async Task<IActionResult> PushToAll()
        {
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasWeb.ToString()).PushToAll();
            return new OkResult();
        }


    }
}
