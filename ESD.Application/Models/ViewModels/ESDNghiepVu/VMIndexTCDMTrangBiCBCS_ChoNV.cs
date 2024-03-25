using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;
using System.Linq;
using ESD.Application.Enums.ESDTieuChuanKiemDinh;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexTCDMTrangBiCBCS_ChoNV
    {
        public PaginatedList<VMTCDMTrangBiCBCS_ChoNV> TCDMTrangBiCBCS_ChoNVs { get; set; } 
        public TCDMTrangBiCBCS_ChoNVCondition SearchParam { get; set; } 

        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; } = new List<DonViNghiepVu>();

        public IEnumerable<ChuyenMonKiThuat> ChuyenMonKiThuats { get; set; } = new List<ChuyenMonKiThuat>();

        public Dictionary<ChuyenMonKT_CanBo, int> SoLuongCanBo = new Dictionary<ChuyenMonKT_CanBo, int>();

        public Dictionary<int, string> NienHans = new Dictionary<int, string>();

        public Dictionary<int, string> DonViTinhs = new Dictionary<int, string>();
    }
}