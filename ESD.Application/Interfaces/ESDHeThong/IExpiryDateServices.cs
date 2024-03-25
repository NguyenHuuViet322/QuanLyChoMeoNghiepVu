using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IExpiryDateServices : IBaseMasterService<ExpiryDate>
    {
        Task<PaginatedList<VMExpiryDate>> SearchByConditionPagging(ExpiryDateCondition condition);
        Task<VMExpiryDate> GetExpiryDate(int id);
        Task<bool> IsCodeExist(string code);
        Task<ServiceResult> CreateExpiryDate(VMExpiryDate vmRole);
        Task<ServiceResult> UpdateExpiryDate(VMExpiryDate vmRole);
        Task<ServiceResult> DeleteExpiryDate(int id);
        Task<ServiceResult> DeleteMultiExpiryDate(IEnumerable<int> ids);
        Task<IEnumerable<VMExpiryDate>> GetListByCondition(ExpiryDateCondition condition);
        Task<IEnumerable<VMExpiryDate>> GetsActive();
        Task<VMExpiryDate> GetNewExpiryDate();
    }
}
