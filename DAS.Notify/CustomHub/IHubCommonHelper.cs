using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAS.Notify.CustomHub
{
    public interface IHubCommonHelper
    {
        IEnumerable<int> GetOnlineUser();
        IEnumerable<int> GetOnlineUserPortal();
        Task PushToUser(int userId);
        Task PushToUsers(int[] userIds);
        Task PushToUserPortal(int userId);
        Task PushToUsersPortal(int[] userIds);
        Task PushToAll();
    }
}
