using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMDonViNghiepVu
    {
        public long ID { get; set; }
 
        [Display(Name = "Mã định danh", Prompt = "Mã định danh")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }
 
        [Display(Name = "Tên đơn vị", Prompt = "Tên đơn vị")]
        [Required(ErrorMessage = "Trường này không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Ten { get; set; }
 
        [Display(Name = "Địa chỉ", Prompt = "Địa chỉ")]
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string DiaChi { get; set; }
 
        [Display(Name = "Số động vật nghiệp vụ", Prompt = "Số động vật nghiệp vụ")]
        [Range(0, int.MaxValue, ErrorMessage = "Không được nhập giá trị âm")]
        public int? SoDongVatNghiepVu { get; set; }
 
        [Display(Name = "Số cán bộ", Prompt = "Số cán bộ")]
        [Range(0, int.MaxValue, ErrorMessage = "Không được nhập giá trị âm")]
        public int? SoCanBo { get; set; }
 
        [Display(Name = "Thông tin khác", Prompt = "Thông tin khác")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string ThongTinKhac { get; set; }
 
        [Display(Name = "Phân loại đơn vị", Prompt = "Phân loại đơn vị")]
        public int PhanLoaiDonVi { get; set; }
 
        public int CreatedBy { get; set; }
 
        public DateTime CreateDate { get; set; }
 
        public DateTime? UpdatedDate { get; set; }
 
        public int? UpdatedBy { get; set; }

        [Display(Name = "Số lượng phòng lưu trữ, bảo quản mùi hơi", Prompt = "Số lượng phòng lưu trữ, bảo quản mùi hơi")]
        [Range(0, int.MaxValue, ErrorMessage = "Không được nhập giá trị âm")]
        public int? SoPhongLuuTruMuiHoi { get; set; } = 0;

        [Display(Name = "Số lượng phòng sao chép, xử lý hơi", Prompt = "Số lượng phòng sao chép, xử lý hơi")]
        [Range(0, int.MaxValue, ErrorMessage = "Không được nhập giá trị âm")]
        public int? SoPhongXuLyHoi { get; set; } = 0;

        [Display(Name = "Lãnh đạo đơn vị", Prompt = "Lãnh đạo đơn vị")]
        public string LanhDaoDonVi { get; set; }

        [Display(Name = "Lãnh đạo phụ trách trực tiếp", Prompt = "Lãnh đạo phụ trách trực tiếp")]
        public string LanhDaoPhuTrachTT { get; set; }

        [Display(Name = "Đội trưởng (Tổ trưởng)", Prompt = "Đội trưởng (Tổ trưởng)")]
        public string DoiTruong { get; set; }

        [Display(Name = "Đội phó (Tổ phó)", Prompt = "Đội phó (Tổ phó)")]
        public string DoiPho { get; set; }

        [Display(Name = "SĐT lãnh đạo đơn vị", Prompt = "SĐT lãnh đạo đơn vị")]
        public string Sdt_LanhDaoDonVi { get; set; }

        [Display(Name = "SĐT lãnh đạo phụ trách trực tiếp", Prompt = "SĐT lãnh đạo phụ trách trực tiếp")]
        public string Sdt_LanhDaoPhuTrachTT { get; set; }

        [Display(Name = "SĐT đội trưởng (Tổ trưởng)", Prompt = "SĐT đội trưởng (Tổ trưởng)")]
        public string Sdt_DoiTruong { get; set; }

        [Display(Name = "SĐT đội phó (Tổ phó)", Prompt = "SĐT đội phó (Tổ phó)")]
        public string Sdt_DoiPho { get; set; }

        [Display(Name = "Năm thành lập", Prompt = "Năm thành lập")]
        public int? NamThanhLap { get; set; }

        [Display(Name = "Đơn vị trực thuộc", Prompt = "Đơn vị trực thuộc")]
        public string DonViTrucThuoc { get; set; }

        [Display(Name = "Cầu tập", Prompt = "Cầu tập")]
        public int CauTap { get; set; }

        [Display(Name = "Sân tập", Prompt = "Sân tập")]
        public int SanTap { get; set; }

        [Display(Name = "Nhà GB", Prompt = "Nhà GB")]
        public int NhaGb { get; set; }

        [Display(Name = "Phương tiện", Prompt = "Phương tiện")]
        public int PhuongTien { get; set; }


        [NotMapped]
        public int SoCSVC;
        [NotMapped]
        public int Chet;
        [NotMapped]
        public int ThaiLoai;
    }
}