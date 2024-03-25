using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class ProfileDestroyedRepository : DasBaseRepository<ProfileDestroyed>, IProfileDestroyedRepository
    {
        public ProfileDestroyedRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
