using System.ComponentModel;

namespace ESD.Application.Enums
{
    public class EnumCategoryType
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Hiệu lực")]
            Active = 1,
        }
        public enum InputType
        {
            [Description("Chữ")]
            InpText = 0,
            [Description("Số nguyên")]
            InpNumber = 1, //int
            [Description("Số thập phân")]
            InpFloat = 2, //float
            [Description("Tiền tệ")]
            InpMoney = 3, //float
            [Description("Ngày tháng")]
            InpDate = 4,
            [Description("TextArea")]
            InpTextArea = 5,
            [Description("Danh mục động")]
            CategoryType = 6,
            [Description("Danh mục đơn vị")]
            Agency = 7,
            [Description("Danh mục phông")]
            ProfileTemplate = 8,
            [Description("Danh mục cấp cha")]
            Parent = 9
        }
        public enum Code
        {
            [Description("Kho")]
            DM_Kho = 1, 
            [Description("Phông")]
            DM_Phong = 2, 
            [Description("Mục lục")]
            DM_MucLuc = 3, 
            [Description("Hộp số")]
            DM_HopSo = 4, 
            [Description("Ngôn ngữ")]
            DM_NgonNgu = 5, 
            [Description("Thời hạn bảo quản")]
            DM_THBaoQuan = 6, 
            [Description("Cấp độ bảo mật")]
            DM_CapDoBaoMat = 7, 
            [Description("Giá")]
            DM_Gia = 8, 
            [Description("Phân loại hồ sơ")]
            DM_PhanLoaiHS = 9,
            [Description("Công việc")]
            DM_CongViec = 10,
            [Description("Loại công việc")]
            DM_LoaiCongViec = 11,
            [Description("Kiểu công việc")]
            DM_KieuCongViec = 12,
        }

        /// <summary>
        /// Giá trị mặc định
        /// </summary>
        public enum DefaultValue
        {
            [Description("Theo người dùng")]
            ByUser = 1, 
            [Description("Ngày tháng hiện tại")]
            DateTimeNow = 2,
        }
    }
}