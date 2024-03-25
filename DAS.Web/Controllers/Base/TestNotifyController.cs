using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESD.Infrastructure.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DAS.Web.Controllers
{
    public class TestNotifyController : Controller
    {
        private readonly IHubNotificationHelper _hubNotificationHelper;
        private readonly IConnectionManager _connectionManager;
        public TestNotifyController(IHubNotificationHelper hubNotificationHelper, IConnectionManager connectionManager)
        {
            _hubNotificationHelper = hubNotificationHelper;
            _connectionManager = connectionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task SendToSpecificUser(string user, string message)
        {
          await  _hubNotificationHelper.PushToUser(2);
        }
    }
} 
