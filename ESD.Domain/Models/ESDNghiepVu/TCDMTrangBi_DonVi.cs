using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class TCDMTrangBi_DonVi : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
 
        [Required]
        public string DanhMucDinhMuc { get; set; }
 
        [Required]
        public int MaPhong { get; set; }
 
        [Required]
        public double DonViTinh { get; set; }
 
        [Required]
        public int NienHan { get; set; }
 
        [Required]
        public int SoLuong { get; set; }
 
        [Required]
        public double DuTru { get; set; }
 
       
    }
}