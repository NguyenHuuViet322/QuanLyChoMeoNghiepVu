using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ESD.Domain.Models.Abstractions;

namespace ESD.Domain.Models.DAS
{
    /// <summary>
    /// Phông
    /// https://m.thuvienphapluat.vn/van-ban/linh-vuc-khac/Thong-tu-02-2019-TT-BNV-tieu-chuan-du-lieu-thong-tin-dau-vao-406241.aspx
    /// </summary>
    public class ProfileTemplate : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int IDOrgan { get; set; } //Bảng cơ quan

        [Description("Kho")]
        public int IDStorage { get; set; } =0;//ID kho , lấy theo danh mục động

        [Description("Mã cơ quan")]
        [MaxLength(13)]
        public string Identifier { get; set; }

        [Description("Mã phông")]
        [MaxLength(13)]
        public string FondCode { get; set; }

        [Description("Tên phông")]
        [Required]
        [MaxLength(200)]
        public string FondName { get; set; }

        [Description("Lịch sử hình thành phông")]
        public string FondHistory { get; set; }

        [Description("Thời gian tài liệu")]
        [MaxLength(30)]
        public string ArchivesTime { get; set; }

        [Description("Tổng số tài liệu giấy")]
        [MaxLength(10)]
        public long PaperTotal { get; set; }

        [Description("Số lượng tài liệu giấy đã số hóa")]
        [MaxLength(10)]
        public long PaperDigital { get; set; }

        [Description("Các nhóm tài liệu chủ yếu")]
        [MaxLength(300)]
        public string KeyGroups { get; set; } //	        Các nhóm tài liệu chủ yếu

        [Description("Các loại hình tài liệu khác")]
        [MaxLength(300)]
        public string OtherTypes { get; set; } //	        Các loại hình tài liệu khác

        [Description("Ngôn ngữ")]
        public int Language { get; set; }

        [Description("Công cụ tra cứu")]
        [MaxLength(50)]
        public string LookupTools { get; set; }

        [Description("Số lượng trang tài liệu đã lập bản sao bảo hiểm")]
        [MaxLength(10)]
        public long CopyNumber { get; set; }

        [Description("Ghi chú")]
        [MaxLength(1000)]
        public string Description { get; set; } // ghi chú
        
        [MaxLength(30)]
        [Description("Ký hiệu thông tin")]
        public string InforSign { get; set; }

        [Description("Từ khóa")]
        [MaxLength(100)]
        public string Keyword { get; set; }

        [Description("Chế độ sử dụng")]
        [MaxLength(20)]
        public string Mode { get; set; }

        [Description("Loại")]
        public int Type { get; set; } = 0; //0: Phông mở - 1: phông đóng

        [Description("Trạng thái")]
        public int Status { get; set; } = 1;
        [Description("Mã đơn vị")]
        public int IDAgency { get; set; } = 0;
    }
}
