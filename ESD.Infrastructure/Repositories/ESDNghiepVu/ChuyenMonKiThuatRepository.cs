using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class ChuyenMonKiThuatRepository : ESDNghiepVuRepository<ChuyenMonKiThuat>, IChuyenMonKiThuatRepository
    {
        public ChuyenMonKiThuatRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
