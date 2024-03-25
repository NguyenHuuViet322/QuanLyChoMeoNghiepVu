using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IGroupPermissionService : IBaseMasterService<GroupPermission>
    {
        Task<ServiceResult> Create(VMGroupPermision vmGroupPermision);
        Task<IEnumerable<VMGroupPermision>> GetGroupPermissionsInList();
        Task<ServiceResult> Update(VMGroupPermision vmGroupPermision);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
        Task<PaginatedList<VMGroupPermision>> SearchByConditionPagging(PermissionGroupCondition permissionGroup);
        Task<IEnumerable<VMGroupPermision>> GetListByCondition(PermissionGroupCondition permissionGroup);
        Task<IEnumerable<int>> UpdateGroupPersByTeams(IEnumerable<int> groupPerIds, IEnumerable<int> groupIds, string type, int idRemoved);
    }
}