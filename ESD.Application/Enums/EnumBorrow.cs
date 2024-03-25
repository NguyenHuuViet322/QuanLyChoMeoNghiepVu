using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumBorrow
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0, 
            [Description("Chờ duyệt")]
            WaitApprove = 1,
            [Description("Đã duyệt")]
            Approved = 2,
            [Description("Từ chối")]
            Reject = 4,
            [Description("Huỷ")]
            Cancel = 5,
        }

        public enum BorrowType
        {
            [Description("Người dùng bên ngoài")]
            Reader = 0, 
            [Description("Người dùng nội bộ")]
            User = 1, 
        }
    }
}