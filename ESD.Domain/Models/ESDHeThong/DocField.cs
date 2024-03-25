using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class DocField : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDDoc { get; set; }

        [Required]
        public int IDDocTypeField { get; set; }

        public string Value { get; set; } 

        //public DateTime? DateTimeVal { get; set; }

        //public int? IntVal { get; set; }

        //public float? FloatVal { get; set; }

        public int Status { get; set; } = 1;
    }
}
