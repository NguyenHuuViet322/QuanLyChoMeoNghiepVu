using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class PositionRepository : DasBaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<bool> IsCodeExist(string code, int id = 0)
        {

            return await DasContext.Position.AnyAsync(n => n.Code.Trim() == code.Trim() && n.ID != id && n.Status >0 /*Todo enum*/) ;
        }
    }
}