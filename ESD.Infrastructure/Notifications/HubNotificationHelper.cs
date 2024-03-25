using ESD.Infrastructure.ContextAccessors;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Notifications
{
    public class HubNotificationHelper : IHubNotificationHelper
    {
        IHubContext<NotificationHub> _hubContext { get; }
        private readonly IConnectionManager _connectionManager;
        public HubNotificationHelper(IConnectionManager connectionManager, IHubContext<NotificationHub> hubContext)
        {
            _connectionManager = connectionManager;
            _hubContext = hubContext;
        }

        public IEnumerable<int> GetOnlineUser()
        {
            return _connectionManager.OnlineUsers;
        }

        /// <summary>
        /// Gọi tới action load notification của user
        /// </summary>
        /// <param name="userId">id người dùng cần load lại notification</param>
        /// <returns></returns>
        public async Task PushToUser(int userId)
        {
            HashSet<string> connections = _connectionManager.GetConnections(userId);

            if (connections != null && connections.Count != 0)
            {
                foreach (var connection in connections)
                {
                    await _hubContext.Clients.Client(connection).SendAsync("LoadNotifyByUserId", userId);
                }
            }
        }

        /// <summary>
        /// Gọi tới action load notification của nhiều user
        /// </summary>
        /// <param name="userId">id những người dùng cần load lại notification</param>
        /// <returns></returns>
        public async Task PushToUsers(int[] userIds)
        {
            for (int i = 0; i < userIds.Length; i++)
            {
                await PushToUser(userIds[i]);
            }
        }

        /// <summary>
        /// Load lại all notification của tất cả người dùng đang online
        /// </summary>
        public async Task PushToAll()
        {
          await  _hubContext.Clients.All.SendAsync("LoadNotifyByUserId");
        }


    }
}
