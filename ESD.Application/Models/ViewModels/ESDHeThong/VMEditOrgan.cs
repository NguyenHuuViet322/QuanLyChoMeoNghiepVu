using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMEditOrgan : Auditable
    {
        public int ID { get; set; }
        public int IDChannel { get; set; } = 0;
        [Required(ErrorMessage = "Mã cơ quan không được để trống")]
        [MaxLength(13, ErrorMessage = "Mã cơ quan không vượt quá 13 ký tự")]
        [Display(Name = "Mã cơ quan", Prompt = "Mã cơ quan")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tên cơ quan không được để trống")]
        [MaxLength(250, ErrorMessage = "Tên cơ quan không vượt quá 250 ký tự")]
        [Display(Name = "Tên cơ quan", Prompt = "Tên cơ quan")]
        public string Name { get; set; }

        public int Status { get; set; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không vượt quá 250 ký tự")]
        [Display(Name = "Địa chỉ", Prompt = "Địa chỉ")]
        public string Address { get; set; }

        [MaxLength(300, ErrorMessage = "Mô tả không vượt quá 300 ký tự")]
        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        public string Description { get; set; }

        [MaxLength(20, ErrorMessage = "Fax không vượt quá 20 ký tự")]
        [Display(Name = "Fax", Prompt = "Fax")]
        public string Fax { get; set; }

        //[Display(Name = "Cơ quan nộp lưu", Prompt = "Cơ quan nộp lưu")]
        //public bool IsArchive { get; set; }

        //public string IsArchiveStr { get; set; }

        //public string ParentIdStr { get; set; }

        //public int ParentId { get; set; }

        //[Display(Name = "Cơ quan cha", Prompt = "Cơ quan cha")]
        //public string ParentName { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        [RegularExpression(@"(^[0-9]{10}\b)|(^[0-9]{11}\b)", ErrorMessage = "Số điện thoại không đúng định dạng (10 hoặc 11 số)")]
        [Display(Name = "Số điện thoại", Prompt = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email", Prompt = "Email")]
        public string Email { get; set; }

        public IEnumerable<Organ> Parents { get; set; }
    }
}
