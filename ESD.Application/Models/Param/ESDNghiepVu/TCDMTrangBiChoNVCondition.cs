using ESD.Domain.Models.ESDNghiepVu;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.Param.ESDNghiepVu
{
    
    public class TCDMTrangBiChoNVCondition
{
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public long IDDonvi { get; set; }
        public int NienHan { get; set; }
        public DateTime CalculationDate { get; set; } = DateTime.Now;
        public int IdDonViNghiepVu { get; set; }
        public string DongVat { get; set; }

        public List<string> ListDongVat
        {
            get
            {
                if (DongVat.IsNotEmpty())
                    return DongVat.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }

        public TCDMTrangBiChoNVCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}