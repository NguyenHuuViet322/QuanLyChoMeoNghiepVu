using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMModule
    {
       
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        [DisplayName("Menu cha")]
        public int ParentId { get; set; }

        [DisplayName("Menu cha")]
        public string ParentName { get; set; }

        [Display(Name = "Tên Menu", Prompt = "Tên Menu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }
        [MaxLength(250)]

        [Display(Name = "Đường dẫn", Prompt = "Đường dẫn")]
        public string Url { get; set; }

        [Display(Name = "Biểu tượng", Prompt = "Biểu tượng")]
        [MaxLength(30)]
        public string Icon { get; set; }
        [Display(Name = "Thứ tự", Prompt = "Thứ tự")]
        public int SortOrder { get; set; }
        public string ParentPath { get; set; }
        public int Status { get; set; } = 1;

        [Display(Name = "Mã phân quyền")]
        public int Code { get; set; }
    }

    public class ModuleCondition
    {
        //public int SearchID { get; set; }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public ModuleCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }

  
}
