using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Domain.Enums
{
    public static class EnumHome
    {
        public enum TypeDataDashBoard
        {
            [Description("Đơn vị")]
            Agency = 1,
            [Description("Cơ quan")]
            Organ = 2,
        }
    }
}