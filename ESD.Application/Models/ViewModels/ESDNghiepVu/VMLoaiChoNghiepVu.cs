using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMLoaiChoNghiepVu
    {
        public int ID { get; set; }
 
        [Display(Name = "Mã loại chó nghiệp vụ", Prompt = "Mã loại chó nghiệp vụ")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }
 
        [Display(Name = "Tên loại chó nghiệp vụ", Prompt = "Tên loại chó nghiệp vụ")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Ten { get; set; }
 
        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string MoTa { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int CreatedBy { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }
 
        public int? UpdatedBy { get; set; }
 
    }
}