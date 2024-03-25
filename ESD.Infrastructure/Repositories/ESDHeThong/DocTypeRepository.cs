using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DocTypeRepository : DasBaseRepository<DocType>, IDocTypeRepository
    {
        public DocTypeRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<bool> IsCodeExist(string code, int status, int id = 0)
        {
            return await DasContext.DocType.AnyAsync(n => n.Code.Trim() == code.Trim() && n.ID != id && n.Status == status);
        }
    }
}