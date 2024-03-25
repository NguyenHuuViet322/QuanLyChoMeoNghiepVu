using ESD.Infrastructure.ContextAccessors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Infrastructure.Notifications
{
   
    public class ConnectionManager : IConnectionManager
    {
        private static Dictionary<int, HashSet<string>> userMap = new Dictionary<int, HashSet<string>>();
        public IEnumerable<int> OnlineUsers
        {
            get
            {
                return userMap.Keys;
            }
        }

        public void AddConnection(int userId, string connectionID)
        {
            lock (userMap)
            {
                if(!userMap.ContainsKey(userId))
                {
                    userMap[userId] = new HashSet<string>();
                }
                userMap[userId].Add(connectionID);
            }
        }

        public HashSet<string> GetConnections(int userId)
        {
            var conn = new HashSet<string>();
            try
            {
                lock (userMap)
                {
                    conn = userMap[userId];
                }
            }
            catch (Exception)
            {
                conn = null;
            }
            return conn;
        }

        public void RemoveConnection(string connectionId)
        {
            lock (userMap)
            {
                foreach (var userId in userMap.Keys)
                {
                    if (userMap.ContainsKey(userId) && userMap[userId].Contains(connectionId) )
                    {
                        userMap[userId].Remove(connectionId);
                        break;
                    }
                }
            }
        }

    }
}
