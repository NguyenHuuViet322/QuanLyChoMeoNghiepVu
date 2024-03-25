using System;
using System.Collections.Generic;

namespace DAS.Notify.CustomHub
{
    public class ConnectionManager : IConnectionManager
    {
        private static Dictionary<int, HashSet<string>> userMap = new Dictionary<int, HashSet<string>>();
        private static Dictionary<int, HashSet<string>> userMapPortal = new Dictionary<int, HashSet<string>>();
        public IEnumerable<int> OnlineUsers
        {
            get
            {
                return userMap.Keys;
            }
        }
        public IEnumerable<int> OnlineUsersPortal
        {
            get
            {
                return userMapPortal.Keys;
            }
        }
        public void AddConnection(int userId, string connectionID)
        {
            lock (userMap)
            {
                if (!userMap.ContainsKey(userId))
                {
                    userMap[userId] = new HashSet<string>();
                }
                userMap[userId].Add(connectionID);
            }
        }
        public void AddConnectionPortal(int userId, string connectionID)
        {
            lock (userMapPortal)
            {
                if (!userMapPortal.ContainsKey(userId))
                {
                    userMapPortal[userId] = new HashSet<string>();
                }
                userMapPortal[userId].Add(connectionID);
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
        public HashSet<string> GetConnectionsPortal(int userId)
        {
            var conn = new HashSet<string>();
            try
            {
                lock (userMapPortal)
                {
                    conn = userMapPortal[userId];
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
                    if (userMap.ContainsKey(userId) && userMap[userId].Contains(connectionId))
                    {
                        userMap[userId].Remove(connectionId);
                        break;
                    }
                }
            }
        }
        public void RemoveConnectionPortal(string connectionId)
        {
            lock (userMapPortal)
            {
                foreach (var userId in userMapPortal.Keys)
                {
                    if (userMapPortal.ContainsKey(userId) && userMapPortal[userId].Contains(connectionId))
                    {
                        userMapPortal[userId].Remove(connectionId);
                        break;
                    }
                }
            }
        }
    }
}
