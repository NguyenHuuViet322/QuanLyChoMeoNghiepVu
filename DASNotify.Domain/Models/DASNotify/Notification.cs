using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DASNotify.Domain.Models.DASNotify
{
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Description("ID người nhận noti")]
        public int UserId { get; set; }

        [MaxLength(500)]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Url { get; set; }
        [Description("ID người tác động")]
        public int IDImpactUser { get; set; }
        [Description("ID thực thể bị tác động")]
        public int IDAffectedObject { get; set; }
        [Description("Loại thực thể bị tác động")]
        public int AffectedObjectType { get; set; }
        [Description("Đơn vị người tác động")]
        public int IDImpactAgency { get; set; }
        [Description("Cơ quan người tác động")]
        public int IDImpactOrgan { get; set; }
        [Description("Loại User Tác động")]
        public int UserImpactType { get; set; }
    }



}
