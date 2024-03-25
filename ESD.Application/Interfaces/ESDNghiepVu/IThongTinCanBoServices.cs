using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface IThongTinCanBoServices
    {
        Task<IEnumerable<ThongTinCanBo>> GetsList();
        Task<ThongTinCanBo> Get(int id);
        Task<VMIndexThongTinCanBo> SearchByConditionPagging(ThongTinCanBoCondition ThongTinCanBoCondition);
        Task<VMUpdateThongTinCanBo> Create();
        Task<ServiceResult> Save(VMUpdateThongTinCanBo vmThongTinCanBo);
        Task<VMUpdateThongTinCanBo> Update(int? id);
        Task<ServiceResult> Change(VMUpdateThongTinCanBo vmThongTinCanBo);
        Task<ServiceResult> Delete(long id);
        Task<ServiceResult> Delete(IEnumerable<long> ids);
    }
}