using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class CategoryType : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }
        public int IDOrgan { get; set; } = 0;

        public int Status { get; set; } = 1;
        public bool IsConfig { get; set; }
        public int? ParentId { get; set; }

        public string ParentPath { get; set; }
    }
}