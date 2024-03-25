using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ICategoryServices
    {
        Task<VMIndexCategory> SearchByConditionPagging(CategoryCondition condition, Hashtable searchData);
        Task<VMCreateCategory> Create(string codeType);
        Task<VMCreateCategory> Create(string codeType, Dictionary<string, int> dictCategoryTypeValues);
        Task<ServiceResult> Create(Hashtable data);
        Task<IEnumerable<Category>> GetsActive(string codeType);
        Task<VMCreateCategory> Update(int? id);
        Task<ServiceResult> Update(Hashtable data);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
        Task<ServiceResult> Delete(int id);
        Task<IEnumerable<Category>> GetsActive(int idCategory);
        Task<string> GetCategoryOptions(Hashtable DATA);
        Task<VMIndexCategory> GetListByCondition(CategoryCondition condition, Hashtable searchData);
        Task<IEnumerable<Category>> GetByParent(string codeType, int inputType, int parentId, int idCategoryTypeRelated);
        Task<VMCategoryType> GetCategoryType(string codeType);
    }
}