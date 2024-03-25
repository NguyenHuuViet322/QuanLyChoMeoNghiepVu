using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class Email : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string FromEmail { get; set; }

        [MaxLength(1000)]
        public string ToEmail { get; set; }

        public string Content { get; set; }

        [MaxLength(250)]
        public string Title { get; set; }      

        [MaxLength(50)]
        public string EmailType { get; set; }
        
        

     
    }
}