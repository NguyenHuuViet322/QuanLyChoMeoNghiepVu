using ESD.Domain.Models.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{
    
    public class TCDMTrangBiCBCS_ChoNVCondition
{
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int IdChuyenMonKiThuat { get; set; }

        public int IdDonViNghiepVu { get; set; }

        public int NienHan { get; set; }

        public TCDMTrangBiCBCS_ChoNVCondition()
        {
            PageIndex = 1;
            PageSize = 10;
            NienHan = 0;
            IdChuyenMonKiThuat = 0;
            IdDonViNghiepVu = 0;
        }
    }
}