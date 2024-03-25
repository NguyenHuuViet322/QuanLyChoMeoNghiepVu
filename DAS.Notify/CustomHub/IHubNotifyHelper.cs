using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAS.Notify.CustomHub
{
    public interface IHubNotifyHelper
    {
        IEnumerable<int> GetOnlineUser();
        Task PushToUser(int userId);
        Task PushToUserPortal(int userId);
        Task PushToUsers(int[] userIds);
        Task PushToUsersPortal(int[] userIds);
        Task PushToAll();
    }
}
