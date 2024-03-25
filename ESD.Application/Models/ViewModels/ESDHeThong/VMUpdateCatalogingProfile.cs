using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMUpdateCatalogingProfile
    {
        public int ID { get; set; }

        public int IDPlan { get; set; } = 0;
        public int IDPlanProfile { get; set; } = 0;
        [Display(Name = "Đơn vị nộp lưu", Prompt = "Đơn vị nộp lưu")]
        // [Required(ErrorMessage = "{0} không được để trống")]
        public int IDAgency { get; set; } = 0;

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mã hồ sơ", Prompt = "Mã hồ sơ")]
        [MaxLength(30, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string FileCode { get; set; }

        [Display(Name = "Kho", Prompt = "Kho")]
        public int? IDStorage { get; set; } //ID kho lấy từ danh mục động

        [Display(Name = "Giá", Prompt = "Giá")]
        public int? IDShelve { get; set; } // ID giá lấy từ danh mục động     

        [Display(Name = "Hộp số", Prompt = "Hộp số")]
        public int? IDBox { get; set; } // ID hộp số lấy từ danh mục động       

        [Display(Name = "Mục lục", Prompt = "Mục lục")]
        public int? IDProfileList { get; set; } // ID mục lục 


        [Display(Name = "Cấp độ bảo mật", Prompt = "Cấp độ bảo mật")]
        public int? IDSecurityLevel { get; set; } // ID mục lục 

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Phông", Prompt = "Phông")]
        public int? IDProfileTemplate { get; set; }  // ID bảng phông
        //public int IDOrgan { get; set; } //Dp

        //[Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mã cơ quan lưu trữ lịch sử", Prompt = "Mã cơ quan lưu trữ lịch sử")]
        [MaxLength(13, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Identifier { get; set; } //Mã cơ quan lưu trữ lịch sử


        [Display(Name = "Mục lục hoặc năm hình thành hồ sơ", Prompt = "Mục lục hoặc năm hình thành hồ sơ")]
        public int? FileCatalog { get; set; } //Mục lục số hoặc năm hình thành hồ sơ

        //[Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(20, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Số và ký hiệu hồ sơ", Prompt = "Số và ký hiệu hồ sơ")]
        public string FileNotation { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Tiêu đề hồ sơ", Prompt = "Tiêu đề hồ sơ")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Thời hạn bảo quản", Prompt = "Thời hạn bảo quản")]
        public int? IDExpiryDate { get; set; } //Thời hạn bảo quản 

        [Display(Name = "Chế độ sử dụng", Prompt = "Chế độ sử dụng")]
        [MaxLength(30, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Rights { get; set; } //Chế độ sử dụng

        [Display(Name = "Ngôn ngữ", Prompt = "Ngôn ngữ")]
        public string Language { get; set; }

        [Display(Name = "Thời gian bắt đầu", Prompt = "Thời gian bắt đầu")]
        public string StartDate { get; set; } //DD/MM/YYYY

        [Display(Name = "Thời gian kết thúc", Prompt = "Thời gian kết thúc")]
        public string EndDate { get; set; } //DD/MM/YYYY

        [Display(Name = "Tổng số văn bản trong hồ sơ", Prompt = "Tổng số văn bản trong hồ sơ")]
        public int TotalDoc { get; set; }

        [Display(Name = "Chú giải", Prompt = "Chú giải")]
        [MaxLength(2000, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Description { get; set; }

        [Display(Name = "Ký hiệu thông tin", Prompt = "Ký hiệu thông tin")]
        [MaxLength(30, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string InforSign { get; set; } // ký hiệu thông tin

        [Display(Name = "Từ khóa", Prompt = "Từ khóa")]
        [MaxLength(100, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Keyword { get; set; }

        [Display(Name = "Số lượng tờ", Prompt = "Số lượng tờ")]
        public int? Maintenance { get; set; } = 0; //Số lượng tờ 

        [Display(Name = "Số lượng trang", Prompt = "Số lượng trang")]
        public int PageNumber { get; set; } //Số lượng trang

        [Display(Name = "Tình trạng vật lý", Prompt = "Tình trạng vật lý")]
        [MaxLength(20, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Format { get; set; } //Tình trạng vật lý 

        public int Status { get; set; } = 1;

        [Display(Name = "Ngày tạo", Prompt = "Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "Lý do từ chối", Prompt = "lý do từ chối")]
        public string ReasonToReject { get; set; }

        public int Type { get; set; } //Loại

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Phân loại hồ sơ", Prompt = "Phân loại hồ sơ")]
        public int? IDProfileCategory { get; set; } //Phan loại

        [NotMapped]
        public Dictionary<int, string> DictStorage { get; set; }
        [NotMapped]
        public Dictionary<int, string> DictProfileCategory { get; set; } //Phân loại hồ sơ
        [NotMapped]
        public Dictionary<int, string> DictProfileTemplate { get; set; } //Phông
        [NotMapped]
        public Dictionary<int, string> DictBox { get; set; } //Họp
        [NotMapped]
        public Dictionary<int, string> DictLangugage { get; set; } //Ngôn ngữ
        [NotMapped]
        public Dictionary<int, string> DictExpiryDate { get; set; } //Thời hạn bảo quản
        [NotMapped]
        public Dictionary<int, string> DictSecurityLevel { get; set; } //Cấp độ bảo mật
        [NotMapped]
        public Dictionary<int, string> DictAgencies { get; set; }
        [NotMapped]
        public Dictionary<int, string> DictCates { get; set; }
        [NotMapped]
        public bool IsDetail { get; set; }
        [NotMapped]
        public bool IsApprover { get; set; }
        [NotMapped]
        public string UpdateLink { get; set; }
        [NotMapped]
        public VMPlan VMPlan { get; set; }


    }
}