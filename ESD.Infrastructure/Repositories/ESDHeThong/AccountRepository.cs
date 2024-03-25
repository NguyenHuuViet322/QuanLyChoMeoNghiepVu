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
    public class AccountRepository : DasBaseRepository<User>, IAccountRepository
    {
        public AccountRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
