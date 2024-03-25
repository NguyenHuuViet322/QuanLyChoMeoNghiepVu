using System.Collections.Generic;
using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using Microsoft.AspNetCore.Http;
namespace ESD.Application.Interfaces
{
    public interface IProfileDestructionServices
    {
        Task<VMIndexProfileWaitDestruction> SearchByConditionPagging(ProfileWaitDestructionCondition condition);
        Task<VMIndexDestructionProfile> SearchByConditionPagging(DestructionProfileCondition condition, List<int> lstStatus,bool byApprove = false);
        Task<VMIndexProfileWaitDestruction> SearchUsingByConditionPagging(ProfileWaitDestructionCondition condition);
        Task<VMIndexCatalogingDoc> CatalogingDocIndex(CatalogingDocCondition condition, bool isExport = false);
        Task<ServiceResult> SendApproved(int id);
        Task<ServiceResult> DeleteDestructionProfile(int id);
        Task<VMCatalogingDocCreate> GetDocCollect(int IDDoc);
        Task<ServiceResult> ChangeExpiryDate(int idProfile, int idExpiryDate);

        Task<ServiceResult> AddProfileWaitDestructions(IEnumerable<int> ids);
        Task<ServiceResult> RemoveProfileWaitDestructions(int id);
        //cookie
        List<int> GetDestructionTicket(HttpRequest request);
        ServiceResult RemoveTicketItem(HttpRequest request, HttpResponse response, int IDprofile);
        Task<ServiceResult> AddProfileToTicket(HttpRequest request, HttpResponse response, int IDprofile);
        Task<ServiceResult> CreateNewDestructionProfile(VMCreateDestructionProfile model,bool isSend =false);
        Task<VMCreateDestructionProfile> CreateNewDestructionProfile(List<int> ids);
        ServiceResult RemoveAllTicket(HttpRequest request, HttpResponse response);
        Task<ServiceResult> ApproveDestruction(int id);
        Task<ServiceResult> RejectDestruction(int id, string reason = "");
        Task<ServiceResult> RestoreDestruction(int id);
        Task<Dictionary<int, string>> GetDictExpiryDate();
    }
}
