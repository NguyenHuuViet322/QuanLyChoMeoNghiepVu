using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexLoaiChoNghiepVu
    {
        public PaginatedList<VMLoaiChoNghiepVu> LoaiChoNghiepVus { get; set; } 
        public LoaiChoNghiepVuCondition SearchParam { get; set; } 


    }
}