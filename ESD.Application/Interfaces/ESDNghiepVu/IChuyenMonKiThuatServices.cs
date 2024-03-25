using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface IChuyenMonKiThuatServices
    {
        Task<IEnumerable<ChuyenMonKiThuat>> GetsList();
        Task<ChuyenMonKiThuat> Get(int id);
        Task<VMIndexChuyenMonKiThuat> SearchByConditionPagging(ChuyenMonKiThuatCondition ChuyenMonKiThuatCondition);
        Task<VMUpdateChuyenMonKiThuat> Create();
        Task<ServiceResult> Save(VMUpdateChuyenMonKiThuat vmChuyenMonKiThuat);
        Task<VMUpdateChuyenMonKiThuat> Update(int? id);
        Task<ServiceResult> Change(VMUpdateChuyenMonKiThuat vmChuyenMonKiThuat);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
    }
}