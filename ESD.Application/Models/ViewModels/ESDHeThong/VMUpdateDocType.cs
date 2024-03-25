using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMUpdateDocType
    {
        public int ID { get; set; }

        [Display(Name = "Tên loại tài liệu", Prompt = "Tên loại tài liệu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mã loại tài liệu", Prompt = "Mã loại tài liệu")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Loại khung biên mục", Prompt = "Loại khung biên mục")]
        public int? Type { get; set; }

        public int Minlenght { get; set; }
        public int Maxlenght { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }


        public IEnumerable<VMDocTypeField> DocTypeFields { get; set; } //For detail, update, create
    }
}