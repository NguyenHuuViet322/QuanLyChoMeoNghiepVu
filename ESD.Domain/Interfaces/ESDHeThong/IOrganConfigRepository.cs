using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IOrganConfigRepository : IBaseRepository<OrganConfig>
    {
        Task<object> GetConfigByCode(string code, int idOrgan = 0);
    }
}
