using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Domain.Enums
{
    public static class EnumCongViec
    {
        public enum Ca
        {
            [Description("Ca ngày (8:00 - 18:00)")]
            Ngay = 1,
            [Description("Ca đêm (18:00 - 8:00)")]
            Dem = 2,
        }
        public enum Status
        {
            [Description("Nháp")]
            Nhap = 1,
            [Description("Chờ duyệt")]
            ChoDuyet = 2,
            [Description("Đã duyệt")]
            DaDuyet = 3,
            [Description("Hủy")]
            Huy = 4,
        }
        public enum KetQua
        {
            [Description("OK")]
            OK = 1,
            [Description("Not OK")]
            NotOk = 2,
            [Description("Tồn")]
            Ton = 3,
        }
    }
}