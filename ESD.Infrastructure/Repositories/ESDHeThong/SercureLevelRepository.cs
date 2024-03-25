using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class SercureLevelRepository : DasBaseRepository<SercureLevel>, ISercureLevelRepository
    {
        public SercureLevelRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
