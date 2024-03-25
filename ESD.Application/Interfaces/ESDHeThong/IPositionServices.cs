using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using ESD.Utility.CustomClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IPositionServices
    {
        Task<IEnumerable<Position>> GetsActive();
        Task<PaginatedList<VMPosition>> SearchByConditionPagging(PositionCondition positionCondition);
        Task<ServiceResult> Create(VMPosition vmPosition);
        Task<ServiceResult> Update(VMPosition vmPosition);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
        Task<List<SelectListItemTree>> GetPostionByTree(VMPosition vMPosition);
        Task<Position> Get(object id);
        Task<IEnumerable<Position>> Gets();
        Task<IEnumerable<VMPosition>> GetListByCondition(PositionCondition positionCondition, bool getParents = false);
    }
}
