using ESD.Application.Enums.DasKTNN;
using ESD.Domain.Models.ESDNghiepVu;
using System;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{

    public class DongVatNghiepVuCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int? Type { get; set; } 
        public string Keyword { get; set; }

        public int? IDLoaiChoNghiepVu { get; set; }
        public int? IDDonViQuanLy { get; set; }
        public int? IDNghiepVuDongVat { get; set; }
        public int? IDCanBo { get; set; }


        public string MenuTypeName
        {

            get
            {
                return Type == (int)MenuDVNV.Loai ? "Động vật chết, bị loại"
                     : "Động vật nghiệp vụ";
            }
        }

        public int[] PhanLoai { get;  set; }
        public int? Loai { get;  set; }

        public DongVatNghiepVuCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}