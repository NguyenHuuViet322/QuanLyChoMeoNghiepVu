using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class UserTeam : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int IDTeam { get; set; }

        public int IDUser { get; set; }

        public int Status { get; set; } = 1;
    }
}