using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CategoryTypeFieldRepository : DasBaseRepository<CategoryTypeField>, ICategoryTypeFieldRepository
    {
        public CategoryTypeFieldRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        } 
    }
}