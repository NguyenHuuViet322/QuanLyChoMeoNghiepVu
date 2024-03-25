using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.ESDNghiepVu
{
    public class ChuyenMonKiThuat : Auditable
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
 
        [Required]
        public string Code { get; set; }
 
        [Required]
        public string Ten { get; set; }
 
        [Required]
        public string MoTa { get; set; }
 
       
    }
}