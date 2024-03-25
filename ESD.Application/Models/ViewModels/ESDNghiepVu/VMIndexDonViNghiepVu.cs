using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;
using ESD.Application.Enums.DasKTNN;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexDonViNghiepVu
    {
        public PaginatedList<VMDonViNghiepVu> DonViNghiepVus { get; set; }
        public DonViNghiepVuCondition SearchParam { get; set; }

        public string TenPhanLoai
        {
            get { return SearchParam.PhanLoai == (int)PhanLoaiDonVi.TraiGiam ? "trại giam" : "đơn vị"; }
        }
    }
}