using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums
{
    public static class EnumSercureLevel
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
