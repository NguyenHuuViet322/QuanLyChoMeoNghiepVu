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
    public class OrganRepository : DasBaseRepository<Organ>, IOrganRepository
    {
        public OrganRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        //public async Task<bool> IsEmailExist(string email)
        //{
        //    return await DasContext.User.AnyAsync(s => s.Email == email);
        //}
    }
}