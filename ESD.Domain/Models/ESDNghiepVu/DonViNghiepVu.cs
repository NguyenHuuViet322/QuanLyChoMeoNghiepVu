using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class DonViNghiepVu : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }
 
        [Required]
        public string Code { get; set; }
 
        [Required]
        public string Ten { get; set; }
 
        [Required]
        public string DiaChi { get; set; }
 
        [Required]
        public int SoDongVatNghiepVu { get; set; }
 
        [Required]
        public int SoCanBo { get; set; }
 
        public string ThongTinKhac { get; set; }
 
        [Required]
        public int PhanLoaiDonVi { get; set; }

        public int SoPhongLuuTruMuiHoi { get; set; } = 0;

        public int SoPhongXuLyHoi { get; set; } = 0;

        public string LanhDaoDonVi { get; set; }
        public string LanhDaoPhuTrachTT { get; set; }

        public string DoiTruong { get; set; }

        public string DoiPho { get; set; }

        public string Sdt_LanhDaoDonVi { get; set; }

        public string Sdt_LanhDaoPhuTrachTT { get; set; }

        public string Sdt_DoiTruong { get; set; }

        public string Sdt_DoiPho { get; set; }

        public int? NamThanhLap { get; set; }

        public string DonViTrucThuoc { get; set; }

        public int CauTap { get; set; }

        public int SanTap { get; set; }

        public int NhaGb { get; set; }

        public int PhuongTien { get; set; }


    }
}