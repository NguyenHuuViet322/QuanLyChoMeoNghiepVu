using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexTCDinhLuongAnChoNV
    {
        public PaginatedList<VMTCDinhLuongAnChoNV> TCDinhLuongAnChoNVs { get; set; } 
        public TCDinhLuongAnChoNVCondition SearchParam { get; set; }
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; }
        public IEnumerable<NghiepVuDongVat> NghiepVuDongVats { get; set; }
        public IEnumerable<DongVatNghiepVu> DongVatNghiepVus { get; set; }
        public Dictionary<int, string> NienHans { get; set; }
        public Dictionary<int, string> DonViTinhs { get; set; }
    }
}