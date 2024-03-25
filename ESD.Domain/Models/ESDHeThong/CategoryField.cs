using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class CategoryField : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDCategory { get; set; }

        [Required]
        public int IDCategoryTypeField { get; set; }

        public string StringVal { get; set; } //nvarchar

        public DateTime? DateTimeVal { get; set; }

        public long? IntVal { get; set; }

        public float? FloatVal { get; set; }

        public int Status { get; set; } = 1;
        public int IDOrgan { get; set; } = 0;
    }
}