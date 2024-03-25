using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums
{
    public class EnumReader
    {
        public enum Gender
        {
            [Description("Nam")]
            Male = 0,
            [Description("Nữ")]
            Female = 1,
            [Description("Khác")]
            Other = 2,
        }
    }
}
