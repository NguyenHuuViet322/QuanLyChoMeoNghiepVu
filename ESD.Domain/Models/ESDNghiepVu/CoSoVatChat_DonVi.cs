using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class CoSoVatChat_DonVi : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }
 
        [Required]
        public long IDCoSoVatChat { get; set; }
 
        [Required]
        public long IDDonViNghiepVu { get; set; }
 
       
    }
}