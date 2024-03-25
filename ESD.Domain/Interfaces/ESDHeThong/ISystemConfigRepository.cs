using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface ISystemConfigRepository : IBaseRepository<SystemConfig>
    {
        Task<object> GetConfigByCode(string code);
    }
}
