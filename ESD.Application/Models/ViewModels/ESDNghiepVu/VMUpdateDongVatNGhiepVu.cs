using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ESD.Domain.Models.ESDNghiepVu;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMUpdateDongVatNghiepVu : VMDongVatNghiepVu
    {
       
        public IEnumerable<LoaiChoNghiepVu> LoaiChoNghiepVus { get; set; } = new List<LoaiChoNghiepVu>();
        public IEnumerable<NghiepVuDongVat> NghiepVuDongVats { get; set; } = new List<NghiepVuDongVat>();
        public IEnumerable<ThongTinCanBo> ThongTinCanBos { get; set; } = new List<ThongTinCanBo>();
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get;  set; } = new List<DonViNghiepVu>();
        public IEnumerable<NghiepVuDongVat_DinhKem> DinhKems { get;  set; } = new List<NghiepVuDongVat_DinhKem>();
        public IEnumerable<DongVatNghiepVu> DongVats { get;  set; } = new List<DongVatNghiepVu>();

        #region Save
        public string StrNgaySinh { get; set; }

        public string StrKhaiBaoDate { get; set; }
        public string ImgName { get; set; }
        public string ImgPath { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsKhaiBaoMat { get; set; }
        public int IsNew { get; set; }

        public byte IsDaChet { get; set; }
        public byte IsThaiLoai { get; set; }

        public bool IsUpdateKhaiBaoMat { get; internal set; }
        #endregion
    }
}