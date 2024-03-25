using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ESD.Application.Models.ViewModels
{
    public class VMDocType
    {
        public int ID { get; set; }
        public int IDOrgan { get; set; }

        [DisplayName("Tên khung biên mục")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }

        [DisplayName("Mã khung biên mục")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }

        public int Status { get; set; }

        [DisplayName("Loại khung biên mục")]
        public int? Type { get; set; }

       public IEnumerable<VMDocTypeField> DocTypeFields { get; set; }
        //public Dictionary<string, string> DictDocTypeCodes { get; set; }
        public Dictionary<int, string> DictCategoryTypes { get; set; }
        public Dictionary<int, string> DictInputTypes { get; set; }
        public bool IsUsed { get;  set; }
        public bool IsBase { get;  set; }
        //public int IsConfig { get;  set; }

        public int Minlenght { get; set; }
        public int Maxlenght { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public Dictionary<int, string> DictType { get; set; }
    }
    public class DocTypeCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public DocTypeCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}