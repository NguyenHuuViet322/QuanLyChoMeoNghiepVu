using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class LoaiChoNghiepVuRepository : ESDNghiepVuRepository<LoaiChoNghiepVu>, ILoaiChoNghiepVuRepository
    {
        public LoaiChoNghiepVuRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
