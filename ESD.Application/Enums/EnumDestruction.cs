using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumDestruction
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Tạo mới")]
            Active = 1,
            [Description("Chờ duyệt tiêu huỷ")]
            WaitApprove = 2,
            [Description("Đã tiêu hủy")]
            Approved = 3,
            [Description("Từ chối tiêu hủy")]
            Reject = 4,
            [Description("Đã khôi phục")]
            Restored = 5,
        }
    }
}
