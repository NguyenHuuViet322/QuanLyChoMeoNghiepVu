using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IProfileCategoryServices
    {
        Task<VMTreeProfileCategory> GetTree(long id, int type);
    }
}