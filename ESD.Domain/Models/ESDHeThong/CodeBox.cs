using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class CodeBox : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int IDChannel { get; set; } = 0;
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public int FromCode { get; set; }

        public int ToCodeCode { get; set; }

        [Required]
        public int IDProfileList { get; set; }

        public int Status { get; set; } = 1;
    }
}
