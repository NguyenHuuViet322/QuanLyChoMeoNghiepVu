using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class NghiepVuDongVat_DinhKemRepository : ESDNghiepVuRepository<NghiepVuDongVat_DinhKem>, INghiepVuDongVat_DinhKemRepository
    {
        public NghiepVuDongVat_DinhKemRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
