using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMTCDMTrangBi_DonVi
    {
        public int ID { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string DanhMucDinhMuc { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int MaPhong { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int DonViTinh { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int NienHan { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public double SoLuong { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public double DuTru { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int CreatedBy { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }
 
        public int? UpdatedBy { get; set; }



        //Result
        public string StrNienHan { get; set; }
        public string StrDonViTinh { get; set; }
        public string StrPhong { get; set; }

        public string StrDuTru
        {
            get
            {
                return DuTru.ToString("0.##");
            }
        }


    }
}