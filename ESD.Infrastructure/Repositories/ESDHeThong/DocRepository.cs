using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DocRepository : DasBaseRepository<Doc>, IDocRepository
    {
        public DocRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
