using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CatalogingDocRepository : DasBaseRepository<CatalogingDoc>, ICatalogingDocRepository
    {
        public CatalogingDocRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
