using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class SystemConfigRepository : DasBaseRepository<SystemConfig>, ISystemConfigRepository
    {
        public SystemConfigRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<object> GetConfigByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }
            else
            {
                var systemConfig = (await GetAllListAsync(x => x.Code == code && x.Status == 1)).FirstOrDefault();
                if (systemConfig == null)
                {
                    return null;
                }
                if (systemConfig.IntVal != null)
                {
                    return systemConfig.IntVal;
                }
                else if(systemConfig.FloatVal != null)
                {
                    return systemConfig.FloatVal;
                }
                else if (systemConfig.DateTimeVal != null)
                {
                    return systemConfig.DateTimeVal;
                }
                else if (!string.IsNullOrEmpty(systemConfig.StringVal))
                {
                    return systemConfig.StringVal;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

