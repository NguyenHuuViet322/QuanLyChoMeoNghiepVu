using DASNotify.Application.Interfaces;
using DAS.Notify.Constants;
using DAS.Notify.CustomHub;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DAS.Notify.Controllers
{
    [ApiController]
    [Route("apiPortal")]
    public class PortalNotifyController : ControllerBase
    {
        // private readonly IHubNotificationHelper _hubNotificationHelper;
        private readonly Func<string, IHubNotifyHelper> _hubNotificationHelper;
        private readonly ISendNotificationServices _sendNotification;
        public PortalNotifyController(Func<string, IHubNotifyHelper> hubNotificationHelper, ISendNotificationServices sendNotification)
        {
            _hubNotificationHelper = hubNotificationHelper;
            _sendNotification = sendNotification;
        }

        [HttpPost]
        [Route("PushToUser")]
        public async Task<IActionResult> PushToUser([FromForm] int userId)
        {
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToUserPortal(userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("PushToUsers")]
        public async Task<IActionResult> PushToUsers(int[] userId, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, int UserImpactType = 0)
        {
            await _sendNotification.PushToUsersPortal(userId, content, url, IDImpactUser, IDAffectedObject, AffectedObjectType, IDImpactAgency, IDImpactOrgan);
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToUsersPortal(userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("PushToUsersPortal")]
        public async Task<IActionResult> PushToUsersPortal(int[] userId, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0)
        {
            await _sendNotification.PushToUsersPortal(userId, content, url, IDImpactUser, IDAffectedObject, AffectedObjectType, IDImpactAgency, IDImpactOrgan);
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToUsersPortal(userId);
            return new OkResult();
        }
        [HttpPost("PushToAll")]
        public async Task<IActionResult> PushToAll()
        {
            await _hubNotificationHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToAll();
            return new OkResult();
        }

    }
}
