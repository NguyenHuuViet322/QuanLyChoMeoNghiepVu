using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    /// <summary>
    /// Mượn van ban - tai lieu 
    /// </summary>
    public class CatalogingBorrow : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDOrgan { get; set; } //ID co quan

        [Required]
        public int IDReader { get; set; } //ID doc gia 

        [Required]
        public string Code { get; set; } //Ma phieu tu sinh 

        [Required]
        public string Purpose { get; set; } //Muc dich

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }

        public string ReasonToReject { get; set; } //Ly do tu choi
        public int Status { get; set; } = 1;
        public int ReaderType { get; set; } //Loại độc giả (Bên ngoài/nội bộ). Table: Reader/User

        public bool IsOriginal { get; set; }  //Mượn bản gốc?
        public bool IsReturned { get; set; } //Đã trả (bản gốc)?
        public DateTime? ReturnDate { get; set; } //Ngày trả (bản gốc)?
    }
}