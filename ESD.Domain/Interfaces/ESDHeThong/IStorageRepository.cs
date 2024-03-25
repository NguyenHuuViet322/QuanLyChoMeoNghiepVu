using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IStorageRepository : IBaseRepository<Storage>
    {
        Task<bool> IsCodeExist(string email, int id = 0);
    }
}
