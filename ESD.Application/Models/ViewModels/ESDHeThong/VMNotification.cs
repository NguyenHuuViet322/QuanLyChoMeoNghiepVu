using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.DASNotify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ESD.Application.Models.ViewModels
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
    }
}
