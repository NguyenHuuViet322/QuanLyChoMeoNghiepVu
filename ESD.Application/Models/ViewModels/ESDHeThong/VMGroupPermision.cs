using AutoMapper.Configuration.Annotations;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMGroupPermision
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name ="Tên nhóm quyền" , Prompt = "Tên nhóm quyền")]
        public string Name { get; set; }

        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        public string Description { get; set; }

        public int Status { get; set; }

        [Display(Name = "Nhóm quyền cơ sở", Prompt = "Nhóm quyền cơ sở")]
        public bool IsBase { get; set; }

        public IEnumerable<PermissionForGroupPer> Permissions { get; set; } //For detail, update, create
        public IEnumerable<Module> Modules { get; set; }
    }

    public class PermissionForGroupPer
    {
        public int IDPermission { get; set; }
        public string PermissionName { get; set; }
        public int IDModule { get; set; }
        public string ModuleName { get; set; }
        public int IDModuleChild { get; set; }
        public string ModuleChildName { get; set; }
        public bool IsCheck { get; set; }
        public int IsChecked { get; set; } //For update, create: IsCheck kiểu bool chưa map đc (1: true, 0:fale)
        public int? IDPermissionGroupPer { get; set; } //For edit case , when isCheck = true, this field has value
        public string GroupPermissionName { get; set; }
        public int Type { get; set; }
    }
    public class PermissionGroupCondition
    {
        public PermissionGroupCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
