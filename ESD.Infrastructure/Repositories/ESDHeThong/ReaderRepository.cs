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
    public class ReaderRepository : DasBaseRepository<Reader>, IReaderRepository
    {
        public ReaderRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
