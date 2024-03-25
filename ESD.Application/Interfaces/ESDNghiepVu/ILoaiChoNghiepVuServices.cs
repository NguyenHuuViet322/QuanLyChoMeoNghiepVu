using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface ILoaiChoNghiepVuServices
    {
        Task<IEnumerable<LoaiChoNghiepVu>> GetsList();
        Task<LoaiChoNghiepVu> Get(int id);
        Task<VMIndexLoaiChoNghiepVu> SearchByConditionPagging(LoaiChoNghiepVuCondition LoaiChoNghiepVuCondition);
        Task<VMUpdateLoaiChoNghiepVu> Create();
        Task<ServiceResult> Save(VMUpdateLoaiChoNghiepVu vmLoaiChoNghiepVu);
        Task<VMUpdateLoaiChoNghiepVu> Update(int? id);
        Task<ServiceResult> Change(VMUpdateLoaiChoNghiepVu vmLoaiChoNghiepVu);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
    }
}