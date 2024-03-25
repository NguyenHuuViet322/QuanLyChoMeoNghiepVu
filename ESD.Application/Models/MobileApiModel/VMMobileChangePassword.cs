using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ESD.Application.Models.MobileApiModel
{
    public class VMMobileChangePassword
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DisplayName("Mật khẩu hiện tại")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Mật khẩu không chứa khoảng trắng")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [DisplayName("Mật khẩu mới")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Mật khẩu không chứa khoảng trắng")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống")]
        [DisplayName("Xác nhận mật khẩu mới")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Mật khẩu không chứa khoảng trắng")]
        public string ConfirmPassword { get; set; }
        //[Required(ErrorMessage = "Token không được để trống")]
        //public string Token { get; set; }
    }
}
