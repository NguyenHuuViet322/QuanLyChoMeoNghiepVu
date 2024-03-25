using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class StorageRepository : DasBaseRepository<Storage>, IStorageRepository
    {
        public StorageRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }


        public async Task<bool> IsCodeExist(string email, int id = 0)
        {
            return await DasContext.Position.AnyAsync(n => n.Code.Trim() == email.Trim() && n.ID != id);
        }
    }
}
