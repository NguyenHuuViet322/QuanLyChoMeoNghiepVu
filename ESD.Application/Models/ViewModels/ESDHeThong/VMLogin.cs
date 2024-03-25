using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMLogin
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [Display(Name = "Tài khoản")]
        [MaxLength(50, ErrorMessage = "Tài khoản có tối đa 50 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu có tối thiểu 6 ký tự")]
        [MaxLength(255, ErrorMessage = "Mật khẩu có tối đa 255 ký tự")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ")]
        public bool RememberMe { get; set; }

        public string RequestPath { get; set; }
        public bool IsMobile { get; set; }
    }
}
