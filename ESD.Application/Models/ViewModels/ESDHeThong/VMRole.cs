using ESD.Domain.Models.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMRole : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Vai trò")]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Nhóm quyền")]
        public string GroupPermissionName { get; set; }

        public int Status { get; set; } = 1;

        public int IDGroupPermission { get; set; }

        public List<VMRole> ListGroupPermission { get; set; }
    }

    public class RoleCondition
    {
        public RoleCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
