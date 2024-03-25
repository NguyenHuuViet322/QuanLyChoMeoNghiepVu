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
    public class GroupPermissionRepository : DasBaseRepository<GroupPermission>, IGroupPermissionRepository
    {
        public GroupPermissionRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<bool> IsNameExist(string name, int id = 0)
        {
            return await DasContext.GroupPermission.AnyAsync(n => n.Name.Trim() == name.Trim() && n.ID != id);
        }
    }
}