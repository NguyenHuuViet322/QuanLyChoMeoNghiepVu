using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    /// <summary>
    /// Mượn van ban - tai lieu 
    /// </summary>
    public class CatalogingBorrowDoc : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDCatalogingBorrow { get; set; } //ID phieu
       
        [Required]
        public int IDProfile { get; set; } //ID hồ sơ  
        [Required]
        public int IDDoc { get; set; } //ID tài liệu  

        public int Status { get; set; } = 1;
    }
}