using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class Organ : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int? ParentId { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string Fax { get; set; }

        public bool IsArchive { get; set; } //Cơ quan nộp lưu = false / trung tam luu tru = true
        public int Status { get; set; } = 1;
        public string Email { get; set; }
    }
}