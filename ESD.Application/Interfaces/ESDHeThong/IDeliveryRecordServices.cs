using System.Collections.Generic;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;

namespace ESD.Application.Interfaces
{
    public interface IDeliveryRecordServices : IBaseMasterService<DeliveryRecord>
    {
        Task<ServiceResult> Create(VMDeliveryRecord vmRecord);
        Task<ServiceResult> Update(VMDeliveryRecord vmRecord);
        Task<ServiceResult> ChangeRecordStatus(int id, int status);
        Task<PaginatedList<VMDeliveryRecord>> SearchListConditionPagging(DeliveryRecordCondition condition);
        Task<VMDeliveryRecord> GetDeliveryRecord(int id);
        Task<ServiceResult> SendDeliveryRecord(int id, byte[] attachment = null, string attachmentName = "");
        Task<ServiceResult> RejectDeliveryRecord(int id, string note);
        Task<ServiceResult> ApproveDeliveryRecord(int id);
        Task<ServiceResult> SendListRecord(int[] ids, Dictionary<int, byte[]> dictAttachment = null, Dictionary<int, string> dictAttachmentName = null);
        Task<ServiceResult> DeleteListRecord(int[] ids);
        Task<PaginatedList<VMDeliveryRecord>> SearchListReceiveConditionPagging(DeliveryRecordCondition condition);
        Task<ServiceResult> GetAgencyByIdPlan(int id);
        Task<ServiceResult> GetPlanByIdAgency(int id);
        Task<ServiceResult> GetUserByIdAgency(int id);
        Task<ServiceResult> GetArchiveApprovedPlanProfileByModel(VMDeliveryRecord model);
        Task<IEnumerable<Agency>> GetAgencyReadyForDelivery();
        Task<IEnumerable<Agency>> GetAgencyReadyForEditDelivery();
        Task<VMDeliveryRecord> GetDeliveryRecordByIDPlanProfile(int id);
    }
}
