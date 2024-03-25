using ESD.Application.Enums.DasKTNN;
using ESD.Domain.Models.ESDNghiepVu;
using System;
using System.Collections.Generic;

namespace ESD.Application.Models.Param.ESDNghiepVu
{

    public class DongVatMat_BiLoaiCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Type { get; set; }
        public string Year { get; set; }

        public int? IDLoaiChoNghiepVu { get; set; }
        public int? IDDonViQuanLy { get; set; }
        public int? IDNghiepVuDongVat { get; set; }
        public int? IDThongTinCanBo { get; set; }


        public string MenuTypeName
        {

            get
            {
                return Type == (int)MenuDVNV.Loai ? "Động vật chết, bị loại"
                     : "Động vật nghiệp vụ";
            }
        }

        public DongVatMat_BiLoaiCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}