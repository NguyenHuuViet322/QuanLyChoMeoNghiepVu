using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ESD.Application.Models.ViewModels;

namespace ESD.Application.Models.MobileApiModel
{
    public class VMMobileNotification
    {

        [MaxLength(500)]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Url { get; set; }
      
        [Description("ID người tác động")]
        public int IdImpactUser { get; set; }
      
        [Description("ID thực thể bị tác động")]
        public int IdAffectedObject { get; set; }

      
        [Description("Loại thực thể bị tác động")]
        public int AffectedObjectType { get; set; }
      
        public string NameImpactUser { get; set; }
      
        public string NameAffectedObject { get; set; }
        [Description("Đơn vị người tác động")]
      
        public int IdImpactAgency { get; set; }
        [Description("Cơ quan người tác động")]
      
        public int IdImpactOrgan { get; set; }
      
        public string NameImpactAgency { get; set; }
      
        public string NameImpactOrgan { get; set; }
    }

    public class MobileNotificationCondition
    {
        public MobileNotificationCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        //public int ReaderId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        //public string Keyword { get; set; }
        public int Status { get; set; }
        //public bool NotiStatus { get; set; }
        //public string FromDate { get; set; }
        //public string ToDate { get; set; }
    }

    public class HeaderMobileNotification
    {
        public int TotalUnreadNotification { get; set; }
        public PaginatedList<VMMobileNotification> ListNotification { get; set; }
    }
}
