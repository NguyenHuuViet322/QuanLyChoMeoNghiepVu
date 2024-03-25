using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class NghiepVuDongVat_DinhKem : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
 
        [Required]
        public long IDDongVatNghiepVu { get; set; }
 
        [Required]
        public string TenFile { get; set; }
 
        [Required]
        public string PathFile { get; set; }
 
        [Required]
        public string Extension { get; set; }
 
        [Required]
        public int PhanLoai { get; set; }
 
       
    }
}