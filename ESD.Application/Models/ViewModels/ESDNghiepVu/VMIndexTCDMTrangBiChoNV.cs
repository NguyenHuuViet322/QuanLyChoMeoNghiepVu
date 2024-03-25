using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexTCDMTrangBiChoNV
    {
        public PaginatedList<VMTCDMTrangBiChoNV> TCDMTrangBiChoNVs { get; set; } 
        public TCDMTrangBiChoNVCondition SearchParam { get; set; }
        public IEnumerable<DongVatNghiepVu> DongVatNghiepVus { get; set; }
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; }
        public IEnumerable<NghiepVuDongVat> NghiepVuDongVats { get; set; }
        public Dictionary<int, string> NienHans { get; set; }
        public Dictionary<int, string> DonViTinhs { get; set; }
    }
}