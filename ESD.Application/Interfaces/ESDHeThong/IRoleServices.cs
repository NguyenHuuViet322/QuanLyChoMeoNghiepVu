using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IRoleServices : IBaseMasterService<Role>
    {
        Task<PaginatedList<VMRole>> SearchByConditionPagging(RoleCondition condition);
        //Task<IEnumerable<VMRole>> GetRolesWithCondition(RoleCondition condition);
        Task<VMCreateRole> GetRole(int id);
        Task<VMRole> GetRoleDetail(int id);
        Task<bool> IsNameExist(string name);
        Task<ServiceResult> CreateRole(VMCreateRole vmRole);
        Task<ServiceResult> UpdateRole(VMCreateRole vmRole);
        Task<ServiceResult> DeleteRole(int id);
        Task<ServiceResult> DeleteMultiRole(IEnumerable<int> ids);
        Task<IEnumerable<Role>> GetActive();
        //Task<IEnumerable<int>> UpdateRolesByTeams(IEnumerable<int> roleIds, IEnumerable<int> groupIds, string type);
        Task<IEnumerable<VMRole>> GetListByCondition(RoleCondition condition);
    }
}
