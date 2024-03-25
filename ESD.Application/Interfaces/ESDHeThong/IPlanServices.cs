using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IPlanServices : IBaseMasterService<Plan>
    {
        Task<ServiceResult> CreatePlan(VMCreatePlan vmPlan, int status = 0);
        Task<VMIndexPlan> SearchByConditionPagging(PlanCondition condition, bool isExport);
        Task<VMIndexPlan> SearchApproveByConditionPagging(PlanCondition condition, bool isExport);
        Task<VMCreatePlan> Create();
        Task<VMCreatePlan> Edit(int id = 0);
        Task<ServiceResult> Deletes(IEnumerable<int> ids);
        Task<ServiceResult> UpdatePlan(VMCreatePlan vmPlan);
        Task<VMIndexPlanProfile> CatalogingIndex(PlanProfileCondition condition, bool isExport = false);
        Task<VMUpdatePlanProfile> CreatePlanProfile(int id);
        Task<ServiceResult> CreatePlanProfile(VMUpdatePlanProfile vmProfile);
        Task<VMUpdatePlanProfile> UpdatePlanProfile(int? id);
        Task<ServiceResult> UpdatePlanProfile(VMUpdatePlanProfile vmProfile);
        Task<ServiceResult> UpdatePlanProfileInCollect(VMUpdatePlanProfile vmProfile);
        Task<ServiceResult> DeletePlanProfile(int id);
        Task<ServiceResult> DeletePlanProfiles(IEnumerable<int> ids);
        Task<PaginatedList<VMPlan>> SearchByConditionPaggingByAgency(PlanCondition condition, bool isExport = false);
        Task<VMIndexPlan> SearchByConditionPlanCollection(PlanCondition condition, bool isExport = false);
        Task<ServiceResult> SendApproveProfile(int id);
        Task<IEnumerable<Plan>> GetComboboxPlan();
        Task<ServiceResult> RejectPlan(int id, string reason = "");
        Task<ServiceResult> ApprovePlan(int id);
        Task<ServiceResult> SendApprovePlan(int id);
        Task<ServiceResult> ClosePlan(int id);
        Task<ServiceResult> OpenPlan(int id);
        Task<VMIndexPlanProfile> GetDocumentCollectListCondition(PlanProfileCondition condition, bool isExport = false);
        Task<VMIndexPlan> ReceiveArchiveIndex(PlanCondition condition);
        Task<VMIndexPlanProfile> ReceiveArchiveProfileIndex(PlanProfileCondition condition, bool isExport = false);
        Task<ServiceResult> RejectArchiveProfile(int id, string reason = "");
        Task<ServiceResult> ApproveArchiveProfile(int id);
        Task<ServiceResult> RejectArchiveProfiles(int[] ids, string reason = "");
        Task<ServiceResult> ApproveArchiveProfiles(int[] ids);
        Task<VMPlanProfile> GetPlanProfile(int? id);
        Task<IEnumerable<VMPlan>> GetActive();
        Task<IEnumerable<VMPlan>> GetApprove();
        Task<IEnumerable<VMPlan>> GetPlanForDelivery();
        Task<IEnumerable<VMPlan>> GetPlanForEditDelivery();
        Task<IEnumerable<VMPlanProfile>> GetPlanProfileByListID(List<string> lstID, VMDeliveryRecord model = null);
    }
}
