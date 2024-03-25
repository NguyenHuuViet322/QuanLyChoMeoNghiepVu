using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Domain.Enums
{
    public static class EnumRole
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