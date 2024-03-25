using System;

namespace DASNotify.Domain.Models.CustomModels
{
    public class LogInfo
    {
        public string TagName { get; set; } // ID của bản ghi
        public string Entity { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long UserId { get; set; }
        public int IDOrgan { get; set; }
        public string Username { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public object ChangedValue { get; set; }

        public LogInfo() { }
        public LogInfo(string tagName, string entity, string action, object oldValue, object newValue)
        {
            TagName = tagName;
            Entity = entity;
            Action = action;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class LogUserInfo
    {
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; }
        public int IDOrgan { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }

        public LogUserInfo() { }
        public LogUserInfo(string action)
        {
            Action = action;
        }
        public LogUserInfo(long userID, string userName, string action, string ipAddress = null)
        {
            Action = action;
            UserId = userID;
            Username = userName;
            IPAddress = ipAddress;
        }
    }

    public class PdInfo
    {
        public string Pname { get; set; }
        public string PoValue { get; set; }
        public string PnValue { get; set; }

        public PdInfo(string pName, string poValue, string pnValue)
        {
            Pname = pName;
            PoValue = poValue;
            PnValue = pnValue;
        }
    }

}
