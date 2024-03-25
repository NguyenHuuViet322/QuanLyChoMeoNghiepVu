using System.ComponentModel;

namespace ESD.Domain.Enums
{
    public static class EnumPlan
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Tạo mới")]
            Active = 1, 
            [Description("Chờ duyệt")]
            NotApproved = 2,
            [Description("Đã duyệt")]
            Approved = 3,
            [Description("Từ chối")]
            Reject = 4,

            //Fake status
            [Description("Đã dóng")]
            Close = 5
        }
    }
}