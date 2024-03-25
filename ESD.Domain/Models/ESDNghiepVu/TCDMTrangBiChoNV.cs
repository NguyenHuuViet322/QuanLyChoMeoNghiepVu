using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class TCDMTrangBiChoNV : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
 
        [Required]
        public string DanhMucDinhMuc { get; set; }
 
        public double? Tu3Den4Thang_DM { get; set; }
 
        [Required]
        public double Tu5Den10Thang_DM { get; set; }
 
        public double? Tren11Thang_DM_DM { get; set; }
 
        public double? ChoGiong_DM { get; set; }
 
        public double? ChuyenThuocNo_DM { get; set; }
 
        public double? ChuyenMaTuy_DM { get; set; }
 
        public double? ChuyenTimDauVet_DM { get; set; }
 
        public double? ChuyenGiamBiet_DM { get; set; }
 
        public double? ChuyenCuuNan_DM { get; set; }
 
        public double? NhapNoiTu30KgTrong12Thang_DM { get; set; }
 
        public double? NhapNoiTu30KgTu13Thang_DM { get; set; }
 
        public double? NhapNoiDuoi30Trong12Thang_DM { get; set; }
 
        public double? NhapNoiDuoi30Tu13Thang_DM { get; set; }
 
        [Required]
        public int DonViTinh { get; set; }
 
        [Required]
        public int NienHan { get; set; }
 
       
    }
}