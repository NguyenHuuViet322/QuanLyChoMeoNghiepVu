using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums
{
    public static class EnumTemplate
    {
        public enum Status {
            [Description("Không hiệu lực")]
            Inactive = 0,
            [Description("Có hiệu lực")]
            Active = 1
        }
    }
}
