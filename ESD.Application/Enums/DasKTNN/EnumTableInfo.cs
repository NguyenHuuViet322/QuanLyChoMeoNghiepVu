using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums.DasKTNN
{
    public static class EnumTableInfo
    {
        public enum ColumnDataType
        {
            [Description("Chuỗi")]
            String = 1,
            [Description("Số nguyên")]
            Integer = 2,
            [Description("Số thập phân")]
            Decimal = 3,
            [Description("Ngày tháng")]
            DateTime = 4,
            [Description("Boolean")]
            Boolean = 5,
            [Description("Tham chiếu bảng khác")]
            TableRef = 6,
            [Description("Tham chiếu bảng người dùng")]
            TableUser = 7,
            [Description("Cha - con")]
            Parent = 8,
            [Description("Liên kết")]
            TableLink = 9,
        }

        #region Fields
        public enum SystemField
        {
            [Description("ID")]
            ID = 1,
            [Description("Người tạo")]
            NguoiTao = 2,
            [Description("Ngày tạo")]
            NgayTao = 3,
            [Description("Ngày sửa")]
            NgaySua = 4,
            [Description("Người sửa")]
            NguoiSua = 5,
            [Description("Trạng thái")]
            TrangThai = 6,

            [Description("Đã được duyệt")]
            DaDuyet = 7,
            [Description("Được duyệt")]
            NgayDuyet = 8,
            [Description("Được duyệt")]
            NguoiDuyet = 9
        }
        public enum DefaultField
        {
            [Description("Tên")]
            Ten = 1,
            [Description("Ghi chú")]
            GhiChu = 2,
            [Description("Thứ tự")]
            ThuTu = 3,
            [Description("Danh mục cha")]
            DanhMucCha = 4,
            [Description("Hiệu lực từ")]
            HieuLucTu = 5,
            [Description("Hiệu lực đến")]
            HieuLucDen = 6,
        }

        public enum CategoryDefaultField
        {
            [Description("Tên danh mục")]
            TenDanhMuc = 1,
            [Description("Mã danh mục")]
            MaDanhMuc = 2,
        }


        public enum CustomField
        {
            [Description("ID phân tách")]
            IDTach = 1,
            [Description("ID hợp nhất")]
            IDGop = 2,
            [Description("Dữ liệu phân tách")]
            DataTach = 3,
            [Description("Dữ liệu hợp nhất")]
            DataGop = 4,
            [Description("Đã đồng bộ")]
            DaDongBo = 5,

            [Description("Tên CSDL")]
            Schemaname = 6,
            [Description("Tên bảng dữ liệu")]
            Tablename = 7,
            [Description("Data Json")]
            DataJson = 8,
            [Description("Url CallBack")]
            UrlCallBack = 9,
            [Description("App Name")]
            AppName = 10,
            [Description("Reason")]
            Reason = 11,
        }

        #endregion

        /// <summary>
        /// Kiểu xem của bảng 
        /// </summary>
        public enum TableListType
        {
            [Description("Dạng bảng")]
            Table = 1,
            [Description("Dạng cây")]
            Tree = 2,
        }

        public enum Status
        {
            [Description("Chưa duyệt")]
            Choduyet = 0,
            [Description("Đã duyệt")]
            DaDuyet = 1,
        }

        public enum ColumnTableGroupCode
        {
            [Description("Hệ thống")]
            SYS = 1,
            [Description("Thông tin")]
            INFO = 2,
        }
        public enum Sync
        {
            [Description("Chưa đồng bộ")]
            ChuaDongBo = 0,
            [Description("Đã đồng bộ")]
            DongBo = 1,
            [Description("Từ chối")]
            TuChoi = 2,
        }

        public enum TableInfoApiConfigType
        {
            [Description("Lấy danh sách phần tử")]
            GetTableRecord = 1,
            [Description("Lấy thông tin bảng")]
            GetTableInfo = 2,
        }
    }
}