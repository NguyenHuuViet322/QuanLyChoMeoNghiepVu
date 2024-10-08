﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DASNotify.Application.Models.ViewModels
{
    public class VMNotification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

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
        public string NameImpactUser { get; set; } = string.Empty;
        public string NameAffectedObject { get; set; } = string.Empty;
        [Description("Đơn vị người tác động")]
        public int IDImpactAgency { get; set; }
        [Description("Cơ quan người tác động")]
        public int IDImpactOrgan { get; set; }
        public string NameImpactAgency { get; set; } = string.Empty;
        public string NameImpactOrgan { get; set; } = string.Empty;
        [Description("Loại User Tác động")]
        public int UserImpactType { get; set; }
    }

    public class NotificationCondition
    {
        public NotificationCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int UserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public int Status { get; set; }
        public bool NotiStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class HeaderNotification
    {
        public int TotalUnreadNotification { get; set; }
        public PaginatedList<VMNotification> ListNotification { get; set; }
    }
}
