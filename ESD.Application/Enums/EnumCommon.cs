using System.ComponentModel;

namespace ESD.Domain.Enums
{
    public static class EnumCommon
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Hiệu lực")]
            Active = 1,
        }

        public enum IsShow
        {
            [Description("Hiển thị")]
            Show = 1,
            [Description("Ẩn")]
            Hidden = 0,
        }
    }
}