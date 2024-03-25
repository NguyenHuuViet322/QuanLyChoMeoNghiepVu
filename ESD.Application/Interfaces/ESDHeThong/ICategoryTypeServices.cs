using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ICategoryTypeServices
    {
        Task<IEnumerable<CategoryType>> GetsActive();
        Task<IEnumerable<DataType>> GetDataTypes();
        Task<PaginatedList<VMCategoryType>> SearchByConditionPagging(CategoryTypeCondition CategoryTypeCondition);
        Task<VMCategoryType> Create();
        Task<ServiceResult> Create(VMUpdateCategoryType vmCategoryType);
        Task<VMCategoryType> Update(int? id);
        Task<ServiceResult> Update(VMUpdateCategoryType vmCategoryType);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
        Task<CategoryType> Get(object id);
        Task<IEnumerable<CategoryType>> Gets();
    }
}