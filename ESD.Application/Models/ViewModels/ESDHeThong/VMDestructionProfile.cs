using System;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMDestructionProfile
    {
        public int ID { get; set; }
        public int IDOrgan { get; set; }
        public int IDUser { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Tên quyết định tiêu hủy", Prompt = "Tên quyết định tiêu hủy")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Phải chọn {0}")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Phải chọn {0}")]
        [Display(Name = "Người duyệt", Prompt = "Người duyệt")]
        public int ApprovedBy { get; set; }

        [Required(ErrorMessage = "Phải chọn {0}")]
        [Display(Name = "Ngày tạo quyết định", Prompt = "Ngày tạo quyết định")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Lý do tiêu hủy", Prompt = "Lý do tiêu hủy")]
        public string ReasonToDestruction { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Nội dung quyết định", Prompt = "Nội dung quyết định")]
        public string Description { get; set; }

        public string ReasonToReject { get; set; }

        [Display(Name = "Trạng thái", Prompt = "Trạng thái")]
        public int Status { get; set; } = 1;

    }
}
