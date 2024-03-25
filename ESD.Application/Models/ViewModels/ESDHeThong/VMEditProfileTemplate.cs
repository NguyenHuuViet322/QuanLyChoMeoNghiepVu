using ESD.Domain.Models.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMEditProfileTemplate
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Description("Cơ quan lưu trữ")]
        [DisplayName("Cơ quan lưu trữ")]
        [Required(ErrorMessage = "Cơ quan lưu trữ không được để trống")]
        public int? IDOrgan { get; set; } //Bảng cơ quan

        [Description("Kho")]
        [DisplayName("Kho")]
        //[Required(ErrorMessage = "Kho không được để trống")]
        public int IDStorage { get; set; } = 0; //ID kho , lấy theo danh mục động

        [Description("Mã cơ quan")]
        [DisplayName("Mã cơ quan")]
        [MaxLength(13, ErrorMessage = "Mã cơ quan không được quá 13 ký tự")]
        public string Identifier { get; set; }

        [Description("Mã phông")]
        [DisplayName("Mã phông")]
        [Required(ErrorMessage = "Mã phông không được để trống")]
        [MaxLength(13, ErrorMessage = "Mã phông không được quá 13 ký tự")]
        public string FondCode { get; set; }

        [Description("Tên phông")]
        [DisplayName("Tên phông")]
        [Required(ErrorMessage = "Tên phông không được để trống")]
        [MaxLength(200, ErrorMessage = "Mã phông không được quá 200 ký tự")]
        public string FondName { get; set; }

        [Description("Lịch sử hình thành phông")]
        [DisplayName("Lịch sử hình thành phông")]
        public string FondHistory { get; set; }

        [Description("Thời gian tài liệu")]
        [DisplayName("Thời gian tài liệu")]
        [MaxLength(30, ErrorMessage = "Thời gian tài liệu không được quá 30 ký tự")]
        public string ArchivesTime { get; set; }

        [Description("Tổng số tài liệu giấy")]
        [DisplayName("Tổng số tài liệu giấy")]
        [Range(0, 9999999999, ErrorMessage = "Tổng số tài liệu giấy không được quá 10 ký tự")]
        public long PaperTotal { get; set; }

        [Description("Số lượng tài liệu giấy đã số hóa")]
        [DisplayName("Số lượng tài liệu giấy đã số hóa")]
        [Range(0, 9999999999, ErrorMessage = "Số lượng tài liệu giấy đã số hóa không được quá 10 ký tự")]
        public long PaperDigital { get; set; }

        [Description("Các nhóm tài liệu chủ yếu")]
        [DisplayName("Các nhóm tài liệu chủ yếu")]
        [MaxLength(300, ErrorMessage = "Các nhóm tài liệu chủ yếu không được quá 300 ký tự")]
        public string KeyGroups { get; set; }

        [Description("Các loại hình tài liệu khác")]
        [DisplayName("Các loại hình tài liệu khác")]
        [MaxLength(300, ErrorMessage = "Các loại hình tài liệu khác không được quá 300 ký tự")]
        public string OtherTypes { get; set; }

        [Description("Ngôn ngữ")]
        [DisplayName("Ngôn ngữ")]
        public int Language { get; set; }

        [Description("Công cụ tra cứu")]
        [DisplayName("Công cụ tra cứu")]
        [MaxLength(50, ErrorMessage = "Công cụ tra cứu không được quá 50 ký tự")]
        public string LookupTools { get; set; }

        [Description("Số lượng trang tài liệu đã lập bản sao bảo hiểm")]
        [DisplayName("Số lượng trang tài liệu đã lập bản sao bảo hiểm")]
        [Range(0, 9999999999, ErrorMessage = "Số lượng trang tài liệu đã lập bản sao bảo hiểm không được quá 10 ký tự")]
        public long CopyNumber { get; set; }

        [Description("Ghi chú")]
        [DisplayName("Ghi chú")]
        [MaxLength(1000, ErrorMessage = "Ghi chú không được quá 1000 ký tự")]
        public string Description { get; set; } // ghi chú

        [Description("Ký hiệu thông tin")]
        [DisplayName("Ký hiệu thông tin")]
        [MaxLength(30, ErrorMessage = "Ký hiệu thông tin không được quá 30 ký tự")]
        public string InforSign { get; set; }

        [Description("Từ khóa")]
        [DisplayName("Từ khóa")]
        [MaxLength(100, ErrorMessage = "Từ khóa không được quá 100 ký tự")]
        public string Keyword { get; set; }

        [Description("Chế độ sử dụng")]
        [DisplayName("Chế độ sử dụng")]
        [MaxLength(20, ErrorMessage = "Chế độ sử dụng không được quá 20 ký tự")]
        public string Mode { get; set; }

        [Description("Loại")]
        [DisplayName("Loại")]
        [Required(ErrorMessage = "Loại phông không được để trống")]
        public int Type { get; set; } = 0; //0: Phông mở - 1: phông đóng

        [Description("Trạng thái")]
        [DisplayName("Trạng thái")]
        public int Status { get; set; }
        public int IDAgency { get; set; }
    }
}
