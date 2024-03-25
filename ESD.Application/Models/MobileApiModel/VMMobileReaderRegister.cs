using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Models.MobileApiModel
{
    public class VMMobileReaderRegister
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [RegularExpression(@"^[0-9a-zA-Z\._]+$", ErrorMessage = "Chỉ sử dụng các chữ cái (a-z), số (0-9) và dấu chấm(.) và dấu gạch dưới (_)")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản không được quá 50 ký tự")]
        [Display(Name = "Tên tài khoản", Prompt = "Tên tài khoản")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [MaxLength(50, ErrorMessage = "Họ và tên không được quá 50 ký tự")]
        [Display(Name = "Họ và tên", Prompt = "Họ và tên ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        //[MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        //[MaxLength(50,ErrorMessage = "Mật khẩu không được quá 50 ký tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Mật khẩu không chứa khoảng trắng")]
        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu ")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        //[MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        //[MaxLength(50, ErrorMessage = "Mật khẩu không được vượt quá 50 ký tự")]
        //[Compare(nameof(Password), ErrorMessage = "Xác nhận mật khẩu chưa trùng khớp")]
        [RegularExpression(@"^\S*$$", ErrorMessage = "Mật khẩu không chứa khoảng trắng")]
        [Display(Name = "Nhập lại mật khẩu ", Prompt = "Nhập lại mật khẩu ")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "Số CMND/CCCD không được để trống")]
        //[MaxLength(12, ErrorMessage = "Số CMND/Hộ chiếu không được quá 12 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số CMND/CCCD không đúng định dạng")]
        [Display(Name = "Số CMND/CCCD", Prompt = "Số CMND/CCCD")]
        public string IdentityNumber { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(50, ErrorMessage = "Email không được quá 100 ký tự")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email chưa đúng định dạng. Vui lòng nhập email đúng định dạng như (@gmail.com, @abc.vn...)")]
        [Display(Name = "Email ", Prompt = "Email ")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(11, ErrorMessage = "Số điện thoại không được quá 11 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [Display(Name = "Số điện thoại", Prompt = "Số điện thoại")]
        public string Phone { get; set; }

        //[Display(Name = "Ngày sinh", Prompt = "Ngày sinh")]
        //public string Birthday { get; set; } //dd/MM/yyyy

        [MaxLength(250, ErrorMessage = "Cơ quan công tác không được quá 250 ký tự")]
        [Display(Name = "Cơ quan công tác", Prompt = "Cơ quan công tác")]
        public string Birthplace { get; set; }

        [Display(Name = "Địa chỉ liên hệ", Prompt = "Địa chỉ liên hệ")]
        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 250 ký tự")]
        public string Address { get; set; }

        //[Display(Name = "Giới tính", Prompt = "Giới tính")]
        //public int Gender { get; set; } = 0;
    }
}
