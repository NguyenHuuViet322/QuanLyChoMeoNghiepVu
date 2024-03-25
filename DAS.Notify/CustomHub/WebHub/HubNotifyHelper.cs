using DAS.Notify.Constants;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAS.Notify.CustomHub.WebHub
{
    public class HubNotifyHelper : IHubNotifyHelper
    {
        IHubContext<NotifyHub> _hubContext { get; }
        // private readonly IConnectionManager _connectionManager;
        private readonly Func<string, IConnectionManager> _connectionManager;
        public HubNotifyHelper(Func<string, IConnectionManager> connectionManager, IHubContext<NotifyHub> hubContext)
        {
            _connectionManager = connectionManager;
            _hubContext = hubContext;
        }

        public IEnumerable<int> GetOnlineUser()
        {
            return _connectionManager(MutipleImplement.ConnectionManagerType.DasWeb.ToString()).OnlineUsers;
        }
        public IEnumerable<int> GetOnlineUserPortal()
        {
            return _connectionManager(MutipleImplement.ConnectionManagerType.DasPortal.ToString()).OnlineUsersPortal;
        }
        /// <summary>
        /// Gọi tới action load notification của user
        /// </summary>
        /// <param name="userId">id người dùng cần load lại notification</param>
        /// <returns></returns>
        public async Task PushToUser(int userId)
        {
            HashSet<string> connections = _connectionManager(MutipleImplement.ConnectionManagerType.DasWeb.ToString()).GetConnections(userId);

            if (connections != null && connections.Count != 0)
            {
                foreach (var connection in connections)
                {
                    await _hubContext.Clients.Client(connection).SendAsync("LoadNotifyByUserId", userId);
                }
            }
        }
        public async Task PushToUserPortal(int userId)
        {
            HashSet<string> connections = _connectionManager(MutipleImplement.ConnectionManagerType.DasPortal.ToString()).GetConnectionsPortal(userId);

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
        /// <param name="userIds">id những người dùng cần load lại notification</param>
        /// <returns></returns>
        public async Task PushToUsers(int[] userIds)
        {
            for (int i = 0; i < userIds.Length; i++)
            {
                await PushToUser(userIds[i]);
            }
        }
        public async Task PushToUsersPortal(int[] userIds)
        {
            for (int i = 0; i < userIds.Length; i++)
            {
                await PushToUserPortal(userIds[i]);
            }
        }
        /// <summary>
        /// Load lại all notification của tất cả người dùng đang online
        /// </summary>
        public async Task PushToAll()
        {
            await _hubContext.Clients.All.SendAsync("LoadNotifyByUserId");
        }

    }
}
