using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class ProfileDestroyed : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int IDDestruction { get; set; }

        [Required]
        public int IDCatalogingProfile { get; set; }
        public int IDOrgan { get; set; } = 0;

        [Description("Tiêu đề hồ sơ")]
        [MaxLength(1000)]
        public string Title { get; set; }

        [Description("Mã hồ sơ")]
        [MaxLength(30)]
        public string FileCode { get; set; }

        [Required]
        public int IDExpiryDate { get; set; }
        public int IDStorage { get; set; }
        public int IDShelve { get; set; }
        public int IDBox { get; set; }

        //ForDestruction
        public int InUsing { get; set; }

        public int Status { get; set; } = 1;
    }
}
