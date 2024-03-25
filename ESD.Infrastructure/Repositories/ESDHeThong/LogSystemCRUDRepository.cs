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
    public class LogSystemCRUDRepository : DasBaseRepository<LogSystemCRUD>, ILogSystemCRUDRepository
    {
        public LogSystemCRUDRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}