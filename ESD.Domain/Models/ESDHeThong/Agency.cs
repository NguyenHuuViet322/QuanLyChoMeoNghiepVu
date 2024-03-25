using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using System.Text;
using ESD.Domain.Models.Abstractions;

namespace ESD.Domain.Models.DAS
{
    public class Agency : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        
        [Required]
        public int? IDOrgan { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public int Status { get; set; } = 1;

        [MaxLength(500)]
        public string Description { get; set; }

        public int ParentId { get; set; }

        public string ParentPath { get; set; }
    }
}