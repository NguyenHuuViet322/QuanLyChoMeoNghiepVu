using ESD.Domain.Models.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMOrgan : Auditable
    {
        public int ID { get; set; }
        public int IDChannel { get; set; } = 0;
        [DisplayName("Mã cơ quan")]
        public string Code { get; set; }
        [DisplayName("Tên cơ quan")]
        public string Name { get; set; }
        public int Status { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("Mô tả")]
        public string Description { get; set; }
        [DisplayName("Fax")]
        public string Fax { get; set; }
        //[DisplayName("Cơ quan nộp lưu")]
        //public bool IsArchive { get; set; }
        //public int ParentId { get; set; }
        //[DisplayName("Cơ quan cha")]
        //public string ParentName { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
    }

    public class OrganCondition
    {
        public OrganCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        //public bool IsArchive { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

}
