using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexChuyenMonKiThuat
    {
        public PaginatedList<VMChuyenMonKiThuat> ChuyenMonKiThuats { get; set; } 
        public ChuyenMonKiThuatCondition SearchParam { get; set; } 


    }
}