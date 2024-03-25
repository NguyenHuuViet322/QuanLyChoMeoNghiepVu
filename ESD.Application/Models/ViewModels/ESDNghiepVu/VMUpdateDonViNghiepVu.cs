namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMUpdateDonViNghiepVu : VMDonViNghiepVu
    {

        public string TenPhanLoai
        {
            get { return PhanLoaiDonVi == (int)Enums.DasKTNN.PhanLoaiDonVi.TraiGiam ? "Trại giam" : "Đơn vị nghiệp vụ"; }
        }
    }
}