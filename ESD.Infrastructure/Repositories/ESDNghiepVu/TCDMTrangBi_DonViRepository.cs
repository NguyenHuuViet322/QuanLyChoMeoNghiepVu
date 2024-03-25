using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class TCDMTrangBi_DonViRepository : ESDNghiepVuRepository<TCDMTrangBi_DonVi>, ITCDMTrangBi_DonViRepository
    {
        public TCDMTrangBi_DonViRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
