using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ESD.Domain.Models.Abstractions;

namespace ESD.Domain.Models.DAS
{
    public class Position : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int Parent { get; set; }

        public string ParentPath { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Code { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int Status { get; set; } = 1;
    }
}