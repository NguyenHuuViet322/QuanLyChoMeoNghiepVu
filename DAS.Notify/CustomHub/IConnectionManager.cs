using System.Collections.Generic;

namespace DAS.Notify.CustomHub
{
    public interface IConnectionManager
    {
        void AddConnection(int userId, string connectionID);
        void AddConnectionPortal(int userId, string connectionID);
        void RemoveConnection(string connectionId);
        void RemoveConnectionPortal(string connectionId);
        HashSet<string> GetConnections(int userId);
        HashSet<string> GetConnectionsPortal(int userId);
        IEnumerable<int> OnlineUsers { get; }
        IEnumerable<int> OnlineUsersPortal { get; }
    }
}
