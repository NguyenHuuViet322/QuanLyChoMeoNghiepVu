using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreateTeam
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "Tên nhóm người dùng không được để trống")]
        [MaxLength(50)]
        [DisplayName("Tên nhóm người dùng")]
        public string Name { get; set; }

        [MaxLength(300)]
        [DisplayName("Mô tả")]
        public string Description { get; set; }

        public int Status { get; set; } = 1;

        [DisplayName("Người dùng")]
        public IEnumerable<string> IDUserStrs { get; set; }

        //[DisplayName("Vai trò")]
        //public IEnumerable<string> IDRoleStrs { get; set; }

        [DisplayName("Nhóm quyền")]
        public IEnumerable<string> IDGroupPerStrs { get; set; }
    }
}