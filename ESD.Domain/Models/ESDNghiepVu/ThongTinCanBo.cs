using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class ThongTinCanBo : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }
 
        public string Code { get; set; }
 
        [Required]
        public string TenCanBo { get; set; }
 
        public DateTime NgaySinh { get; set; }
 
        [Required]
        public int GioiTinh { get; set; }
 
        [Required]
        public long IDDonViNghiepVu { get; set; }
 
        public int IDChuyenMonKiThuat { get; set; }
 
        public int PhanLoai { get; set; }

        public string SDT { get; set; }
    }
}