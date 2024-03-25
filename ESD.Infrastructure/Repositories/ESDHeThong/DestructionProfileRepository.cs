using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DestructionProfileRepository : DasBaseRepository<DestructionProfile>, IDestructionProfileRepository
    {
        public DestructionProfileRepository(ESDContext repositoryContext)
           : base(repositoryContext)
        {
        }
    }
}
