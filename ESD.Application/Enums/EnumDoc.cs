using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumDoc
    {
    }

    public static class EnumDocCollect
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Đang biên mục")]
            Active = 1,
            [Description("Hoàn thành")]
            Complete = 2,
            [Description("Đã tiêu hủy")]
            Destroyed = 3,
        }
    }
}
