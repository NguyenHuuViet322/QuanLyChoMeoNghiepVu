using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class StgFile : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [MaxLength(250)]
        public string FileName { get; set; }

        [MaxLength(1000)]
        public string PhysicalPath { get; set; }

        public int FileType { get; set; } // // Avatar, văn bản, âm thanh, hình ảnh, video,...

        public decimal Size { get; set; }

        public bool IsTemp { get; set; } = true; // Mark file is temp

        public bool? IsEncrypted { get; set; }

        public int Status { get; set; } = 1;

        public long? IDOldFile { get; set; }

        public bool IsSign { get; set; } = false;

        public int SignType { get; set; }

        public int IDSigner { get; set; }
    }
}
