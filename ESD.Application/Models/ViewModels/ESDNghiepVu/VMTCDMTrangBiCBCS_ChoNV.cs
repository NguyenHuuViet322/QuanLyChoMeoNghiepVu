using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMTCDMTrangBiCBCS_ChoNV
    {
        public int ID { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string DanhMucDinhMuc { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string NhomDanhMucDinhMuc { get; set; }
 
        public double? CanBoQL_DM { get; set; }
 
        public double? GiaoVienHD_DM { get; set; }
 
        public double? CanBoQLChoNV_DM { get; set; }
 
        public double? HocVien_DM { get; set; }
 
        public double? CanBoThuY_DM { get; set; }
 
        public double? NVCapDuong_DM { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int DonViTinh { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int NienHan { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int CreatedBy { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }
 
        public int? UpdatedBy { get; set; }

        [NotMapped]
        public int? CapPhat { get; set; }
    }
}