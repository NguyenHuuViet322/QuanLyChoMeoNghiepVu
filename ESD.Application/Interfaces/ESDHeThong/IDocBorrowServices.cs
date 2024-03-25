using System.Collections;
using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ESD.Application.Enums;

namespace ESD.Application.Interfaces
{
    public interface IDocBorrowServices
    {
        Task<VMIndexDocBorrow> SearchByCondition(DocBorrowCondition condition, EnumBorrow.BorrowType borrowType);
        Task<VMIndexDocBorrow> BorrowSearchByCondition(DocBorrowCondition condition, EnumBorrow.BorrowType borrowType);
        Task<VMIndexDocBorrow> DetailProfile(int idProfile, EnumBorrow.BorrowType borrowType);
        Task<VMCatalogingDocCreate> DetailDoc(int idDoc, EnumBorrow.BorrowType borrowType);
        //Task<ServiceResult> BorrowProfile(int id);
        Task<ServiceResult> BorrowProfile(HttpRequest request, HttpResponse response, int id, EnumBorrow.BorrowType borrowType);

        //Task<ServiceResult> BorrowDoc(int id);
        Task<ServiceResult> BorrowDoc(HttpRequest request, HttpResponse response, int id, EnumBorrow.BorrowType borrowType);
        Task<string> ViewFile(int id, EnumBorrow.BorrowType borrowType);
        Task<ServiceResult> Approve(VMUpdateCatalogingBorrow borrow);
        Task<ServiceResult> Approves(VMUpdateCatalogingBorrow borrow);
        Task<ServiceResult> Rejects(int[] ids, string note);
        Task<ServiceResult> Returns(int[] ids);
        Task<VMIndexDocBorrow> ManagerBorrowSearchByCondition(DocBorrowCondition condition);
        Task<VMCatalogingDocCreate> GetDocCollect(int IDBorrow, int IDDoc);
        Task<ServiceResult> CancelBorrow(int id);
        Task<ServiceResult> ReBorrowDoc(int id);
        List<int> GetBorrowCart(HttpRequest request, EnumBorrow.BorrowType borrowType);
        Task<VMIndexBorrowCart> BorrowCartSearchByCondition(HttpRequest request, HttpResponse response, BorrowCartCondition condition, EnumBorrow.BorrowType _borrowType);
        Task<ServiceResult> RequestBorrow(HttpRequest request, HttpResponse response, VMUpdateCatalogingBorrow catalogingBorrow, EnumBorrow.BorrowType borrowType);
        ServiceResult RemoveCartItem(HttpRequest request, HttpResponse response, int id, EnumBorrow.BorrowType borrowType);
        Task<VMCatalogingBorrow> DetailBorrow(int id, int idReader);
        Task<VMCatalogingBorrow> GetBorrows(int[] ids);
        Task<VMIndexAdvanceSearch> AdvanceSearch(AdvanceSearchCondition condition);
        Task<List<VMDocTypeField>> GetSearchFieldsByType(int type);
        Task<List<VMDocTypeField>> GetSearchFieldsByIDDocType(int idDocType);
    }
}
