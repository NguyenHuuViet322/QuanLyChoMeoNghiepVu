using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMEditUser : Auditable
    {
        public int ID { get; set; }

        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [MaxLength(50, ErrorMessage = "Họ và tên không được quá 50 ký tự")]
        [DisplayName("Họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số CMND/Hộ chiếu không được để trống")]
        [MaxLength(20, ErrorMessage = "Số CMND/Hộ chiếu không được quá 20 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số CMND/Hộ chiếu không đúng định dạng")]
        [DisplayName("Số CMND/Hộ chiếu")]
        public string IdentityNumber { get; set; }

        public string OldIdentityNumber { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 20 ký tự")]
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
        //public IEnumerable<string> IDRoleStrs { get; set; }

        //public int IDRole { get; set; }

        [DisplayName("Nhóm quyền")]
        public IEnumerable<string> IDGroupPerStrs { get; set; }

        public int IDGroupPer { get; set; }

        [DisplayName("Nhóm người dùng")]
        public IEnumerable<string> IDTeamStrs { get; set; }

        public int IDTeam { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; }

        public string Description { get; set; }

        public List<VMEditUser> ListUserRole { get; set; }

        [DisplayName("Quản trị viên")]
        public bool HasOrganPermission { get; set; }
    }

    public class VMEditAdminUser : Auditable
    {
        public int ID { get; set; }

        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        public string Password { get; set; }

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
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không được quá 20 ký tự")]
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

        public int IDAgency { get; set; }

        [DisplayName("Chức vụ")]
        public int? IDPosition { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; }

        public string Description { get; set; }
    }
}