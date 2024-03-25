using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class PlanAgency
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDPlan { get; set; }

        [Required]
        public int IDAgency { get; set; }

        public int Status { get; set; } = 1;

        public DateTime? CreateDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}