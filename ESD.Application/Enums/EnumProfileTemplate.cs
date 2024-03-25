using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Domain.Enums
{
    public static class EnumProfileTemplate
    {
        public enum Type
        {
            [Description("Phông mở")]
            Open = 1,
            [Description("Phông đóng")]
            Close = 2,
        }
    }
}