using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<List<string>> GetListEmailByIds(List<int> lstId, IDasRepositoryWrapper dasRepo);
        Task<string> GetEmailByUser(int userId, IDasRepositoryWrapper dasRepo);
        //Task<bool> IsEmailExist(string email);
    }
}