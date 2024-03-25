using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class StgFileRepository : DasBaseRepository<StgFile>, IStgFileRepository
    {
        public StgFileRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
