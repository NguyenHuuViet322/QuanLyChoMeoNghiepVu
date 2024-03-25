using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class ESDNghiepVuRepositoryWrapper : IESDNghiepVuRepositoryWrapper
    {
        #region ctor
        private readonly ESDNGHIEPVUContext _repoContext;

        public ESDNghiepVuRepositoryWrapper(ESDNGHIEPVUContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
        #endregion

        #region properties

        private IChuyenMonKiThuatRepository _chuyenMonKiThuat;
        public IChuyenMonKiThuatRepository ChuyenMonKiThuat
        {
            get
            {
                if (_chuyenMonKiThuat == null)
                {
                    _chuyenMonKiThuat = new ChuyenMonKiThuatRepository(_repoContext);
                }
                return _chuyenMonKiThuat;
            }
        }
        private ICoSoVatChatRepository _coSoVatChat;
        public ICoSoVatChatRepository CoSoVatChat
        {
            get
            {
                if (_coSoVatChat == null)
                {
                    _coSoVatChat = new CoSoVatChatRepository(_repoContext);
                }
                return _coSoVatChat;
            }
        }
        private ICoSoVatChat_DonViRepository _coSoVatChat_DonVi;
        public ICoSoVatChat_DonViRepository CoSoVatChat_DonVi
        {
            get
            {
                if (_coSoVatChat_DonVi == null)
                {
                    _coSoVatChat_DonVi = new CoSoVatChat_DonViRepository(_repoContext);
                }
                return _coSoVatChat_DonVi;
            }
        }
        private IDongVatNghiepVuRepository _dongVatNghiepVu;
        public IDongVatNghiepVuRepository DongVatNghiepVu
        {
            get
            {
                if (_dongVatNghiepVu == null)
                {
                    _dongVatNghiepVu = new DongVatNghiepVuRepository(_repoContext);
                }
                return _dongVatNghiepVu;
            }
        }
        private IDonViNghiepVuRepository _donViNghiepVu;
        public IDonViNghiepVuRepository DonViNghiepVu
        {
            get
            {
                if (_donViNghiepVu == null)
                {
                    _donViNghiepVu = new DonViNghiepVuRepository(_repoContext);
                }
                return _donViNghiepVu;
            }
        }
        private ILoaiChoNghiepVuRepository _loaiChoNghiepVu;
        public ILoaiChoNghiepVuRepository LoaiChoNghiepVu
        {
            get
            {
                if (_loaiChoNghiepVu == null)
                {
                    _loaiChoNghiepVu = new LoaiChoNghiepVuRepository(_repoContext);
                }
                return _loaiChoNghiepVu;
            }
        }
        private INghiepVuDongVatRepository _nghiepVuDongVat;
        public INghiepVuDongVatRepository NghiepVuDongVat
        {
            get
            {
                if (_nghiepVuDongVat == null)
                {
                    _nghiepVuDongVat = new NghiepVuDongVatRepository(_repoContext);
                }
                return _nghiepVuDongVat;
            }
        }
        private INghiepVuDongVat_DinhKemRepository _nghiepVuDongVat_DinhKem;
        public INghiepVuDongVat_DinhKemRepository NghiepVuDongVat_DinhKem
        {
            get
            {
                if (_nghiepVuDongVat_DinhKem == null)
                {
                    _nghiepVuDongVat_DinhKem = new NghiepVuDongVat_DinhKemRepository(_repoContext);
                }
                return _nghiepVuDongVat_DinhKem;
            }
        }
        private IThongTinCanBoRepository _thongTinCanBo;
        public IThongTinCanBoRepository ThongTinCanBo
        {
            get
            {
                if (_thongTinCanBo == null)
                {
                    _thongTinCanBo = new ThongTinCanBoRepository(_repoContext);
                }
                return _thongTinCanBo;
            }
        }

        private ITCDinhLuongAnChoNVRepository _tCDinhLuongAnChoNV;
        public ITCDinhLuongAnChoNVRepository TCDinhLuongAnChoNV
        {
            get
            {
                if (_tCDinhLuongAnChoNV == null)
                {
                    _tCDinhLuongAnChoNV = new TCDinhLuongAnChoNVRepository(_repoContext);
                }
                return _tCDinhLuongAnChoNV;
            }
        }
        private ITCDMTrangBi_DonViRepository _tCDMTrangBi_DonVi;
        public ITCDMTrangBi_DonViRepository TCDMTrangBi_DonVi
        {
            get
            {
                if (_tCDMTrangBi_DonVi == null)
                {
                    _tCDMTrangBi_DonVi = new TCDMTrangBi_DonViRepository(_repoContext);
                }
                return _tCDMTrangBi_DonVi;
            }
        }
        private ITCDMTrangBiCBCS_ChoNVRepository _tCDMTrangBiCBCS_ChoNV;
        public ITCDMTrangBiCBCS_ChoNVRepository TCDMTrangBiCBCS_ChoNV
        {
            get
            {
                if (_tCDMTrangBiCBCS_ChoNV == null)
                {
                    _tCDMTrangBiCBCS_ChoNV = new TCDMTrangBiCBCS_ChoNVRepository(_repoContext);
                }
                return _tCDMTrangBiCBCS_ChoNV;
            }
        }
        private ITCDMTrangBiChoNVRepository _tCDMTrangBiChoNV;
        public ITCDMTrangBiChoNVRepository TCDMTrangBiChoNV
        {
            get
            {
                if (_tCDMTrangBiChoNV == null)
                {
                    _tCDMTrangBiChoNV = new TCDMTrangBiChoNVRepository(_repoContext);
                }
                return _tCDMTrangBiChoNV;
            }
        }
        //RenderHere

        #endregion

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}