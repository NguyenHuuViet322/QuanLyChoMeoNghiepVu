using ESD.Domain.Models.Abstractions;
using ESD.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMUserProfile : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [RegularExpression(@"^[0-9a-zA-Z\.]+$", ErrorMessage = "Chỉ sử dụng các chữ cái (a-z), số (0-9) và dấu chấm(.)")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản không được quá 50 ký tự")]
        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [MaxLength(50, ErrorMessage = "Họ và tên không được quá 50 ký tự")]
        [DisplayName("Họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số CMND/Hộ chiếu không được để trống")]
        [MaxLength(20, ErrorMessage = "Số CMND/Hộ chiếu không được quá 20 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số CMND/Hộ chiếu không đúng định dạng")]
        [DisplayName("Số CMND/Hộ chiếu")]
        public string IdentityNumber { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email chưa đúng định dạng. Vui lòng nhập email đúng định dạng như (@gmail.com, @abc.vn...)")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 250 ký tự")]
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Avatar")]
        public long? Avatar { get; set; }

        public string SrcAvatar { get; set; }

        public string PositionName { get; set; }

        public string OrganName { get; set; }

        public string AgencyName { get; set; }

        public IFormFile File { get; set; }

        [MaxLength(250)]
        public string FileName { get; set; }

        [MaxLength(1000)]
        public string PhysicalPath { get; set; }

        public int? FileType { get; set; }

        public decimal? Size { get; set; }
    }
}