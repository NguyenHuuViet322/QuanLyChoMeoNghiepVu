using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CatalogingBorrowRepository : DasBaseRepository<CatalogingBorrow>, ICatalogingBorrowRepository
    {
        public CatalogingBorrowRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
         
    }
}
