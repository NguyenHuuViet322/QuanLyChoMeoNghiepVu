using System.ComponentModel;

namespace ESD.Application.Enums
{
    public class EnumOfficers
    {
        public enum EnumOfficer
        {
            [Description("Cán bộ huấn luyện")]
            CBHuanLuyen = 0,
            [Description("Cán bộ cảnh sát")]
            CBCanhSat = 1,
            [Description("Cán bộ thú y")]
            CBThuY = 2,
            [Description("Cán bộ cấp dưỡng")]
            CBCapDuong = 3,
        }
    }
}
