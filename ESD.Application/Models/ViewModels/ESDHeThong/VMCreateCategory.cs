using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreateCategory
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public int IdCategoryType { get; set; }
        public string CodeType { get; set; }
        public int? ParentId { get; set; }
        public string ParentPath { get; set; }
        public List<VMCategoryTypeField> VMCategoryTypeFields { get; set; }
        public VMCategoryType VMCategoryType { get; set; }
        public IEnumerable<VMCategoryType> VMCategoryTypes { get;  set; }
        public List<VMCategoryField> CategoryFields { get;  set; }
        public List<VMCategory> CategoriesSelected { get;  set; }
        public int IDOrgan { get; set; }
    }
}