using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{
    
    public class ChuyenMonKiThuatCondition
{
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }



        public ChuyenMonKiThuatCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}