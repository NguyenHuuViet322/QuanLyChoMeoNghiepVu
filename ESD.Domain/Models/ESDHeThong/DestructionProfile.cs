using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class DestructionProfile:Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int IDOragan { get; set; } = 0;

        [Required]
        public int IDUser { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public int ApprovedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(500)]
        public string ReasonToDestruction { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTime? ApprovedDate { get; set; } //Ngày duyệt       
        public string ReasonToReject { get; set; }
        public int Status { get; set; } = 1;
    }
}
