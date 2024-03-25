using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class CoSoVatChat_DonViRepository : ESDNghiepVuRepository<CoSoVatChat_DonVi>, ICoSoVatChat_DonViRepository
    {
        public CoSoVatChat_DonViRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
