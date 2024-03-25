using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.ComponentModel;

namespace ESD.Application.Models.ViewModels
{
    public class VMTeam : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Tên nhóm người dùng")]
        public string Name { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; } = 1;

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        //[DisplayName("Vai trò")]
        //public IEnumerable<Role> Roles { get; set; }

        [DisplayName("Nhóm quyền")]
        public IEnumerable<GroupPermission> GroupPers { get; set; }

        [DisplayName("Người dùng")]
        public IEnumerable<User> Users { get; set; }
    }

    public class TeamCondition
    {
        public TeamCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public string Agencies { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}