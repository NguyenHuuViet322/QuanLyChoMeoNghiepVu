using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMDongVatNghiepVu
    {
        public long ID { get; set; }
 
        [Display(Name = "Mã định danh", Prompt = "Mã định danh")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }
 
        [Display(Name = "Tên động vật nghiệp vụ", Prompt = "Tên động vật nghiệp vụ")]
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Ten { get; set; }
 
        [Display(Name = "Giới tính", Prompt = "Giới tính")]
        public int? GioiTinh { get; set; }
 
        [Display(Name = "Loài/Giống", Prompt = "Loài/Giống")]
        public int? IDLoaiChoNghiepVu { get; set; }
 
        [Display(Name = "Cân nặng", Prompt = "Cân nặng")]
        public double? CanNang { get; set; }
 
        [Display(Name = "Số quyết định báo sinh", Prompt = "Số quyết định báo sinh")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string SoQDBS { get; set; }
   
        [Display(Name = "Ngày khai sinh/Ngày nhập", Prompt = "Ngày khai sinh/Ngày nhập")]
        public DateTime? NgaySinh { get; set; }
 
        [Display(Name = "Số giấy chứng nhận tốt nghiệp", Prompt = "Số giấy chứng nhận tốt nghiệp")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string SoCNTotNghiep { get; set; }
 
        [Display(Name = "Chuyên khoa", Prompt = "Chuyên khoa")]
        public int? IDNghiepVuDongVat { get; set; }

        [Display(Name = "Đơn vị quản lý", Prompt = "Đơn vị quản lý")]
        public long? IDDonViQuanLy { get; set; }

        [Display(Name = "Đơn vị huấn luyện", Prompt = "Đơn vị huấn luyện")]
        public long? IDDonViNghiepVu { get; set; }
 
        [Display(Name = "Tình trạng", Prompt = "Tình trạng")]
        public int PhanLoai { get; set; }

        [Display(Name = "Ghi chú", Prompt = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Cán bộ huấn luyện", Prompt = "Cán bộ huấn luyện")]
        public long? IDThongTinCanBo { get; set; } = 0;
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public int CreatedBy { get; set; }
 
        [Required(ErrorMessage = "Giá trị không được để trống")]
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Ngày khai báo")]
        public DateTime? KhaiBaoDate { get; set; }

        public string khaiBaoDateStr { get; set; }

        public string year { get; set; }


        public int? UpdatedBy { get; set; }

        [Display(Name = "Nguyên nhân chết/bị loại", Prompt = "Nguyên nhân chết/bị loại")]
        public string LyDo { get; set; }
        
        [Display(Name = "Lý do điều chuyển", Prompt = "Lý do điều chuyển")]
        public string LyDoDieuChuyen { get; set; }

        [Display(Name = "Bố", Prompt = "Bố")]
        public long? IDChoBo { get; set; }

        [Display(Name = "Mẹ", Prompt = "Mẹ")]
        public long? IDChoMe { get; set; }

    
        [Display(Name = "Phân loại động vật", Prompt = "Phân loại động vật")]
        public int? PhanLoaiDongVat { get; set; }

        [Display(Name = "Trọng lượng tối đa", Prompt = "Trọng lượng tối đa")]
        public int? TrongLuongToiDa { get; set; }

        [Display(Name = "Số hiệu DVNV", Prompt = "Số hiệu DVNV")]
        public string SoHieu { get; set; }

        [Display(Name = "Số quyết định thải loại", Prompt = "Số quyết định thải loại")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string SoQDThaiLoai { get; set; }

    }
}