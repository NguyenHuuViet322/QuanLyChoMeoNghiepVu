using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;

namespace ESD.Application.Interfaces
{
    public interface IProfileListService : IBaseMasterService<ProfileList>
    {
        Task<PaginatedList<VMProfileList>> SearchByConditionPagging(ProfileListCondition condition);
        Task<ServiceResult> CreateProfileList(VMProfileList vmProfileList);
        Task<ServiceResult> DeleteProfileList(int id);
        Task<ServiceResult> DeleteMultiProfileList(IEnumerable<int> ids);
        Task<VMProfileList> GetActive(int id);
        Task<ServiceResult> UpdateProfileList(VMProfileList vMProfileList);
        Task<IEnumerable<VMProfileList>> GetListByCondition(ProfileListCondition condition);
        Task<IEnumerable<VMProfileList>> GetsActive();
    }
}
