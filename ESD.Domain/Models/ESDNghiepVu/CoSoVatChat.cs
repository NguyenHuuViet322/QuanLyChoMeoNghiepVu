using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class CoSoVatChat : Auditable
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
        public string DonViTinh { get; set; }
 
        [Required]
        public string NienHanSuDung { get; set; }
 
        [Required]
        public string MoTaChiTiet { get; set; }
 
       
    }
}