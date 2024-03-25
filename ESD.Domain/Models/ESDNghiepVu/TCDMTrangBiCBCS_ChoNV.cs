using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class TCDMTrangBiCBCS_ChoNV : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
 
        [Required]
        public string DanhMucDinhMuc { get; set; }
 
        [Required]
        public string NhomDanhMucDinhMuc { get; set; }
 
        public double? CanBoQL_DM { get; set; }
 
        public double? GiaoVienHD_DM { get; set; }
 
        public double? CanBoQLChoNV_DM { get; set; }
 
        public double? HocVien_DM { get; set; }
 
        public double? CanBoThuY_DM { get; set; }
 
        public double? NVCapDuong_DM { get; set; }
 
        [Required]
        public int DonViTinh { get; set; }
 
        [Required]
        public int NienHan { get; set; }
 
       
    }
}