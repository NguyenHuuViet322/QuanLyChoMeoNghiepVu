using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IDocTypeRepository : IBaseRepository<DocType>
    {
        Task<bool> IsCodeExist(string code, int status, int id = 0);
    }
}