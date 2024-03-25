using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class DongVatNghiepVu : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }
 
        public string Code { get; set; }
 
        [Required]
        public string Ten { get; set; }
 
        public int? GioiTinh { get; set; }
 
        public int? IDLoaiChoNghiepVu { get; set; }
 
        public double CanNang { get; set; }
 
        public string SoQDBS { get; set; }
 
        public DateTime? NgaySinh { get; set; }
 
        public string SoCNTotNghiep { get; set; }
 
        public int? IDNghiepVuDongVat { get; set; }

        public long? IDDonViNghiepVu { get; set; }

        public long? IDDonViQuanLy { get; set; }

        public DateTime? KhaiBaoDate { get; set; }

        [NotMapped]
        public string? khaiBaoDateStr { get; set; }
 
        public int PhanLoai { get; set; }
 
        public long? IDThongTinCanBo { get; set; }

        public string GhiChu { get; set; }
        public string LyDoDieuChuyen { get; set; }
        public string LyDo { get; set; }
        public long? IDChoBo { get; set; }
        public long? IDChoMe { get; set; }

        public int TrongLuongToiDa { get; set; }
        public int PhanLoaiDongVat { get; set; }

        public string SoHieu { get; set; }
        public string SoQDThaiLoai { get; set; }
    }
}