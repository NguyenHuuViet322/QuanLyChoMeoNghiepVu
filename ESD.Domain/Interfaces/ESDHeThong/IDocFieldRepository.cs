using ESD.Domain.Models.DAS;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IDocFieldRepository : IBaseRepository<DocField>
    {
        Task<bool> IsCodeExist(string code, int status, int id = 0);
    }
}