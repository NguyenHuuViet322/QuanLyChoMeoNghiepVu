using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMPlan : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "Tên kế hoạch không được để trống")]
        [MaxLength(250, ErrorMessage = "Tên kế hoạch không được quá 250 ký tự")]
        [DisplayName("Tên kế hoạch")]
        public string Name { get; set; }

        public string OrganName { get; set; }

        [Required(ErrorMessage = "Người duyệt không được để trống")]
        [DisplayName("Người duyệt")]
        public int ApprovedBy { get; set; }

        public string ApprovedByName { get; set; }

        [Required(ErrorMessage = "Ngày tạo không được để trống")]
        [DisplayName("Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Nội dung kế hoạch")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Thời gian thu thập không được để trống")]
        [DisplayName("Từ ngày")]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "Thời gian thu thập không được để trống")]
        [DisplayName("Đến ngày")]
        public DateTime? EndDate { get; set; }

        public int Status { get; set; } = 1;
        public bool IsClosed { get; set; }
        public string Reason { get; set; }
        public int TotalProfile { get;  set; }
        public int AgencyID { get;  set; }
        public string AgencyName { get;  set; }
        public string StrCreatedAt { get; set; } 
        public string StrStatus { get; set; }
        public int TotalDoc { get; internal set; }
    }
}
