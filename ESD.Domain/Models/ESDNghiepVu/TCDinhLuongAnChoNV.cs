using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class TCDinhLuongAnChoNV : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
 
        [Required]
        public string DanhMucDinhMuc { get; set; }
 
        public double? Tu30KgDuoi2Thang_DM { get; set; }
 
        [Required]
        public double Tu30KgTu3Den4Thang_DM { get; set; }
 
        public double? Tu30KgTu5Den10Thang_DM { get; set; }
 
        public double? Tu30KgTu11Thang_DM { get; set; }
 
        public double? HuanLuyenTu30Kg_DM { get; set; }
 
        public double? GiongTu30Kg_DM { get; set; }
 
        public double? Duoi30KgDuoi2Thang_DM { get; set; }
 
        public double? Duoi30KgTu3Den4Thang_DM { get; set; }
 
        public double? Duoi30KgTu5Den10Thang_DM { get; set; }
 
        public double? Duoi30KgTu11Thang_DM { get; set; }
 
        public double? HuanLuyenDuoi30Kg_DM { get; set; }
 
        public double? GiongDuoi30Kg_DM { get; set; }
 
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