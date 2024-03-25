using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMCategoryTypeField
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        public int IDCategoryType { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }
        [MaxLength(30, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }
        [MaxLength(30, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Format { get; set; }  //for date field, number field
        public int? IDCategoryTypeRelated { get; set; } // for category field
        [Required(ErrorMessage = "{0} không được để trống")] 
        public int Priority { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        public bool IsRequire { get; set; } = false;
        [Required(ErrorMessage = "{0} không được để trống")]
        public int Status { get; set; } = 1;
        public int InputType { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        public bool IsShowGrid { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        public bool IsSearchGrid { get; set; }
        public bool IsUsed { get; set; }

        public int Minlenght { get; set; }
        public int Maxlenght { get; set; }
        public long? MaxValue { get; set; }
        public long? MinValue { get; set; }
        public bool IsReadonly { get; set; }
        public int DefaultValueType { get; set; }
        public int IDOrgan { get; set; }
        public int IDAgency { get; set; }
    }
}