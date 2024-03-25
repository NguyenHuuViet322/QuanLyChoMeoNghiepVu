using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class GroupPermission
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public int Status { get; set; } = 1;

        public bool IsBase { get; set; }

        [Description("ID Cơ quan")]
        public int IDOrgan { get; set; } = 0;

        [Description("Là admin cơ quan?")]
        public bool IsAdminOrgan { get; set; } = false;

        public int ActiveNotification { get; set; } = 0;
    }
}