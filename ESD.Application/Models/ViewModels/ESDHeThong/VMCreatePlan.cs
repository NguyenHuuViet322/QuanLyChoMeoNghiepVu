using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreatePlan : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Tên kế hoạch", Prompt = "Tên kế hoạch")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Người duyệt", Prompt = "Người duyệt")]
        public int? ApprovedBy { get; set; }


        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Ngày tạo", Prompt = "Ngày tạo")]
        public string CreatedAt { get; set; }


        [Display(Name = "Nội dung", Prompt = "Nội dung")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Content { get; set; }


        //[Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Thu thập từ ngày", Prompt = "Thu thập từ ngày")]
        public string FromDate { get; set; }

        //[Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Đến ngày", Prompt = "Đến ngày")]
        public string EndDate { get; set; }

        public int Status { get; set; } = 1;
        public DateTime? ApprovedDate { get; set; }

        public Dictionary<int, string> DictUser { get; set; }

        [Display(Name = "Đóng kế hoạch", Prompt = "Đóng kế hoạch")]
        public int IsClosed { get; set; }

        public bool IsDetail { get; set; }
    }
}
