using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IProfileRepository : IBaseRepository<Profile>
    {
        Task<bool> IsCodeExist(string code, int status, int id = 0);
    }
}