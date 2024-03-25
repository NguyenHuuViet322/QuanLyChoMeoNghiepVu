using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class ExpiryDateRepository : DasBaseRepository<ExpiryDate>, IExpiryDateRepository
    {
        public ExpiryDateRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
