using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CategoryRepository : DasBaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<bool> IsCodeExist(string code, int status, int idOrgan, int id = 0)
        {
            return await DasContext.Category.AnyAsync(n => n.Code.Trim() == code.Trim() && n.IDOrgan == idOrgan && n.ID != id && n.Status == status);
        }
        public Task<bool> GetPaging()
        {
            throw new NotImplementedException();
        }
    }
}