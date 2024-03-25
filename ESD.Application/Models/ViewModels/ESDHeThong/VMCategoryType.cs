using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ESD.Application.Models.ViewModels
{
    public class VMCategoryType
    {
        public int ID { get; set; }

        [Display(Name = "Tên loại danh mục", Prompt = "Tên loại danh mục")]
        public string Name { get; set; }

        [Display(Name = "Mã loại danh mục", Prompt = "Mã loại danh mục")]
        public string Code { get; set; }

        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        public string Description { get; set; }
        public int Status { get; set; }

        public IEnumerable<VMUpdateCategoryTypeField> CategoryTypeFields { get; set; }
        //public Dictionary<string, string> DictCategoryTypeCodes { get; set; }
        public Dictionary<int, string> DictCategoryTypes { get; set; }
        public Dictionary<int, string> DictInputTypes { get; set; }
        public bool IsUsed { get; set; }
        public int IsConfig { get; set; }

        [Display(Name = "Cấp trên", Prompt = "Cấp trên")]
        public int? ParentId { get; set; }
        public int IDOrgan { get; set; } = 0;

        public string ParentPath { get; set; }
        public Dictionary<int, string> DictDefaultValueTypes { get; set; }
        public Dictionary<int, string> DictParents { get; set; }
    }
    public class CategoryTypeCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public CategoryTypeCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}