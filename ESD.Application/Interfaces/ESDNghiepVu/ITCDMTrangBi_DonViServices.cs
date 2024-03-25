using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface ITCDMTrangBi_DonViServices
    {
        Task<IEnumerable<TCDMTrangBi_DonVi>> GetsList();
        Task<TCDMTrangBi_DonVi> Get(int id);
        Task<VMIndexTCDMTrangBi_DonVi> SearchByConditionPagging(TCDMTrangBi_DonViCondition TCDMTrangBi_DonViCondition);
        Task<VMUpdateTCDMTrangBi_DonVi> Create();
        Task<ServiceResult> Save(VMUpdateTCDMTrangBi_DonVi vmTCDMTrangBi_DonVi);
        Task<VMUpdateTCDMTrangBi_DonVi> Update(int? id);
        Task<ServiceResult> Change(VMUpdateTCDMTrangBi_DonVi vmTCDMTrangBi_DonVi);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);

        Task<VMIndexTCDMTrangBi_DonVi> TongHopTieuChuan(TCDMTrangBi_DonViCondition condition);
    }
}