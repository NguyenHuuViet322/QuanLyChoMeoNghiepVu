using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMEditTeam
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        [DisplayName("Tên nhóm người dùng")]
        public string Name { get; set; }

        [MaxLength(300)]
        [DisplayName("Mô tả")]
        public string Description { get; set; }

        public int Status { get; set; } = 1;

        public int IDUser { get; set; }

        [DisplayName("Người dùng")]
        public IEnumerable<string> IDUserStrs { get; set; }

        public int IDRole { get; set; }

        //[DisplayName("Vai trò")]
        //public IEnumerable<string> IDRoleStrs { get; set; }

        [DisplayName("Nhóm quyền")]
        public IEnumerable<string> IDGroupPerStrs { get; set; }
    }
}