using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ESD.Application.Models.ViewModels
{
    public class VMOrganConfig : Auditable
    {
        [Required]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        public int IDOrgan { get; set; } = 0;

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Tên tham số", Prompt = "Tên tham số")]
        public string Name { get; set; }


        [Required(ErrorMessage = "{0} không được để trống")]
        [Column(TypeName = "varchar(64)")]
        [MaxLength(64, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }

        [MaxLength(255, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Giá trị chuỗi", Prompt = "Giá trị chuỗi")]
        public string StringVal { get; set; }

        [Display(Name = "Giá trị ngày tháng", Prompt = "Giá trị ngày tháng")]
        public DateTime? DateTimeVal { get; set; } //dd/MM/yyyy

        [Display(Name = "Giá trị nguyên", Prompt = "Giá trị nguyên")]
        public long? IntVal { get; set; }

        [Display(Name = "Giá trị thực", Prompt = "Giá trị thực")]
        public float? FloatVal { get; set; }

        [MaxLength(300, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        public string Description { get; set; }

        public int Status { get; set; } = 1;
    }
    public class OrganConfigCondition
    {
        public OrganConfigCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
