using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class NghiepVuDongVatRepository : ESDNghiepVuRepository<NghiepVuDongVat>, INghiepVuDongVatRepository
    {
        public NghiepVuDongVatRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
