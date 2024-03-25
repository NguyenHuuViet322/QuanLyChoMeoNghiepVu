using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class PlanAgencyRepository : DasBaseRepository<PlanAgency>, IPlanAgencyRepository
    {
        public PlanAgencyRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

    }
}