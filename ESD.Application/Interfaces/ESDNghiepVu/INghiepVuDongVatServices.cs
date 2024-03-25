using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface INghiepVuDongVatServices
    {
        Task<IEnumerable<NghiepVuDongVat>> GetsList();
        Task<NghiepVuDongVat> Get(int id);
        Task<VMIndexNghiepVuDongVat> SearchByConditionPagging(NghiepVuDongVatCondition NghiepVuDongVatCondition);
        Task<VMUpdateNghiepVuDongVat> Create();
        Task<ServiceResult> Save(VMUpdateNghiepVuDongVat vmNghiepVuDongVat);
        Task<VMUpdateNghiepVuDongVat> Update(int? id);
        Task<ServiceResult> Change(VMUpdateNghiepVuDongVat vmNghiepVuDongVat);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
    }
}