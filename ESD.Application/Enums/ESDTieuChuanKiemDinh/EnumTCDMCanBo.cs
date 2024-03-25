using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums.ESDTieuChuanKiemDinh
{
    public enum NienHan
    {
        [Description("01 năm")]
        Nam = 1,
        [Description("01 tháng")]
        Thang = 2,
        [Description("02 tháng")]
        Hai_Thang = 3
    }
    public enum ChuyenMonKT_CanBo
    {
        [Description("Cán bộ làm công tác quản lý")]
        CanBoQL_DM = 0,
        [Description("Giáo viên hướng dẫn")]
        GiaoVienHD_DM = 1,
        [Description("CBCS trực tiếp quản lý, huấn luyện, sử dụng chó nghiệp vụ")]
        CanBoQLChoNV_DM = 2,
        [Description("Học viên trong thời gian học tập và huấn luyện tại Bộ Tư lệnh")]
        HocVien_DM = 3,
        [Description("Cán bộ làm công tác thú y")]
        CanBoThuY_DM = 4,
        [Description("Cán bộ cấp dưỡng")]
        NVCapDuong_DM = 5
    }

    public enum DonViTinh
    {
        [Description("Chiếc")]
        Chiec = 1,
        [Description("Bộ")]
        Bo = 2,
        [Description("Hộp")]
        Hop = 3,
        [Description("Cái")]
        Cai = 4,
        [Description("Kg")]
        Kg = 5,
        [Description("Quả")]
        Qua = 6,
        [Description("Lít")]
        Lit = 7,
    }
    
    public enum LoaiPhong
    {
        [Description("Phòng lưu trữ, bảo quản mùi hơi (chỉ trang bị cho K02)")]
        PhongQlMuiHoi = 1,
        [Description("Phòng sao chép, xử lý hơi (chỉ trang bị cho K02, PK02, PK02E)")]
        PhongXlHoi = 2,
    }

    public static class EnumExtensionMethods
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
    }

    public enum TypeCanBo
    {
        [Description("Huấn luyện")]
        HuyanLuyen = 0,
        [Description("Cảnh sát")]
        CanhSat = 1,
        [Description("Thú y")]
        ThuY = 2,
        [Description("Cấp dưỡng")]
        CapDuong = 3,
    }
}
