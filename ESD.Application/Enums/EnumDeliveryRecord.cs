using System;
using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumDeliveryRecord
    {
        public enum Status
        {
            [Description("Không hoạt động")]
            Inactive = 0,
            [Description("Tạo mới")]
            Active = 1,
            [Description("Từ chối bàn giao")]
            Reject = 2,
            [Description("Đã bàn giao")]
            Complete = 3,
            [Description("Chờ tiếp nhận")]
            WaitingReceive = 4,
        }
    }
}
