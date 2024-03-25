using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IAgencyServices : IBaseMasterService<Agency>
    {
        Task<IEnumerable<Agency>> GetAgencys(IEnumerable<int> organId, IEnumerable<int> ids);
        Task<PaginatedList<VMAgency>> SearchByConditionPagging(AgencyCondition condition);
        Task<VMAgency> GetDetail(int id);
        Task<VMEditAgency> GetAgency(int id);
        Task<ServiceResult> CreateAgency(VMCreateAgency vmAgency);
        Task<ServiceResult> UpdateAgency(VMEditAgency vmAgency);
        Task<ServiceResult> DeleteAgency(int id);
        Task<ServiceResult> DeleteMultiAgency(IEnumerable<int> id);
        Task<IEnumerable<Agency>> GetActive();
        Task<IEnumerable<Agency>> GetAgencyByUser();
        Task <IEnumerable<VMAgency>> GetListByCondition(AgencyCondition condition);
        Task<IEnumerable<Agency>> GetParentAgency(int id);
        Task<IEnumerable<HierachyAgency>> GetHierachyAgency(int id);
    }
}
