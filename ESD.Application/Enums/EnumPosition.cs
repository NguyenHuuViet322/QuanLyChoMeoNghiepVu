using System.ComponentModel;

namespace ESD.Domain.Enums
{
    public static class EnumPosition
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Hiệu lực")]
            Active = 1,
        }
    }
}