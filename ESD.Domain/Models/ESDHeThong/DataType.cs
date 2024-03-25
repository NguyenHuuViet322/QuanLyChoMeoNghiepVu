using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class DataType
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short ID { get; set; }

        [MaxLength(20)]
        [Required]
        public string Code { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public int Status { get; set; } = 1;
    }
}
