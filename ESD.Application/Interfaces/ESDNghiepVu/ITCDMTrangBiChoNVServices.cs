using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface ITCDMTrangBiChoNVServices
    {
        Task<VMIndexTCDMTrangBiChoNV> TongHopTieuChuan(TCDMTrangBiChoNVCondition condition);
        Task<IEnumerable<TCDMTrangBiChoNV>> GetsList();
        Task<TCDMTrangBiChoNV> Get(int id);
        Task<VMIndexTCDMTrangBiChoNV> SearchByConditionPagging(TCDMTrangBiChoNVCondition TCDMTrangBiChoNVCondition);
        Task<VMUpdateTCDMTrangBiChoNV> Create();
        Task<ServiceResult> Save(VMUpdateTCDMTrangBiChoNV vmTCDMTrangBiChoNV);
        Task<VMUpdateTCDMTrangBiChoNV> Update(int? id);
        Task<ServiceResult> Change(VMUpdateTCDMTrangBiChoNV vmTCDMTrangBiChoNV);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
    }
}