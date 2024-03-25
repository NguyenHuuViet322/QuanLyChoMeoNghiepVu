using DAS.Notify.Constants;
using DAS.Notify.CustomHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DAS.Notify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CommonController : ControllerBase
    {
        private readonly Func<string, IHubCommonHelper> _hubCommonHelper;
        public CommonController(Func<string, IHubCommonHelper> hubCommonHelper)
        {
            _hubCommonHelper = hubCommonHelper;
        }

        [HttpPost]
        [Route("PushToUser")]
        public async Task<IActionResult> PushToUser(int userId)
        {
            await _hubCommonHelper(MutipleImplement.HubNotifyHelperType.DasWeb.ToString()).PushToUser(userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("PushToUsers")]
        public async Task<IActionResult> PushToUsers([FromBody] int[] userId)
        {
            await _hubCommonHelper(MutipleImplement.HubNotifyHelperType.DasWeb.ToString()).PushToUsers(userId);
            return new OkResult();
        }
        [HttpPost]
        [Route("PushToUserPortal")]
        public async Task<IActionResult> PushToUserPortal(int userId)
        {
            await _hubCommonHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToUserPortal(userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("PushToUsersPortal")]
        public async Task<IActionResult> PushToUsersPortal([FromBody] int[] userId)
        {
            await _hubCommonHelper(MutipleImplement.HubNotifyHelperType.DasPortal.ToString()).PushToUsersPortal(userId);
            return new OkResult();
        }
        [HttpPost("PushToAll")]
        public async Task<IActionResult> PushToAll()
        {
            await _hubCommonHelper(MutipleImplement.HubNotifyHelperType.DasWeb.ToString()).PushToAll();
            return new OkResult();
        }


    }
}
