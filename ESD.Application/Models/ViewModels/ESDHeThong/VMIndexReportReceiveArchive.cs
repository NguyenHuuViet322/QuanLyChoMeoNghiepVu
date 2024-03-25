using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexReportReceiveArchive
    {
        public ReportReceiveArchiveCondition ReportReceiveArchiveCondition { get; set; }
        public PaginatedList<VMReportReceiveArchive> VMReportReceiveArchives { get; set; }
        public Dictionary<int,string> DictAgency { get; set; }
    }

    public class ReportReceiveArchiveCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string Agencies { get; set; }
        public List<string> ListAgencyStr
        {
            get
            {
                if (Agencies.IsNotEmpty())
                    return Agencies.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public ReportReceiveArchiveCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
