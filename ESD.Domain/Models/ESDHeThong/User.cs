using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class User : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int? IDPosition { get; set; }

        public int IDAgency { get; set; }

        public int IDOrgan { get; set; }

        [MaxLength(50)]
        public string AccountName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(20)]
        public string IdentityNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; }

        public DateTime? Birthday { get; set; }

        [MaxLength(250)]
        public string Birthplace { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        public int Gender { get; set; } = 0;

        public long? Avatar { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Status { get; set; } = 1;

        public int? IDTeam { get; set; }

        public int CountLoginFail { get; set; } = 0;

        public bool HasOrganPermission { get; set; }

        [MaxLength(250)]
        public string FileName { get; set; }

        [MaxLength(1000)]
        public string PhysicalPath { get; set; }

        public int? FileType { get; set; }

        public decimal? Size { get; set; }
    }
}