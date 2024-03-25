using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMCoSoVatChat
    {
        public long ID { get; set; }
 
        [Display(Name = "Mã định danh", Prompt = "Mã định danh")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }
 
        [Display(Name = "Tên cơ sở vật chất", Prompt = "Tên cơ sở vật chất")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Ten { get; set; }
 
        [Display(Name = "Đơn vị tính", Prompt = "Đơn vị tính")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string DonViTinh { get; set; }
 
        [Display(Name = "Niên hạn sử dụng", Prompt = "NIên hạn sử dụng")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string NienHanSuDung { get; set; }
 
        [Display(Name = "Mô tả chi tiết", Prompt = "Mô tả chi tiết")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string MoTaChiTiet { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int CreatedBy { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }
 
        public int? UpdatedBy { get; set; }
 
    }
}