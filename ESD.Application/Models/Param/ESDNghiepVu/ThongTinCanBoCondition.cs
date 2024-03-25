using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{
    
    public class ThongTinCanBoCondition
{
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public int Type { get; set; }
        public int IDDonViNghiepVu { get; set; }
        public int IDChuyenMonKiThuat { get; set; }

        public ThongTinCanBoCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}