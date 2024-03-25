using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IProfileTemplateServices : IBaseMasterService<ProfileTemplate>
    {
        Task<PaginatedList<ProfileTemplateExportExcel>> SearchByConditionPagging(ProfileTemplateCondition condition);
        Task<IEnumerable<ProfileTemplateExportExcel>> GetByCondition(ProfileTemplateCondition condition);
        Task<IEnumerable<ProfileTemplate>> GetProfileTemplateByStorage(IEnumerable<int> storageIDs);
        Task<VMProfileTemplate> GetDetail(int id);
        Task<VMEditProfileTemplate> GetProfileTemplate(int id);
        Task<ServiceResult> CreateProfileTemplate(VMCreateProfileTemplate vmOrgan);
        Task<ServiceResult> UpdateProfileTemplate(VMEditProfileTemplate vmOrgan);
        Task<ServiceResult> DeleteProfileTemplate(int id);
        Task<ServiceResult> DeleteMultiProfileTemplate(IEnumerable<int> id);
        Task<IEnumerable<ProfileTemplate>> GetActive(int type = 0, int id = 0);
    }
}
