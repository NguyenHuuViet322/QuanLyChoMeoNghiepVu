using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexTCDMTrangBi_DonVi
    {
        public PaginatedList<VMTCDMTrangBi_DonVi> TCDMTrangBi_DonVis { get; set; } 
        public TCDMTrangBi_DonViCondition SearchParam { get; set; }
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; }
        public IEnumerable<SelectListItem> NienHans { get; set; }
        public Dictionary<int, string> LoaiPhongs { get; set; }
        public Dictionary<int, string> DonViTinhs { get; set; }
    }
}