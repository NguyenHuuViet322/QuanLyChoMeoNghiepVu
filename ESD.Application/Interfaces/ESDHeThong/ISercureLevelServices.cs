using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ISercureLevelServices : IBaseMasterService<SercureLevel>
    {
        Task<PaginatedList<VMSercureLevel>> SearchByConditionPagging(SercureLevelCondition condition);        
        Task<VMSercureLevel> GetSercureLevel(int id);
        Task<bool> IsNameExist(string name);
        Task<bool> IsCodeExist(string code);
        Task<ServiceResult> CreateSercureLevel(VMSercureLevel vmSercureLevel);
        Task<ServiceResult> UpdateSercureLevel(VMSercureLevel vmSercureLevel);
        Task<ServiceResult> DeleteSercureLevel(int id);
        Task<ServiceResult> DeleteMultiSercureLevel(IEnumerable<int> ids);
        Task<IEnumerable<VMSercureLevel>> GetListByCondition(SercureLevelCondition condition);
        Task<IEnumerable<VMSercureLevel>> GetsActive();
    }
}
