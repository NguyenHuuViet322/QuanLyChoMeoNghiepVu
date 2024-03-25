using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Infrastructure.Notifications
{
    public interface IConnectionManager
    {
        void AddConnection(int userId, string connectionID);
        void RemoveConnection(string connectionId);
        HashSet<string> GetConnections(int userId);
        IEnumerable<int> OnlineUsers { get; }
    }
}
