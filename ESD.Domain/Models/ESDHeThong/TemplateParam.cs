
using ESD.Domain.Models.Abstractions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class TemplateParam : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Code { get; set; }
        public int IDTemplate { get; set; }
        [Description("Keyword for Template")]
        public string Name { get; set; }
        public int IDOrgan { get; set; }
    }
}
