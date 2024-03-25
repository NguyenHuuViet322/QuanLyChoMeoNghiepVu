using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CatalogingBorrowDocRepository : DasBaseRepository<CatalogingBorrowDoc>, ICatalogingBorrowDocRepository
    {
        public CatalogingBorrowDocRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
         
    }
}
