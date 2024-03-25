namespace DAS.Web.Constants
{
    /// <summary>
    /// Constants chung
    /// </summary>
    public static class CustomsConfig
    {
        /// <summary>
        /// Format to En
        /// </summary>
        public const string CultureInfo = "en-us";
        /// <summary>
        /// Format to VN
        /// </summary>
        public const string CultureInfoVn = "vi-VN";
        /// <summary>
        /// Format tiền ~ tỷ nhé
        /// </summary>
        public const string MoneyBillionVn = "#,##0,,, Tỷ";
        public const int MoneyBillionVn2 = 1000000000;

        #region Config Controller Menu

        /// <summary>
        /// Controller Thông tin cán bộ
        /// </summary>
        public const string ThongTinCanBo = "ThongTinCanBo";
         
        /// <summary>
        /// Cấu hình router trang chủ
        /// </summary>
        public const string TrangChu = "IndexBid";


        /// <summary>
        /// Controller DongVatNghiepVu
        /// </summary>
        public const string DongVatNghiepVu = "DongVatNghiepVu";

        /// <summary>
        /// Controller DonViNghiepVu
        /// </summary>
        public const string DonViNghiepVu = "DonViNghiepVu";
        #endregion

        #region Config Router menu

        /// <summary>
        /// Router Thông tin cán bộ
        /// </summary>
        public const string RouterThongTinCanBo = "CanBo";
         
        #endregion

        #region Base
        public const string DangNhap = "Login";
        #endregion
    }
}
