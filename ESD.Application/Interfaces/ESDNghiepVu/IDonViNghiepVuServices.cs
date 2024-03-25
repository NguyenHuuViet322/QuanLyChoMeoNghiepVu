using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface IDonViNghiepVuServices
    {
        Task<IEnumerable<DonViNghiepVu>> GetsList();
        Task<DonViNghiepVu> Get(int id);
        Task<VMIndexDonViNghiepVu> SearchByConditionPagging(DonViNghiepVuCondition DonViNghiepVuCondition);
        Task<VMReportDonViNghiepVu> SearchReportByConditionPagging(DonViNghiepVuCondition DonViNghiepVuCondition);
        Task<VMUpdateDonViNghiepVu> Create(int phanLoai);
        Task<ServiceResult> Save(VMUpdateDonViNghiepVu vmDonViNghiepVu);
        Task<VMUpdateDonViNghiepVu> Update(int? id);
        Task<ServiceResult> Change(VMUpdateDonViNghiepVu vmDonViNghiepVu);
        Task<ServiceResult> Delete(long id);
        Task<ServiceResult> Delete(IEnumerable<long> ids);
        Task<VMExportReportDonViNghiepVu> ExportReport(int id);
    }
}