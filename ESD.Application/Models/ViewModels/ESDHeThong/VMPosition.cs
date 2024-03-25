using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMPosition
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Cấp trên")]
        public int Parent { get; set; }

        [Display(Name = "Tên chức vụ", Prompt = "Tên chức vụ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }

        [Display(Name = "Mã chức vụ", Prompt = "Mã chức vụ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }

        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        [DisplayName("Mô tả")]
        public string Description { get; set; }

        public int Status { get; set; }
        public string ParentPath { get; set; }
        public string ParentName { get; set; }
    }

    public class PositionCondition
    {
        public PositionCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
