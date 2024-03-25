using System.ComponentModel;

namespace ESD.Application.Enums.DasKTNN
{

    /// <summary>
    /// Tình trạng nghiệp vụ 
    /// </summary>
    public enum PhanLoaiDVNV
    {
        [Description("Bình thường")]
        BinhThuong = 1,
        [Description("Loại, thải")]
        Loai = 2,
        [Description("Chết")]
        Mat = 3,
    } 
    
    /// <summary>
    /// Menu động vật 
    /// </summary>
    public enum MenuDVNV
    {
        [Description("Bình thường")]
        BinhThuong = 1,
        [Description("Loại/Chết")]
        Loai= 2,
    }
  
    /// <summary>
    /// Phân loại file đính kèm 
    /// </summary>
    public enum PhanLoaiDinhKem
    {
        [Description("Hình ảnh")]
        HinhAnh = 1,
        [Description("Đính kèm")]
        File = 2,
    }
    
    /// <summary>
    /// Phân loại đơn vị 
    /// </summary>
    public enum PhanLoaiDonVi
    {
        [Description("Đơn vị nghiệp vụ")]
        DVNghiepVu = 1,
        [Description("Trại giam")]
        TraiGiam = 2,
    }

    /// <summary>
    /// Gioi tinh dv
    /// </summary>
    public enum GioiTinhDongVat
    {
        [Description("Đực")]
        Duc = 1,
        [Description("Cái")]
        Cai = 2,
    }
    
    /// <summary>
    /// Trọng lượng tối da
    /// </summary>
    public enum TrongLuongToiDa
    {
        [Description("Dưới 30kg")]
        Duoi30Kg = 1,
        [Description("Từ 30kg trở lên")]
        Tren30Kg = 2,  
    }

    /// <summary>
    /// Phan loai dong vat
    /// </summary>
    public enum PhanLoaiDongVat
    {   
        [Description("Chó sinh sản (con)")]
        ChoSinhSanCon = 1,
        [Description("Chó sinh sản (bố, mẹ)")]
        ChoSinhSanBoMe = 2,
        [Description("Chó dự bị")]
        ChoDuBi = 3,
        [Description("Chó chuyên khoa nghiệp vụ")]
        ChoNghiepVu = 4,
        [Description("Chó nhập")]
        ChoNhap = 5,
    }
}