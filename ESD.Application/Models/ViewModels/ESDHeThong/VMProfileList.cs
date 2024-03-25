using ESD.Domain.Models.Abstractions;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMProfileList: Auditable 
    { 
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(30, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mã mục lục", Prompt = "Mã mục lục")]
        public string Code { get; set; }

        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tên mục lục", Prompt = "Tên mục lục")]
        public string Name { get; set; }

        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tên kho", Prompt = "Tên kho")]
        [Range(1, Int32.MaxValue,ErrorMessage ="Phải chọn kho")]
        public int IDStorage { get; set; }

        [Display(Name = "Tên kho", Prompt = "Tên kho")]
        public string StorageName { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Phông", Prompt = "Phông")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Phải chọn phông")]
        public int IDProfileTemplate { get; set; }

        [Display(Name = "Tên phông", Prompt = "Mã phông")]
        public string FondName { get; set; }

        public int Status { get; set; } = 1;
    }

    public class ProfileListCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public ProfileListCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public string ProfileTemplates { get; set; }
        public string Storages { get; set; }
        public List<string> listStoragetr
        {
            get
            {
                if (Storages.IsNotEmpty())                
                    return Storages.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public List<string> listProfileTemplatestr
        {
            get
            {
                if (ProfileTemplates.IsNotEmpty())
                    return ProfileTemplates.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }

    }
}
