using ESD.Infrastructure.ContextAccessors;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Notifications
{
    public class NotificationHub: Hub
    {
        private IConnectionManager _connectionManager;
        private readonly IUserPrincipalService _userPrincipalService;
        public NotificationHub(IConnectionManager connectionManager, IUserPrincipalService userPrincipalService)
        {
            _connectionManager = connectionManager;
            _userPrincipalService = userPrincipalService;
        }

     
        public override Task OnConnectedAsync()
        {
            _connectionManager.AddConnection(_userPrincipalService.UserId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Example, nếu cần thì viết hàm  để gọi
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAll(string message)
        {
           await Clients.All.SendAsync("ReceiveMessage", message);
        }

    }
}
