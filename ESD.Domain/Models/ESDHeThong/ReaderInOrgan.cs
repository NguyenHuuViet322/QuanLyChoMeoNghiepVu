using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ESD.Domain.Models.Abstractions;


namespace ESD.Domain.Models.DAS
{
    public class ReaderInOrgan : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int IDOrgan { get; set; }

        [Required]
        public int IDReader { get; set; }

        public int Status { get; set; }
    }
}
