using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ESD.Application.Interfaces
{
    public interface IReportArchiveServices
    {
        Task<VMIndexReportSendArchive> ReportSendArchivePaging(ReportSendArchiveCondition condition, bool isExport = false);
        Task<VMIndexReportReceiveArchive> ReportReceiveArchivePaging(ReportReceiveArchiveCondition condition, bool isExport = false);
    }
}
