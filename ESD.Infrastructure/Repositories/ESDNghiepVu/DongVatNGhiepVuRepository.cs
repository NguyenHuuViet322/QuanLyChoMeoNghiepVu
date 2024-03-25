using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class DongVatNghiepVuRepository : ESDNghiepVuRepository<DongVatNghiepVu>, IDongVatNghiepVuRepository
    {
        public DongVatNghiepVuRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
