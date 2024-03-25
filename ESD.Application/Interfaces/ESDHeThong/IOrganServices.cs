using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IOrganServices : IBaseMasterService<Organ>
    {
        //Task<IEnumerable<OrganHierarchy>> GetListOrganHierarchy();
        Task<List<SelectListItem>> GetOrgans(string ids);
        Task<PaginatedList<VMOrgan>> SearchByConditionPagging(OrganCondition condition);
        Task<VMOrgan> GetDetail(int id);
        Task<VMEditOrgan> GetOrgan(int id);
        Task<ServiceResult> CreateOrgan(VMCreateOrgan vmOrgan);
        Task<ServiceResult> UpdateOrgan(VMEditOrgan vmOrgan);
        Task<IEnumerable<Organ>> GetParentOrgan(int id);
        Task<ServiceResult> DeleteOrgan(int id);
        Task<ServiceResult> DeleteMultiOrgan(IEnumerable<int> id);
        Task<IEnumerable<Organ>> GetActive(bool isShowAll = false);
        Task<IEnumerable<VMOrgan>> GetListByCondition(OrganCondition condition);
    }
}
