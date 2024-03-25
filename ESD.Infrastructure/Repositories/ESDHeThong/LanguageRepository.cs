using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class LanguageRepository : DasBaseRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
