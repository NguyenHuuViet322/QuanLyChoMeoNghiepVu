using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using System.Collections.Generic;

namespace ESD.Application.Interfaces
{
    public interface IArchiveManagementService
    {
        Task<VMIndexPlanProfile> SearchByConditionPagging(ArchiveManagementCondition condition);
        Task<VMIndexPlanProfile> SearchListApprovedByConditionPagging(ArchiveManagementCondition condition);
        Task<VMIndexPlanProfile> SearchByCondition(ArchiveManagementCondition condition);
        Task<VMIndexPlanProfile> SearchListApprovedByCondition(ArchiveManagementCondition condition);
        Task<ServiceResult> ApprovePlanProfile(int id);
        Task<ServiceResult> RejectPlanProfile(int id, string note);
        Task<IEnumerable<Plan>> GetComboboxPlan();
        Task<IEnumerable<Plan>> GetComboboxPlanListApproved();

        Task<VMIndexDocPlan> PlanDocDetailIndex(PlanDocCondition condition, bool isExport = false);
        Task<VMIndexDocPlan> PlanDocDetailIndexListApproved(PlanDocCondition condition);
        Task<VMIndexDocPlan> PlanDocDetailIndexNoPaging(PlanDocCondition condition);
        Task<VMIndexDocPlan> PlanDocDetailIndexListApprovedNoPaging(PlanDocCondition condition);
        Task<VMDocCreate> GetDocCollect(int IDDoc);
    }
}
