using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class UserBookMarkRepository : DasBaseRepository<UserBookmark>, IUserBookMarkRepository
    {
        public UserBookMarkRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
