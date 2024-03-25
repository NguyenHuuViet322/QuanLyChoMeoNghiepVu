using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace ESD.Domain.Models.DAS
{
    public class ModuleChild
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDModule { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
    }
}
