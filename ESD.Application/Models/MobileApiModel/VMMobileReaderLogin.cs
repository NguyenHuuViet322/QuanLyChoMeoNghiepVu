using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.MobileApiModel
{
    public class VMMobileReaderLogin
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
