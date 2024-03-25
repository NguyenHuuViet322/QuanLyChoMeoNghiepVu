using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<bool> GetPaging();
        Task<bool> IsCodeExist(string code, int status, int idOrgan, int id = 0);
    }
}