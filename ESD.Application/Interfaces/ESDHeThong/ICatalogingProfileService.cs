using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Http;

namespace ESD.Application.Interfaces
{
    public interface ICatalogingProfileService
    {
        Task<VMIndexPlanProfile> SearchByConditionPagging(ArchiveManagementCondition condition);
        Task<VMIndexPlanProfile> SearchListApprovedByConditionPagging(ArchiveManagementCondition condition);
        Task<ServiceResult> ApproveCatalogingProfile(int id);
        Task<ServiceResult> RejectCatalogingProfile(int id, string note);
        Task<VMIndexCatalogingProfile> DocumentCatalogingSearch(CatalogingProfileCondition condition);
        Task<PaginatedList<VMApproveStorage>> SearchListWaitApproveByConditionPagging(ApproveStorageCondition condition);
        Task<ServiceResult> ApproveCatalogingDocument(int id);
        Task<ServiceResult> RejectCatalogingDocument(int id, string reasonToReject);
        Task<ServiceResult> SendApprove(int[] ids, int approveBy);
        Task<VMUpdateCatalogingProfile> CreateCatalogingProfile();
        Task<ServiceResult> CreateCatalogingProfile(VMUpdateCatalogingProfile vmProfile);
        Task<VMUpdateCatalogingProfile> UpdateCatalogingProfile(int? id);
        Task<ServiceResult> UpdateCatalogingProfile(VMUpdateCatalogingProfile vmProfile);
        Task<ServiceResult> DeleteCatalogingProfiles(IEnumerable<int> ids);
        Task<ServiceResult> DeleteCatalogingProfile(int id);
        Task<ServiceResult> ViewDetailCatalogingDocument(int id);
        Task<ServiceResult> ApproveListCatalogingDocument(int[] ids);
        Task<VMIndexCatalogingDoc> CatalogingDocIndex(CatalogingDocCondition condition, bool isExport = false);
        Task<ServiceResult> UpdateCatalogingDoc(Hashtable data, bool isComplete);
        Task<ServiceResult> CreateCatalogingDoc(Hashtable data, bool isComplete);
        Task<VMCatalogingDocCreate> CreateCatalogingDoc(int IDProfile, int IDDocType = 1);
        Task<VMCatalogingDocCreate> GetDocCollect(int IDDoc);
        Task<ServiceResult> DeleteDocs(IEnumerable<int> ids);
        Task<ServiceResult> DeleteDoc(int id);
        Task<ServiceResult> SaveFile(VMCatalogingDoc vMCatalogingDoc, string urlViewFile);
        Task<ServiceResult> AutoOCR(long idFile);
        Task<VMIndexCatalogingProfile> ProfileStoringIndex(CatalogingProfileCondition condition);
        Task<ServiceResult> SaveProfileStoring(Hashtable data);
        Task<ServiceResult> SaveScanFile(VMCatalogingDoc vMCatalogingDoc, string urlViewFile);

    }
}
