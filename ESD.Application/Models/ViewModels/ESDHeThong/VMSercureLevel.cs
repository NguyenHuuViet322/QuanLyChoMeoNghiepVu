using ESD.Domain.Models.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;


namespace ESD.Application.Models.ViewModels
{
    public class VMSercureLevel: Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(20, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mã cấp độ bảo mật", Prompt = "Mã cấp độ bảo mật")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Tên cấp độ bảo mật", Prompt = "Tên cấp độ bảo mật")]
        public string Name { get; set; }

        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Description { get; set; }

        public int Status { get; set; } = 1;
    }

    public class SercureLevelCondition
    {
        public SercureLevelCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
