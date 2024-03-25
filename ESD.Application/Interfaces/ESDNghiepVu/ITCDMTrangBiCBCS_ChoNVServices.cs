using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface ITCDMTrangBiCBCS_ChoNVServices
    {
        Task<IEnumerable<TCDMTrangBiCBCS_ChoNV>> GetsList();
        Task<TCDMTrangBiCBCS_ChoNV> Get(int id);
        Task<List<DonViNghiepVu>> GetsListDonVi(int IdDonViNghiepVu);
        Task<VMIndexTCDMTrangBiCBCS_ChoNV> SearchByConditionPagging(TCDMTrangBiCBCS_ChoNVCondition TCDMTrangBiCBCS_ChoNVCondition);
        Task<VMUpdateTCDMTrangBiCBCS_ChoNV> Create();
        Task<ServiceResult> Save(VMUpdateTCDMTrangBiCBCS_ChoNV vmTCDMTrangBiCBCS_ChoNV);
        Task<VMUpdateTCDMTrangBiCBCS_ChoNV> Update(int? id);
        Task<ServiceResult> Change(VMUpdateTCDMTrangBiCBCS_ChoNV vmTCDMTrangBiCBCS_ChoNV);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
    }
}