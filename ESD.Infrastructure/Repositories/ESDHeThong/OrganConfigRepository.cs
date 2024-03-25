using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using ESD.Infrastructure.ContextAccessors;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class OrganConfigRepository : DasBaseRepository<OrganConfig>, IOrganConfigRepository
    {
        public OrganConfigRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async  Task<object> GetConfigByCode(string code, int idOrgan = 0)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }
            else
            {
                var organConfig = (await GetAllListAsync(x => x.Code == code && x.IDOrgan == idOrgan && x.Status == 1)).FirstOrDefault();
                if (organConfig == null)
                {
                    return null;
                }
                if (organConfig.IntVal != null)
                {
                    return organConfig.IntVal;
                }
                else if (organConfig.FloatVal != null)
                {
                    return organConfig.FloatVal;
                }
                else if (organConfig.DateTimeVal != null)
                {
                    return organConfig.DateTimeVal;
                }
                else if (!string.IsNullOrEmpty(organConfig.StringVal))
                {
                    return organConfig.StringVal;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
