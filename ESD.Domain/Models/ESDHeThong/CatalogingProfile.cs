using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    /// <summary>
    /// Hồ sơ
    /// https://m.thuvienphapluat.vn/van-ban/linh-vuc-khac/Thong-tu-02-2019-TT-BNV-tieu-chuan-du-lieu-thong-tin-dau-vao-406241.aspx
    /// </summary>
    public class CatalogingProfile : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDPlanProfile { get; set; }

        public int IDChannel { get; set; } = 0;

        public int IDPlan { get; set; } 

        [Description("Mã hồ sơ")]
        [MaxLength(30)]
        public string FileCode { get; set; }

        public int IDStorage { get; set; } //ID kho lấy từ danh mục động

        public int IDCodeBox { get; set; } // ID hộp số lấy từ danh mục động 

        public int IDProfileList { get; set; } // ID mục lục 

        public int IDSecurityLevel { get; set; } // ID cấp độ bảo mật 

        public int IDProfileTemplate { get; set; }  // ID bảng phông

        //public int IDAgency { get; set; } //Dp

        [Description("Mã cơ quan lưu trữ lịch sử")]
        [MaxLength(13)]
        public string Identifier { get; set; } //Mã cơ quan lưu trữ lịch sử
        
        [Description("Mục lục số hoặc năm hình thành hồ sơ")]
        [MaxLength(4)]
        public int FileCatalog { get; set; } //Mục lục số hoặc năm hình thành hồ sơ
        
        [Description("Số và kí hiệu hồ sơ")]
        [MaxLength(20)]
        public string FileNotation { get; set; }

        [Description("Tiêu đề hồ sơ")]
        [MaxLength(1000)]
        public string Title { get; set; } 

        [Required]
        public int IDExpiryDate { get; set; } //Thời hạn bảo quản 

        [Description("Chế độ sử dụng")]
        [MaxLength(30)]
        public string Rights { get; set; } //Chế độ sử dụng

        [Description("Ngôn ngữ")]
        public string Language { get; set; } //Cách nhau dấu phẩy

        [Description("Thời gian bắt đầu")]
        public DateTime? StartDate { get; set; } //DD/MM/YYYY

        [Description("Thời gian kết thúc")]
        public DateTime? EndDate { get; set; } //DD/MM/YYYY

        [Description("Tổng số văn bản trong hồ sơ")]
        [MaxLength(10)]
        public int TotalDoc { get; set; }

        [Description("Chú giải")]
        [MaxLength(2000)]
        public string Description { get; set; }

        [Description("Ký hiệu thông tin")]
        [MaxLength(30)]
        public string InforSign { get; set; } // ký hiệu thông tin

        [Description("Từ khóa")]
        [MaxLength(100)]
        public string Keyword { get; set; }

        [Description("Số lượng tờ")]
        [MaxLength(10)]
        public int Maintenance { get; set; } //Số lượng tờ 

        [Description("Số lượng trang")]
        [MaxLength(10)]
        public int PageNumber { get; set; } //Số lượng trang

        [Description("Tình trạng vật lý")]
        [MaxLength(20)]
        public string Format { get; set; } //Tình trạng vật lý 

        public int Status { get; set; } = 1;

        public int IDAgency { get; set; } //Đơn vị
        public int IDOrgan { get; set; } //Cơ quan

        public string ReasonToReject { get; set; }
        public int ApprovedBy { get; set; } //người duyệt/ từ chối
        public DateTime? ApprovedDate { get; set; } //Ngày duyệt

        public int Type { get; set; } //Loại
        public int IDProfileCategory { get; set; } //Phan loại

        //ForDestruction
        public int InUsing { get; set; } = 1; //EnumCataloging.InUse
        public int IDDestruction { get; set; }

        #region Xếp hộp cặp hồ sơ

        public int IDShelve { get; set; } //Giá/kệ
        public int IDBox { get; set; } //Hộp/cặp
        public bool IsStoraged { get; set; } //Đã được sếp cặp
        public bool IsPublic { get; set; } = true; //hồ sơ public?

        #endregion
    }
}
