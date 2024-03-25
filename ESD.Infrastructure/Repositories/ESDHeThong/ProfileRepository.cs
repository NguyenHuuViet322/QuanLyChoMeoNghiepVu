using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class ProfileRepository : DasBaseRepository<Profile>, IProfileRepository
    {
        public ProfileRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<bool> IsCodeExist(string code, int status, int id = 0)
        {
            return await DasContext.Profile.AnyAsync(n => n.FileCode.Trim() == code.Trim() && n.ID != id && n.Status == status);
        }
    }
}