using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreateUser
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [RegularExpression(@"^[0-9a-zA-Z\.]+$", ErrorMessage = "Chỉ sử dụng các chữ cái (a-z), số (0-9) và dấu chấm(.)")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản không được quá 50 ký tự")]
        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [MaxLength(250)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [Compare(nameof(Password), ErrorMessage = "Xác nhận mật khẩu chưa chính xác")]
        [MaxLength(250)]
        [DisplayName("Nhập lại mật khẩu")]
        public string ConfirmPassword { get; set; }

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
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 250 ký tự")]
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        public string StartDateStr { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        public string EndDateStr { get; set; }

        [DisplayName("Cơ quan")]
        [Required(ErrorMessage = "Cơ quan không được để trống")]
        public int IDOrgan { get; set; }

        [DisplayName("Đơn vị")]
        [Required(ErrorMessage = "Đơn vị không được để trống")]
        public int IDAgency { get; set; }

        [DisplayName("Chức vụ")]
        public int? IDPosition { get; set; }

        //[DisplayName("Vai trò")]
        //public List<string> IDRoleStrs { get; set; }

        //public int IDRole { get; set; }

        [DisplayName("Nhóm quyền")]
        public List<string> IDGroupPerStrs { get; set; }

        public int IDGroupPer { get; set; }

        [DisplayName("Nhóm người dùng")]
        public List<string> IDTeamStrs { get; set; }

        public int IDTeam { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; }

        public string Description { get; set; }

        public List<VMCreateUser> ListUserRole { get; set; }

        [DisplayName("Quyền cơ quan")]
        public bool HasOrganPermission { get; set; }
    }

    public class VMCreateAdminUser
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [RegularExpression(@"^[0-9a-zA-Z\.]+$", ErrorMessage = "Chỉ sử dụng các chữ cái (a-z), số (0-9) và dấu chấm(.)")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản không được quá 50 ký tự")]
        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [MaxLength(250)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [Compare(nameof(Password), ErrorMessage = "Xác nhận mật khẩu chưa chính xác")]
        [MaxLength(250)]
        [DisplayName("Nhập lại mật khẩu")]
        public string ConfirmPassword { get; set; }

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
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 250 ký tự")]
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        public string StartDateStr { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        public string EndDateStr { get; set; }

        [DisplayName("Cơ quan")]
        [Required(ErrorMessage = "Cơ quan không được để trống")]
        public int IDOrgan { get; set; }

        [DisplayName("Đơn vị")]
        public int IDAgency { get; set; }

        [DisplayName("Chức vụ")]
        public int? IDPosition { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; }

        public string Description { get; set; }
    }
}