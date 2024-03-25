using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DocTypeFieldRepository : DasBaseRepository<DocTypeField>, IDocTypeFieldRepository
    {
        public DocTypeFieldRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        } 
    }
}