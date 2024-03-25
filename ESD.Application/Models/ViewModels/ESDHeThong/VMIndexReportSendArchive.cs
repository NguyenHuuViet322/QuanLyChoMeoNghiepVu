using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexReportSendArchive
    {
        public ReportSendArchiveCondition ReportSendArchiveCondition { get; set; }
        public PaginatedList<VMReportSendArchive> VMReportSendArchives { get; set; }
        public List<VMDocType> VMDocTypes { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public List<VMDocField> VMDocFields { get; set; }
    }

    public class ReportSendArchiveCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public ReportSendArchiveCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
