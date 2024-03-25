using System;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMCategoryField
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public int IDCategory { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public int IDCategoryTypeField { get; set; }

        public string StringVal { get; set; }

        public DateTime? DateTimeVal { get; set; }

        public long? IntVal { get; set; }

        public float? FloatVal { get; set; }

        public string DisplayVal { get; set; } //Du lieu hien thi
        public int IDOrgan { get; set; }
    }
}