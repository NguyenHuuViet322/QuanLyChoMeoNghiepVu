using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class Reader : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
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

        public int Status { get; set; } = 1;

        public int CountLoginFail { get; set; } = 0;
    }
}
