using DAS.Notify.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DAS.Notify.CustomHub.CommonHub
{
    public class CommonHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Func<string, IConnectionManager> _connectionManager;
        public CommonHub(Func<string, IConnectionManager> connectionManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionManager = connectionManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task OnConnectedAsync()
        {
            var userId = _httpContextAccessor.HttpContext.Request.Query["userid"];

            //Get UserId from client
            int intUserId;
            var bnlRs = int.TryParse(userId, out intUserId);
            if (bnlRs)
            {
                _connectionManager(MutipleImplement.ConnectionManagerType.DasWeb.ToString()).AddConnection(intUserId, Context.ConnectionId);
                _connectionManager(MutipleImplement.ConnectionManagerType.DasPortal.ToString()).AddConnectionPortal(intUserId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager(MutipleImplement.ConnectionManagerType.DasWeb.ToString()).RemoveConnection(Context.ConnectionId);
            _connectionManager(MutipleImplement.ConnectionManagerType.DasPortal.ToString()).RemoveConnectionPortal(Context.ConnectionId);
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
