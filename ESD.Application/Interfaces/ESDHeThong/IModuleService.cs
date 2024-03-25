using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using ESD.Utility.CustomClass;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IModuleService
    {
        Task<IEnumerable<Module>> Gets();
        Task<bool> ReUpdateModule(int IDparent = 1, int IDChannel = 0);
        Task<IEnumerable<Module>> GetModuleForCurrentUser();
        Task<IEnumerable<Module>> GetsActive();
        Task<PaginatedList<VMModule>> SearchByConditionPagging(ModuleCondition Condition);
        Task<ServiceResult> Create(VMModule vmModule);
        Task<ServiceResult> Update(VMModule vmModule);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Deletes(IEnumerable<int> ids);
        Task<List<SelectListItemTree>> GetModuleByTree(VMModule vMModule);
        Task<List<SelectListItem>> GetListIcon(VMModule vMModule);
        Task<Module> Get(object id);
        public List<SelectListItem> GetModuleCodeDrd(string moduleCode);
    }
}
