using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ITeamService : IBaseMasterService<Team>
    {
        Task<PaginatedList<VMTeam>> SearchByConditionPagging(TeamCondition condition);
        Task<IEnumerable<Team>> GetActive();
        Task<ServiceResult> CreateTeam(VMCreateTeam user);
        Task<ServiceResult> UpdateTeam(VMEditTeam user);
        Task<VMEditTeam> GetTeam(int id);
        Task<VMTeam> GetTeamDetail(int id);
        Task<ServiceResult> DeleteTeam(int id);
        Task<ServiceResult> DeleteMultiTeam(IEnumerable<int> ids);
        Task<IEnumerable<int>> UpdateTeamsByGroupPers(IEnumerable<int> groupPerIds, IEnumerable<int> groupIds, int idRemoved);
        Task<IEnumerable<VMTeam>> GetListByCondition(TeamCondition condition);
        Task<int[]> GetUserOfTeam(int idTeam);
    }
}
