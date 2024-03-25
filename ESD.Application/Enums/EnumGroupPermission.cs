using System.ComponentModel;

namespace ESD.Domain.Enums
{
    public static class EnumGroupPermission
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
            [Description("Admin cơ quan")]
            AdminOrgan = 1,
        }

        public enum ActiveNotification
        {
            [Description("Chưa kích hoạt")]
            InActive = 0,
            [Description("Kích hoạt")]
            Active = 1,
        }
    }
}