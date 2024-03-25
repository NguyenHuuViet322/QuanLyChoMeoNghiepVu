using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums
{
    public static class EnumModule
    {
        public enum Code
        {
            #region Quản trị hệ thống
            [Description("Quản lý cơ quan")]
            M20010 = 1,
            [Description("Quản lý tài khoản quản trị cơ quan")]
            S9023 = 2,
            [Description("Nhật ký người dùng")]
            NKND = 3,
            [Description("Nhật ký hệ thống")]
            NKHT = 4,
            [Description("Cấu hình tham số cơ quan")]
            CHTS = 5,

            [Description("Quản lý menu")]
            QLMENU = 48, //chưa sử dụng

            [Description("Quản lý đơn vị")]
            M20020 = 6,
            [Description("Quản lý nhóm quyền")]
            S9030 = 7,
            [Description("Quản lý nhóm người dùng")]
            S9010 = 8,
            [Description("Quản lý người dùng")]
            S9020 = 9,
            [Description("Quản lý phân quyền")]
            QLPQ = 10,//Chưa sử dụng

            #endregion

            #region Quản trị danh mục chung
            [Description("Chức vụ")]
            M20030 = 14,
            #endregion

            [Description("Báo cáo thống kê về lỗi hệ thống")]
            SYSTEMLOGERROL = 57,

            #region csdl
            [Description("Động vật nghiệp vụ")]
            DongVatNghiepVu = 100,
            [Description("Quản lý cơ sở vật chất")]
            CoSoVatChat = 101,
            [Description("Quản lý đơn vị nghiệp vụ - trại giam")]
            DonViNghiepVu = 102,
            [Description("Quản lý cán bộ")]
            ThongTinCanBo = 103,
            [Description("Quản lý danh mục")]
            QLDM = 104,
            [Description("Cấp phát hàng nghiệp vụ CSVC")]
            CapPhatCSVC = 105,
            [Description("Quản lý nghiệp vụ động vật")]
            NghiepVuDongVat = 106,
            #endregion
        }
    }
}
