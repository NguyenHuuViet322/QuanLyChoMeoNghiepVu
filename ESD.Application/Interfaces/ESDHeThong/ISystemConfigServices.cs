using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ISystemConfigServices : IBaseMasterService<SystemConfig>
    {
        Task<PaginatedList<VMSystemConfig>> SearchByConditionPagging(SystemConfigCondition condition, bool isExport = false);
        Task<VMUpdateSystemConfig> GetSystemConfig(int id);
        Task<ServiceResult> CreateSystemConfig(VMUpdateSystemConfig model);
        Task<ServiceResult> UpdateSystemConfig(VMUpdateSystemConfig model);
        Task<ServiceResult> DeleteSystemConfig(int id);
        Task<ServiceResult> Deletes(IEnumerable<int> ids);
        Task<IEnumerable<SystemConfig>> GetsActive();
    }
}
