using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class OrganConfig : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        public int IDOrgan { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(64)")]
        [MaxLength(64)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string StringVal { get; set; }

        public DateTime? DateTimeVal { get; set; }

        public long? IntVal { get; set; }

        public float? FloatVal { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public int Status { get; set; } = 1;
    }
}
