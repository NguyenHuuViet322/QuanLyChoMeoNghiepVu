using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class TCDinhLuongAnChoNVRepository : ESDNghiepVuRepository<TCDinhLuongAnChoNV>, ITCDinhLuongAnChoNVRepository
    {
        public TCDinhLuongAnChoNVRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
