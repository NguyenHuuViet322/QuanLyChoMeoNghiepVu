using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class TCDMTrangBiCBCS_ChoNVRepository : ESDNghiepVuRepository<TCDMTrangBiCBCS_ChoNV>, ITCDMTrangBiCBCS_ChoNVRepository
    {
        public TCDMTrangBiCBCS_ChoNVRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
