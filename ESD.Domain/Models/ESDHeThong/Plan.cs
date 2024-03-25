using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class Plan : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        public int IDOrgan { get; set; }  //Tổ chức

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public int ApprovedBy { get; set; }

        public int SendApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; } //Ngày duyệt

        public DateTime CreatedAt { get; set; }

        public string Content { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Status { get; set; } = 1;
        public string Reason { get; set; }
        public bool IsClosed { get; set; }
    }
}