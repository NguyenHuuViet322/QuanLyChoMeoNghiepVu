using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexThongTinCanBo
    {
        public PaginatedList<VMThongTinCanBo> ThongTinCanBos { get; set; } 
        public ThongTinCanBoCondition SearchParam { get; set; } 

        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; } = new List<DonViNghiepVu>();
        public IEnumerable<ChuyenMonKiThuat> ChuyenMonKiThuats { get; set; } = new List<ChuyenMonKiThuat>();
        public Dictionary<int, string> LoaiCanBos { get; set; }
    }
}