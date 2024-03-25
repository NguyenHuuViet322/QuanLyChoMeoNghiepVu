using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ESD.Application.Interfaces
{
    public interface ILanguageServices : IBaseMasterService<Language>
    {
        Task<PaginatedList<VMLanguage>> SearchByConditionPagging(LanguageCondition condition);
        //Task<IEnumerable<VMLanguage>> GetLanguagesWithCondition(LanguageCondition condition);
        Task<VMLanguage> GetLanguage(int id);
        Task<VMLanguage> GetLanguageDetail(int id);
        Task<bool> IsNameExist(string name);
        Task<bool> IsCodeExist(string name);
        Task<ServiceResult> CreateLanguage(VMLanguage vmLanguage);
        Task<ServiceResult> UpdateLanguage(VMLanguage vmLanguage);
        Task<ServiceResult> DeleteLanguage(int id);
        Task<ServiceResult> DeleteMultiLanguage(IEnumerable<int> ids);
        Task<IEnumerable<VMLanguage>> GetListByCondition(LanguageCondition condition);
        Task<IEnumerable<Language>> GetsActive();
    }
}

