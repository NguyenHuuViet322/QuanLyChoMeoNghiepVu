using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CategoryTypeRepository : DasBaseRepository<CategoryType>, ICategoryTypeRepository
    {
        public CategoryTypeRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<bool> IsCodeExist(string code, int status, int id = 0, int idOrgan = 0)
        {
            return await DasContext.CategoryType.AnyAsync(n => n.Code.Trim() == code.Trim() && n.ID != id && n.Status == status && n.IDOrgan == idOrgan);
        }
    }
}