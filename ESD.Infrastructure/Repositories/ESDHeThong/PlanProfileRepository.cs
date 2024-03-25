using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class PlanProfileRepository : DasBaseRepository<PlanProfile>, IPlanProfileRepository
    {
        public PlanProfileRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<bool> IsCodeExist(string code, int status, int id = 0)
        {
            return await DasContext.PlanProfile.AnyAsync(n => n.FileCode.Trim() == code.Trim() && n.ID != id && n.Status == status);
        }
    }
}