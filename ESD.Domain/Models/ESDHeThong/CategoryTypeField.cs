using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class CategoryTypeField : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int IDCategoryType { get; set; }
        [Required]
        public int InputType { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Code { get; set; }

        [MaxLength(30)]
        public string Format { get; set; }  //for date field, number field

        public int Status { get; set; } = 1;

        public int? IDCategoryTypeRelated { get; set; } // for category field
        public int Priority { get; set; }
        public bool IsRequire { get; set; } = false;
        public bool IsShowGrid { get; set; }
        public bool IsSearchGrid { get; set; }

        public int Minlenght { get; set; }
        public int Maxlenght { get; set; }
        public long? MaxValue { get; set; }
        public long? MinValue { get; set; }

        public bool IsReadonly { get; set; }
        public int DefaultValueType { get; set; }
        public int IDOrgan { get; set; } = 0;
    }
}