using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class ResetPasswordRepository : DasBaseRepository<ResetPassword>, IResetPasswordRepository
    {
        public ResetPasswordRepository(ESDContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
