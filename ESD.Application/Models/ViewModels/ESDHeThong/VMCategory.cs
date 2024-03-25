using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMCategory
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(50, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public int IdCategoryType { get; set; }

        public int Status { get; set; }

        public string CodeType { get; set; }
        public int IDOrgan { get; set; }
        public int? ParentId { get; set; }
        public string ParentPath { get; set; }
        public List<VMCategoryField> VMCategoryFields { get; set; }
    }
}