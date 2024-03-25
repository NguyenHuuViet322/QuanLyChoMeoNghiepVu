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
    public class VMUser : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Chức vụ")]
        public int? IDPosition { get; set; } = 0;

        public string PositionName { get; set; }

        [DisplayName("Đơn vị")]
        public int? IDAgency { get; set; } = 0;

        public string AgencyName { get; set; }

        [DisplayName("Cơ quan")]
        public int? IDOrgan { get; set; } = 0;

        public string OrganName { get; set; }

        //[DisplayName("Vai trò")]
        //public IEnumerable<string> IDRoleStrs { get; set; }

        [DisplayName("Vai trò")]
        public IEnumerable<string> IDGroupPerStrs { get; set; }

        [DisplayName("Nhóm người dùng")]
        public IEnumerable<string> IDTeamStrs { get; set; }

        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        [DisplayName("Họ và tên")]
        public string Name { get; set; }

        [DisplayName("Số CMND/Hộ chiếu")]
        public string IdentityNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Avatar")]
        public long? Avatar { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; } = 1;

        [DisplayName("Quyền cơ quan")]
        public bool HasOrganPermission { get; set; }
    }

    public class UserCondition
    {
        public UserCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Statuses { get; set; }
        public string Positions { get; set; }
        public string Agencies { get; set; }
        public string Agencys { get; set; }


        public List<string> ListStatusStr
        {
            get
            {
                if (Statuses.IsNotEmpty())
                    return Statuses.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public List<string> IDOrganStr
        {
            get
            {
                if (Agencies.IsNotEmpty())
                    return Agencies.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public List<string> IDAgencyStr
        {
            get
            {
                if (Agencys.IsNotEmpty())
                    return Agencys.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public List<string> IDPositionStr
        {
            get
            {
                if (Positions.IsNotEmpty())
                    return Positions.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class VMAdminUser : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Chức vụ")]
        public int? IDPosition { get; set; } = 0;

        public string PositionName { get; set; }

        [DisplayName("Đơn vị")]
        public int IDAgency { get; set; } = 0;

        [DisplayName("Cơ quan")]
        public int? IDOrgan { get; set; } = 0;

        public string OrganName { get; set; }

        [DisplayName("Tên tài khoản")]
        public string AccountName { get; set; }

        [DisplayName("Họ và tên")]
        public string Name { get; set; }

        [DisplayName("Số CMND/Hộ chiếu")]
        public string IdentityNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Trạng thái")]
        public int Status { get; set; } = 1;
    }

    public class UserData
    {
        public int IDAgency { get; set; }
        public string AgencyName { get; set; }
        public int IDOrgan { get; set; }
        public string OrganName { get; set; }
        public string ParentPath { get; set; }
        public bool HasOrganPermission { get; set; }
        public bool IsAdminOrgan { get; set; }
        public int Status { get; set; } = 1;
    }
}