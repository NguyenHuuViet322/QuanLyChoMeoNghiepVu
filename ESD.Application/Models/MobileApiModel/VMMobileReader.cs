using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ESD.Application.Models.MobileApiModel
{
    public class VMMobileReader
    {
        [Required(ErrorMessage = "ID Reader không được để trống")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [RegularExpression(@"^[0-9a-zA-Z\._]+$", ErrorMessage = "Chỉ sử dụng các chữ cái (a-z), số (0-9) và dấu chấm(.) và dấu gạch dưới (_)")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản không được quá 50 ký tự")]
        [Display(Name = "Tên tài khoản", Prompt = "Tên tài khoản")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [MaxLength(50, ErrorMessage = "Họ và tên không được quá 50 ký tự")]
        [Display(Name = "Họ và tên", Prompt = "Họ và tên")]
        public string Name { get; set; }

        [MaxLength(12, ErrorMessage = "Số CMND/CCCD không được quá 12 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số CMND/CCCD không đúng định dạng")]
        [Display(Name = "Số CMND/CCCD", Prompt = "Số CMND/CCCD")]
        public string IdentityNumber { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(50, ErrorMessage = "Email không được quá 100 ký tự")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email chưa đúng định dạng. Vui lòng nhập email đúng định dạng như (@gmail.com, @abc.vn...)")]
        [Display(Name = "Email", Prompt = "Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(11, ErrorMessage = "Số điện thoại không được quá 11 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [Display(Name = "Số điện thoại", Prompt = "Số điện thoại")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessage = "Cơ quan công tác không được quá 250 ký tự")]
        [Display(Name = "Cơ quan công tác", Prompt = "Cơ quan công tác")]
        public string Birthplace { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 250 ký tự")]
        [Display(Name = "Địa chỉ liên hệ", Prompt = "Địa chỉ liên hệ")]
        public string Address { get; set; }

        //[JsonIgnore]
        //[Display(Name = "Trạng thái", Prompt = "Trạng thái hệ thống")]
        //public int Status { get; set; } = 1;

        //[JsonIgnore]
        //public int IDOrgan { get; set; }

        [JsonIgnore]
        [DisplayName("Avatar")]
        public long? Avatar { get; set; }

        //[JsonIgnore]
        //[NotMapped]
        //public string SrcAvatar { get; set; }

        public FileBinaryInfo File { get; set; }
    }

    public class MobileReaderCondition
    {
        public MobileReaderCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int IdStatus { get; set; } = -1;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class VMMobileIndexReader
    {
        public MobileReaderCondition Condition { get; set; }
        public PaginatedList<VMMobileReader> VmReaders { get; set; }
    }
}
