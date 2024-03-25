using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumCataloging
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Tạo mới")]
            Active = 1,
            [Description("Chờ duyệt")]
            WaitApprove = 2,
            [Description("Đã duyệt")]
            Approved = 3,
            [Description("Từ chối")]
            Reject = 4,
            [Description("Hoàn thành")]
            CollectComplete = 5,
            [Description("Từ chối nộp lưu kho")]
            StorageReject = 6,
            [Description("Duyệt nộp lưu kho")]
            StorageApproved = 7,
            [Description("Đã tiêu hủy")]
            Destroyed = 8,
        }

        public enum InUse
        {
            [Description("Đang sử dụng")]
            Using = 1,
            [Description("Hết giá trị")]
            OffValue =2,
            [Description("ĐỢi tiêu hủy")]
            WaitDestructionExpiry = 3,
            [Description("ĐỢi tiêu hủy")]
            WaitDestructionUnUse = 4
        }
        public enum StatusDestruction
        {
            [Description("Hết thời hạn")]
            Expiry = 1,
            [Description("Hết giá trị")]
            OffValue = 2
        }
    }
}