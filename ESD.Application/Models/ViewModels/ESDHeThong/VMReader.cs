using ESD.Domain.Models.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ESD.Application.Models.ViewModels
{
    public class VMReader : Auditable
    {
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

        //[Required(ErrorMessage = "Số CMND/CCCD không được để trống")]
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

        [Display(Name = "Ngày sinh", Prompt = "Ngày sinh")]
        public string Birthday { get; set; } //dd/MM/yyyy

        [MaxLength(250, ErrorMessage = "Cơ quan công tác không được quá 250 ký tự")]
        [Display(Name = "Cơ quan công tác", Prompt = "Cơ quan công tác")]
        public string Birthplace { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 250 ký tự")]
        [Display(Name = "Địa chỉ liên hệ", Prompt = "Địa chỉ liên hệ")]
        public string Address { get; set; }

        [Display(Name = "Giới tính", Prompt = "Giới tính")]
        public int Gender { get; set; } = 0;
        [Display(Name = "Trạng thái", Prompt = "Trạng thái hệ thống")]
        public int Status { get; set; } = 1;
        public int IDOrgan { get; set; }
        [Display(Name = "Trạng thái", Prompt = "Trạng thái")]
        public int StatusByOrgan { get; set; } = 1;
        [DisplayName("Avatar")]
        public long? Avatar { get; set; }

        public string SrcAvatar { get; set; }
        public IFormFile File { get; set; }

    }

    public class ReaderCondition
    {
        public ReaderCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int IDStatus { get; set; } = -1;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

}
