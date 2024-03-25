using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;
using ESD.Application.Enums.DasKTNN;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMExportReportDonViNghiepVu
    {
        public IEnumerable<VMDongVatNghiepVu> DongVats { get; set; } = new List<VMDongVatNghiepVu>();
        public VMDonViNghiepVu DonVi { get; set; }
        public DonViNghiepVuCondition SearchParam { get; set; }

        public string TenPhanLoai
        {
            get { return SearchParam.PhanLoai == (int)PhanLoaiDonVi.TraiGiam ? "trại giam" : "đơn vị"; }
        }

        public IEnumerable<VMThongTinCanBo> CanBos { get; set; } = new List<VMThongTinCanBo>();
        public IEnumerable<VMChuyenMonKiThuat> ChuyenMonKiThuats { get; set; } = new List<VMChuyenMonKiThuat>();
        public IEnumerable<VMNghiepVuDongVat> NghiepVuDongVats { get; set; } = new List<VMNghiepVuDongVat>();
        public IEnumerable<VMLoaiChoNghiepVu> LoaiChoNghiepVus { get;  set; } = new List<VMLoaiChoNghiepVu>();
    }
}