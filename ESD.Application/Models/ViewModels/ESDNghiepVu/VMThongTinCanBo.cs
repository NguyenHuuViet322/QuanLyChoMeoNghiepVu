using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMThongTinCanBo
    {
        public long ID { get; set; }
 
        [Display(Name = "Tên cán bộ", Prompt = "Tên cán bộ")]
        [Required(ErrorMessage = "Tên cán bộ không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        //public string Code { get; set; }
 
        //[Display(Name = "Tên cán bộ", Prompt = "Tên cán bộ")]
        //[Required(ErrorMessage = "Tên cán bộ không được để trống")]
        //[MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string TenCanBo { get; set; }
 
        //[Display(Name = "Ngày sinh", Prompt = "Ngày sinh")]
        //[Required(ErrorMessage = "Giá trị không được để trống")]
        //public DateTime NgaySinh { get; set; }

        [Display(Name = "Giới tính", Prompt = "Giới tính")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int GioiTinh { get; set; }
 
        [Display(Name = "Đơn vị nghiệp vụ", Prompt = "Đơn vị nghiệp vụ")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public long IDDonViNghiepVu { get; set; }
 
        [Display(Name = "Chuyên môn kỹ thuật", Prompt = "Chuyên môn kỹ thuật")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int IDChuyenMonKiThuat { get; set; }
 
        [Display(Name = "Phân loại", Prompt = "Phân loại")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int PhanLoai { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int CreatedBy { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }
 
        public int? UpdatedBy { get; set; }

        [Display(Name = "Số điện thoại", Prompt = "Số điện thoại")]
        public string SDT { get; set; }

        [Ignore]
        public string TenDonViNghiepVu { get; set; }
        [Ignore]
        public string TenChuyenMonKyThuat { get; set; }
        [Ignore]
        public string Gender { get; set; }
        [Ignore]
        public string BirthDay { get; set; }
        [Ignore]
        public int SoLuongCho { get; set; }
    }
}