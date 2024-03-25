using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface ITCDinhLuongAnChoNVServices
    {
        Task<IEnumerable<TCDinhLuongAnChoNV>> GetsList();
        Task<TCDinhLuongAnChoNV> Get(int id);
        Task<VMIndexTCDinhLuongAnChoNV> SearchByConditionPagging(TCDinhLuongAnChoNVCondition TCDinhLuongAnChoNVCondition);
        Task<VMIndexTCDinhLuongAnChoNV> TongHopDinhMuc(TCDinhLuongAnChoNVCondition TCDinhLuongAnChoNVCondition);
        Task<VMUpdateTCDinhLuongAnChoNV> Create();
        Task<ServiceResult> Save(VMUpdateTCDinhLuongAnChoNV vmTCDinhLuongAnChoNV);
        Task<VMUpdateTCDinhLuongAnChoNV> Update(int? id);
        Task<ServiceResult> Change(VMUpdateTCDinhLuongAnChoNV vmTCDinhLuongAnChoNV);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
    }
}