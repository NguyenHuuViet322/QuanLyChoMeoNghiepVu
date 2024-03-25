using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreateRole : Auditable
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        public int IDChannel { get; set; } = 0;

        [DisplayName("Tên vai trò")]
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [MaxLength(250)]
        public string Name { get; set; }

        public int Status { get; set; } = 1;

        [DisplayName("Mô tả")]
        [MaxLength(500, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Description { get; set; }

        [DisplayName("Nhóm quyền")]
        public List<string> IDGroupPermissionStrs { get; set; }
    }
}
