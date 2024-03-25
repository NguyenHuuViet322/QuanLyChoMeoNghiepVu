using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMTCDinhLuongAnChoNV
    {
        public int ID { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string DanhMucDinhMuc { get; set; }
 
        public double? Tu30KgDuoi2Thang_DM { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public double? Tu30KgTu3Den4Thang_DM { get; set; }
 
        public double? Tu30KgTu5Den10Thang_DM { get; set; }
 
        public double? Tu30KgTu11Thang_DM { get; set; }
 
        public double? HuanLuyenTu30Kg_DM { get; set; }
 
        public double? GiongTu30Kg_DM { get; set; }
 
        public double? Duoi30KgDuoi2Thang_DM { get; set; }
 
        public double? Duoi30KgTu3Den4Thang_DM { get; set; }
 
        public double? Duoi30KgTu5Den10Thang_DM { get; set; }
 
        public double? Duoi30KgTu11Thang_DM { get; set; }
 
        public double? HuanLuyenDuoi30Kg_DM { get; set; }
 
        public double? GiongDuoi30Kg_DM { get; set; }
 
        public double? NhapNoiTu30KgTrong12Thang_DM { get; set; }
 
        public double? NhapNoiTu30KgTu13Thang_DM { get; set; }
 
        public double? NhapNoiDuoi30Trong12Thang_DM { get; set; }
 
        public double? NhapNoiDuoi30Tu13Thang_DM { get; set; }
 
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

        public double SoLuong { get; set; }
    }
}