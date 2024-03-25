using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class DonViNghiepVuRepository : ESDNghiepVuRepository<DonViNghiepVu>, IDonViNghiepVuRepository
    {
        public DonViNghiepVuRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
