using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class ProfileTemplateRepository : DasBaseRepository<ProfileTemplate>, IProfileTemplateRepository
    {
        public ProfileTemplateRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
