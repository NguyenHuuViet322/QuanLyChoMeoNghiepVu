using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexDongVatNghiepVu
    {
        public PaginatedList<VMDongVatNghiepVu> DongVatNghiepVus { get; set; }
        public DongVatNghiepVuCondition SearchParam { get; set; }

        public IEnumerable<LoaiChoNghiepVu> LoaiChoNghiepVus { get; set; } = new List<LoaiChoNghiepVu>();
        public IEnumerable<NghiepVuDongVat> NghiepVuDongVats { get; set; } = new List<NghiepVuDongVat>();
        public IEnumerable<ThongTinCanBo> ThongTinCanBos { get; set; } = new List<ThongTinCanBo>();
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; } = new List<DonViNghiepVu>();
        public bool IsKhaiBaoMat { get; set; }
    }
}