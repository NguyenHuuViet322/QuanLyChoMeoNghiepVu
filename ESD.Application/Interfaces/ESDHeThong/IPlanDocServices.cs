using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ESD.Application.Interfaces
{
    public interface IPlanDocServices
    {
        Task<VMIndexDocPlan> PlanDocDetailIndex(PlanDocCondition condition, bool isExport = false);
        Task<VMIndexDocPlan> PlanDocDetailIndexNoPaging(PlanDocCondition condition);
        Task<VMIndexDocPlan> PlanDocDetailIndexListApprovedNoPaging(PlanDocCondition condition);
        Task<VMDocCreate> CreateDocCollect(int IDProfile, int IDDocType = 0);
        Task<ServiceResult> Create(Hashtable data, bool isComplete);
        Task<ServiceResult> Update(Hashtable data, bool isComplete);
        Task<VMDocCreate> GetDocCollect(int IDDoc);
        Task<ServiceResult> DeleteDoc(int id);
        Task<ServiceResult> DeleteDocs(IEnumerable<int> ids);
        Task<ServiceResult> SaveFile(VMDoc vMDoc, string urlViewFile);
        Task<ServiceResult> AutoOCR(long idFile);
        Task<ServiceResult> SaveScanFile(VMDoc vmDoc, string v);
        Task<ServiceResult> GetDocByIDFile(long id);
        Task<ServiceResult> UpdateDocFile(VMDoc model);
    }
}
