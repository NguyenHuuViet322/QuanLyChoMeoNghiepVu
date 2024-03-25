using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class TeamRole
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDTeam { get; set; }

        [Required]
        public int IDRole { get; set; }

        public int? CreatedBy { get; set; }

        public int Status { get; set; } = 1;

        public DateTime? CreateDate { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public int? UpdatedBy { get; set; }
    }
}