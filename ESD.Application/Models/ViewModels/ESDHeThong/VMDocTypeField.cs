using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMDocTypeField
    {
        public int ID { get; set; }

        [DisplayName("Tên trường")]
        [Required(ErrorMessage = "Tên trường không được để trống")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }

        [DisplayName("Mã trường")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; } 
        public string Format { get; set; }  //for date field, number field
        [Required(ErrorMessage = "Thứ tự không được để trống")]
        [Display(Name = "Thứ tự", Prompt = "Thứ tự")]
        [Range(0, 999, ErrorMessage = "{0} giá trị tối đa là {2}")]
        public int? Priority { get; set; }
        public int IsRequire { get; set; }
        public int IDDocType { get; set; }

        public int? IDCategoryTypeRelated { get; set; }
        public int InputType { get; set; }
        public int IsShowGrid { get; set; }
        public int IsSearchGrid { get; set; }
        public int? Index { get; set; }
        public bool IsDelete { get; set; }
        public bool IsUpdate { get; set; }

        public bool IsDetail { get; set; }
        public IEnumerable<SelectListItem> DlInputTypes { get; set; }
        public IEnumerable<SelectListItem> DlCategoryTypes { get; set; }
        public bool IsUsed { get; internal set; }

        public int Minlenght { get; set; }
        public int Maxlenght { get; set; }
        public int? MaxValue { get; set; }
        public int? MinValue { get; set; }

        public int IsBase { get; set; }

        [NotMapped]
        public int TypeOfDocType { get;  set; }
    }
}