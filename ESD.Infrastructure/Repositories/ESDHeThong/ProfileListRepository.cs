using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class ProfileListRepository : DasBaseRepository<ProfileList>, IProfileListRepository
    {
        public ProfileListRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
