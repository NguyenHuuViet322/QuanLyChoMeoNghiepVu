using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class CoSoVatChatRepository : ESDNghiepVuRepository<CoSoVatChat>, ICoSoVatChatRepository
    {
        public CoSoVatChatRepository(ESDNGHIEPVUContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
