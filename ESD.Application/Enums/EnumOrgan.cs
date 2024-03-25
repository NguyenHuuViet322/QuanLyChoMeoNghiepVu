using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Domain.Enums
{
    public static class EnumOrgan
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