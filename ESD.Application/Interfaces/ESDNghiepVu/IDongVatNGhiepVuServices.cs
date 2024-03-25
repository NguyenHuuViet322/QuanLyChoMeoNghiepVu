using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ESD.Application.Interfaces.ESDNghiepVu
{
    public interface IDongVatNghiepVuServices
    {
        Task<IEnumerable<DongVatNghiepVu>> GetsList(int? type, long id = 0);
        Task<DongVatNghiepVu> Get(long id);
        Task<VMIndexDongVatNghiepVu> SearchByConditionPagging(DongVatNghiepVuCondition DongVatNghiepVuCondition);
        Task<VMReportDongVatNghiepVu> SearchReportByConditionPagging(DongVatMat_BiLoaiCondition DongVatNghiepVuCondition);
        Task<VMUpdateDongVatNghiepVu> Create(int type);
        Task<ServiceResult> Save(VMUpdateDongVatNghiepVu vmDongVatNghiepVu);
        Task<VMUpdateDongVatNghiepVu> Update(int type, long? id);
        Task<ServiceResult> Change(VMUpdateDongVatNghiepVu vmDongVatNghiepVu);
        Task<ServiceResult> LuuDieuChuyen(VMUpdateDongVatNghiepVu vmDongVatNghiepVu);
        Task<ServiceResult> Delete(int type, long id);
        Task<ServiceResult> Delete(int type, IEnumerable<long> ids);
        Task<IEnumerable<ThongTinCanBo>> GetCanBoHuanLuyen(long? idDonvi = null);
    }
}