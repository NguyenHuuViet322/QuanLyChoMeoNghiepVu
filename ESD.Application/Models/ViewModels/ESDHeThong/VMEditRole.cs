using ESD.Domain.Models.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMEditRole : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Tên vai trò")]
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [MaxLength(250)]
        public string Name { get; set; }

        public int Status { get; set; } = 1;

        [DisplayName("Mô tả")]
        [MaxLength(500)]
        public string Description { get; set; }

        [DisplayName("Nhóm quyền")]
        public List<string> IDGroupPermissionStrs { get; set; }
    }
}
