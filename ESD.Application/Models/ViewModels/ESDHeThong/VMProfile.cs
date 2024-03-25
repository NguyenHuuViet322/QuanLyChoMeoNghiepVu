using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMProfile
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        public int IDOrgan { get; set; } //Cơ quan
        public int Type { get; set; }


        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mã hồ sơ", Prompt = "Mã hồ sơ")]
        public string FileCode { get; set; }

        //[Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Kho", Prompt = "Kho")]
        public int? IDStorage { get; set; } //ID kho lấy từ danh mục động

        [Display(Name = "Hộp số", Prompt = "Hộp số")]
        public int IDCodeBox { get; set; } // ID hộp số lấy từ danh mục động 

        [Display(Name = "Mục lục", Prompt = "Mục lục")]
        public int IDProfileList { get; set; } // ID mục lục 

        //public int IDOrgan { get; set; } //Dp
        public int? IDSecurityLevel { get; set; } // ID cấp độ bảo mật 


        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mã cơ quan lưu trữ lịch sử", Prompt = "Mã cơ quan lưu trữ lịch sử")]
        public string Identifier { get; set; } //Mã cơ quan lưu trữ lịch sử

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Phông", Prompt = "Phông")]
        public int IDProfileTemplate { get; set; }  // ID bảng phông

        [Display(Name = "Năm hình thành hồ sơ", Prompt = "Năm hình thành hồ sơ")]
        public int FileCatalog { get; set; } //Mục lục số hoặc năm hình thành hồ sơ

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Số và ký hiệu hồ sơ", Prompt = "Số và ký hiệu hồ sơ")]
        public string FileNotation { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tiêu đề hồ sơ", Prompt = "Tiêu đề hồ sơ")]
        public string Title { get; set; }

        [Display(Name = "Thời hạn bảo quản", Prompt = "Thời hạn bảo quản")]
        public int IDExpiryDate { get; set; } //Thời hạn bảo quản 

        [Display(Name = "Chế độ sử dụng", Prompt = "Chế độ sử dụng")]
        public string Rights { get; set; } //Chế độ sử dụng

        [Display(Name = "Ngôn ngữ", Prompt = "Ngôn ngữ")]
        public string Language { get; set; }

        [Display(Name = "Thời gian bắt đầu", Prompt = "Thời gian bắt đầu")]
        public DateTime? StartDate { get; set; } //DD/MM/YYYY

        [Display(Name = "Thời gian kết thúc", Prompt = "Thời gian kết thúc")]
        public DateTime? EndDate { get; set; } //DD/MM/YYYY

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tổng số văn bản trong hồ sơ", Prompt = "Tổng số văn bản trong hồ sơ")]
        public int TotalDoc { get; set; }

        [Display(Name = "Chú giải", Prompt = "Chú giải")]
        public string Description { get; set; }

        [Display(Name = "Ký hiệu thông tin", Prompt = "Ký hiệu thông tin")]
        public string InforSign { get; set; } // ký hiệu thông tin

        [Display(Name = "Từ khóa", Prompt = "Từ khóa")]
        public string Keyword { get; set; }

        [Display(Name = "Số lượng tờ", Prompt = "Số lượng tờ")]
        public int Maintenance { get; set; } //Số lượng tờ 

        [Display(Name = "Số lượng trang", Prompt = "Số lượng trang")]
        public int PageNumber { get; set; } //Số lượng trang

        [Display(Name = "Tình trạng vật lý", Prompt = "Tình trạng vật lý")]
        public string Format { get; set; } //Tình trạng vật lý 

        public int Status { get; set; } = 1;

        public int IDAgency { get; set; } //Đơn vị

        public string ResonToRefuse { get; set; }

        #region ResultColumn
        public string ProfileTemplateName { get; set; }  //Tên bảng phông

        #endregion
    }

}
