using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface ICategoryTypeRepository : IBaseRepository<CategoryType>
    {
        Task<bool> IsCodeExist(string code, int status, int id = 0, int idOrgan = 0);
    }
}