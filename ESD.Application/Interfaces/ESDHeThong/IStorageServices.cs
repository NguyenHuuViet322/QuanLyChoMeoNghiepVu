using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IStorageServices : IBaseMasterService<Storage>
    {
        Task<VMIndexProfileAndDoc> SearchProfileAndDocByConditionPaging(SearchProfileCondition condition);


        Task<VMIndexDocCatalogingProfile> PlanDocDetailIndex(SearchProfileCondition condition);
        Task<VMIndexDocCatalogingProfile> PlanDocDetailIndexNoPaging(SearchProfileCondition condition);
        Task<VMSearchProfileDoc> GetDocCollect(int IDDoc);
        Task<VMIndexProfileAndDoc> PortalSearch(SearchProfileCondition condition);
        Task<IEnumerable<ProfileTemplate>> GetListProfileTemplate(List<int> ids);
    }
}
