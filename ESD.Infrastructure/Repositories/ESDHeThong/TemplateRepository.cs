using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class TemplateRepository : DasBaseRepository<Template>, ITemplateRepository
    {
        public TemplateRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
