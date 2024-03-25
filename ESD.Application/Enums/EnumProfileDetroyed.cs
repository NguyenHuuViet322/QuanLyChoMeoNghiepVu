using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumProfileDetroyed
    {
        public enum Status
        {

            [Description("Đã tiêu hủy")]
            Detroyed = 1,
            [Description("Đã khôi phục")]
            Restored = 2,
        }
    }
}
