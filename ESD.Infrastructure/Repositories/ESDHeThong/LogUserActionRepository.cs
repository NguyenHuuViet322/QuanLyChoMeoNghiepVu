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
    public class LogUserActionRepository : DasBaseRepository<LogUserAction>, ILogUserActionRepository
    {
        public LogUserActionRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

       
    }
}