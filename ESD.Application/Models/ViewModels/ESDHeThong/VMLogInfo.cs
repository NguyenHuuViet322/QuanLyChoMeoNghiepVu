using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMLogInfoStatistic
    {
        public int? Index { get; set; }
        public string PreFix { get; set; }
        public object ChartData { get; set; }
        public PaginatedList<VMLogInfo> Tables { get; internal set; }
    }
    public class VMLogInfo
    {
        public string ID { get; set; }
      
        public string TagName { get; set; } // ID của bản ghi
        public string Entity { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long UserId { get; set; }
        public string Username { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedValue { get; set; }
        public int Total { get; set; }
        public object ChartData { get; set; }
        public List<VMLogInfo> vMLogInfos { get; set; }
    }

    public class VMUserLogInfo
    {
        public string ID { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long UserId { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }

    }

    public class LogInfoCondition
    {
        public LogInfoCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string FromDate { get; set; } //dd/MM/yyyy
        public string ToDate { get; set; }
        public string ActionCRUD { get; set; }
        public int Type { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
