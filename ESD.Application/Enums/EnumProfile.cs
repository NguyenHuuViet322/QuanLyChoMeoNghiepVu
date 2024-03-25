using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumProfile
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Hiệu lực")]
            Active = 1,
        } 
        public enum Type
        {
            [Description("Số hóa")]
            Digital = 1,
            [Description("Điện tử")]
            Electronic = 2
        }
    }
    public static class EnumProfilePlan
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Đang thu thập")]
            Active = 1,
            [Description("Chờ duyệt")]
            WaitApprove = 2,
            [Description("Chờ duyệt nộp lưu")]
            WaitArchiveApproved = 3,
            [Description("Từ chối thu thập")]
            Reject = 4,
            [Description("Hoàn thành")]
            CollectComplete = 5,
            [Description("Từ chối nộp lưu")]
            ArchiveReject = 6,
            [Description("Duyệt nộp lưu")]
            ArchiveApproved = 7,
            [Description("Đã bàn giao")]
            DeliveryComplete = 8,
        }
    }
}