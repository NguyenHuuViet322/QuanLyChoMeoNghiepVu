using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
namespace ESD.Infrastructure.Repositories.DAS
{
    public class ReaderInOrganRepository : DasBaseRepository<ReaderInOrgan>, IReaderInOrganRepository
    {
        public ReaderInOrganRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
