using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreateDestructionProfile 
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
        public string CreatedAt { get; set; } 

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Lý do tiêu hủy", Prompt = "Lý do tiêu hủy")]
        public string ReasonToDestruction { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Nội dung quyết định", Prompt = "Nội dung quyết định")]
        public string Description { get; set; }

        [Display(Name = "Trạng thái", Prompt = "Trạng thái")]
        public int Status { get; set; } = 1;

        public List<int> ListProfile { get; set; }
        //Profile
        public PaginatedList<VMPlanProfile> VMPlanProfiles { get; set; }
        //Dict
        public Dictionary<int, string> DictUser { get; set; }
        public Dictionary<int, string> DictStorage { get; set; }
        public Dictionary<int, string> DictSheleve { get; set; }
        public Dictionary<int, string> DictBox { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
    }

    
}
