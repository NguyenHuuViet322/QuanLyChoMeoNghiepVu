using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IPlanProfileRepository : IBaseRepository<PlanProfile>
    {
        Task<bool> IsCodeExist(string code, int status, int id = 0);
    }
}