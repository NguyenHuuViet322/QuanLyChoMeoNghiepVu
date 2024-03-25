using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ESD.Domain.Models.Abstractions;

namespace ESD.Application.Models.ViewModels
{
    public class VMAccount
    {
        public int userID { get; set; } = 0;
        public string AccountName { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DisplayName("Mật khẩu hiện tại")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [MaxLength(255, ErrorMessage = "Mật khẩu có tối đa 255 ký tự")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [DisplayName("Mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [MaxLength(255, ErrorMessage = "Mật khẩu có tối đa 255 ký tự")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống")]
        [DisplayName("Xác nhận mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Compare(nameof(Password), ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        [MaxLength(255, ErrorMessage = "Mật khẩu có tối đa 255 ký tự")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
    }
}
