using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IProfileService 
    {
        Task<IEnumerable<VMProfile>> GetProfilesInList();
        Task<ServiceResult> Update(VMUpdateProfile vmProfile);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids); 
        Task<IEnumerable<VMProfile>> GetListByCondition(ProfileCondition permissionGroup);
        Task<Profile> Get(object id);
        Task<IEnumerable<Profile>> GetActive();
        Task<VMUpdateProfile> Update(int? id);
        Task<VMUpdateProfile> Create();
        Task<ServiceResult> Create(VMUpdateProfile  vmProfile);
        Task<VMIndexProfile> SearchByConditionPagging(ProfileCondition condition);

    }
}