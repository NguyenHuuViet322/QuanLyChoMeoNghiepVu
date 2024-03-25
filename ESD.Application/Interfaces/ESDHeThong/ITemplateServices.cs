using System.Collections.Generic;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
namespace ESD.Application.Interfaces
{
    public interface ITemplateServices : IBaseMasterService<Template>
    {
        Task<ServiceResult> Create(VMTemplate model);
        Task<ServiceResult> Update(VMTemplate model);
        Task<VMTemplate> GetTemplate(int id);
        Task<ServiceResult> DeleteListTemplate(int[] ids);
        Task<PaginatedList<VMTemplate>> SearchListTemplateConditionPagging(TemplateCondition condition);
        Task<IEnumerable<Template>> GetActiveTemplate();
        Task<IEnumerable<VMTemplateParam>> GetTemplateParam(int id);
    }
}
