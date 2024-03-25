using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class CatalogingDocFieldRepository : DasBaseRepository<CatalogingDocField>, ICatalogingDocFieldRepository
    {
        public CatalogingDocFieldRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}