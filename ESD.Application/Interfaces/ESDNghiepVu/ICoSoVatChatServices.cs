using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface ICoSoVatChatServices
    {
        Task<IEnumerable<CoSoVatChat>> GetsList();
        Task<CoSoVatChat> Get(long id);
        Task<VMIndexCoSoVatChat> SearchByConditionPagging(CoSoVatChatCondition CoSoVatChatCondition);
        Task<VMUpdateCoSoVatChat> Create();
        Task<ServiceResult> Save(VMUpdateCoSoVatChat vmCoSoVatChat);
        Task<VMUpdateCoSoVatChat> Update(long? id);
        Task<ServiceResult> Change(VMUpdateCoSoVatChat vmCoSoVatChat);
        Task<ServiceResult> Delete(long id);
        Task<ServiceResult> Delete(IEnumerable<long> ids);
    }
}