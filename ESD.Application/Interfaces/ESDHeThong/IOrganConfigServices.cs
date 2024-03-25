using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IOrganConfigServices : IBaseMasterService<OrganConfig>
    {
        Task<PaginatedList<VMOrganConfig>> SearchByConditionPagging(OrganConfigCondition condition, bool isExport = false);
        Task<VMUpdateOrganConfig> GetOrganConfig(int id);
        Task<ServiceResult> CreateOrganConfig(VMUpdateOrganConfig model);
        Task<ServiceResult> UpdateOrganConfig(VMUpdateOrganConfig model);
        Task<ServiceResult> DeleteOrganConfig(int id);
        Task<ServiceResult> Deletes(IEnumerable<int> ids);
    }
}
