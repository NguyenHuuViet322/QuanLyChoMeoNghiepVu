using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class TCDMTrangBiChoNVRepository : ESDNghiepVuRepository<TCDMTrangBiChoNV>, ITCDMTrangBiChoNVRepository
    {
        public TCDMTrangBiChoNVRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
