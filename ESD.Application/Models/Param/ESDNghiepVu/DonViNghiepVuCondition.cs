using ESD.Application.Enums.DasKTNN;
using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{

    public class DonViNghiepVuCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public int PhanLoai { get; set; }

     


        public DonViNghiepVuCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}