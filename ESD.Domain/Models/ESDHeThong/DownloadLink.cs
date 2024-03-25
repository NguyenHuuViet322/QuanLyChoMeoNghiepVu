using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class DownloadLink
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string DownloadHash { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public long? IDFile { get; set; }

        public int? IDFolder { get; set; }

        public long? CreatedBy { get; set; }

    }
}
