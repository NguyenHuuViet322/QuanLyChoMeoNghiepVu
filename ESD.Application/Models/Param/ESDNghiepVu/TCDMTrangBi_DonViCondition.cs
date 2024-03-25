using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{
    
    public class TCDMTrangBi_DonViCondition
{
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public long IDDonvi  { get; set; }
        public int NienHan  { get; set; }

        public TCDMTrangBi_DonViCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}