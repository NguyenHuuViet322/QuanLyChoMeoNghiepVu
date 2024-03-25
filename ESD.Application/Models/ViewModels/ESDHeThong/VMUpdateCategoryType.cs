using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMUpdateCategoryType
    {
        public int ID { get; set; }

        [Display(Name = "Tên loại danh mục", Prompt = "Tên loại danh mục")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mã loại danh mục", Prompt = "Mã loại danh mục")]
        public string Code { get; set; }
        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Description { get; set; }
        public int IsConfig { get; set; }

        [Display(Name = "Cấp trên", Prompt = "Cấp trên")]
        public int? ParentId { get; set; }
        public string ParentPath { get; set; }
        public IEnumerable<VMUpdateCategoryTypeField> CategoryTypeFields { get; set; } //For detail, update, create
    }
}