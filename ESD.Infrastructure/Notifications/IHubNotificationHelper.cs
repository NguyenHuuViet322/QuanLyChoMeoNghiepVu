using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Notifications
{
    public interface IHubNotificationHelper
    {

        IEnumerable<int> GetOnlineUser();
        Task PushToUser(int userId);
        Task PushToUsers(int[] userIds);
        Task PushToAll();
    }
}
