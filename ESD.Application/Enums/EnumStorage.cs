using System.ComponentModel;

namespace ESD.Domain.Enums
{
    public static class EnumStorage
    {
        public enum SearchProfileType
        {
            [Description("Hồ sơ")]
            Profile = 0,
            [Description("Thành phần hồ sơ")]
            ProfileElement = 1,
        }
    }
}