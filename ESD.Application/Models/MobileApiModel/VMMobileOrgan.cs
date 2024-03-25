using System.ComponentModel;
using ESD.Domain.Models.Abstractions;

namespace ESD.Application.Models.MobileApiModel
{
    public class VMMobileOrgan
    {
        public int ID { get; set; }
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
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Logo")]
        public long? Logo { get; set; }
        public string SrcLogo { get; set; }
        [DisplayName("Mã định danh cơ quan")]
        public string OrganIdentityCode { get; set; }
        [DisplayName("Thứ tự hiển thị")]
        public int Priority { get; set; }
    }

    public class MobileOrganCondition
    {
        public MobileOrganCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
