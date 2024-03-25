using ESD.Domain.Interfaces.ESDNghiepVu;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.ESDNghiepVu
{
    public interface IESDNghiepVuRepositoryWrapper
    {

        IChuyenMonKiThuatRepository ChuyenMonKiThuat { get; }
        ICoSoVatChatRepository CoSoVatChat { get; }
        ICoSoVatChat_DonViRepository CoSoVatChat_DonVi { get; }
        IDongVatNghiepVuRepository DongVatNghiepVu { get; }
        IDonViNghiepVuRepository DonViNghiepVu { get; }
        ILoaiChoNghiepVuRepository LoaiChoNghiepVu { get; }
        INghiepVuDongVatRepository NghiepVuDongVat { get; }
        INghiepVuDongVat_DinhKemRepository NghiepVuDongVat_DinhKem { get; }
        IThongTinCanBoRepository ThongTinCanBo { get; }
        ITCDinhLuongAnChoNVRepository TCDinhLuongAnChoNV { get; }
        ITCDMTrangBi_DonViRepository TCDMTrangBi_DonVi { get; }
        ITCDMTrangBiCBCS_ChoNVRepository TCDMTrangBiCBCS_ChoNV { get; }
        ITCDMTrangBiChoNVRepository TCDMTrangBiChoNV { get; }
        
        //RenderHere
        Task SaveAync();
    }
}