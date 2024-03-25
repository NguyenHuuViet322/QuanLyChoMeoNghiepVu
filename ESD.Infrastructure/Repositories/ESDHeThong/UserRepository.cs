using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class UserRepository : DasBaseRepository<User>, IUserRepository
    {
        public UserRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<string> GetEmailByUser(int userId, IDasRepositoryWrapper dasRepo)
        {
            return await dasRepo.User.GetEmailByUser(userId, dasRepo);
        }

        public async Task<List<string>> GetListEmailByIds(List<int> lstId, IDasRepositoryWrapper dasRepo)
        {
            List<string> lstEmail = new List<string>();
            foreach (int userID in lstId)
            {
                lstEmail.Add(await dasRepo.User.GetEmailByUser(userID, dasRepo));
            }
            return lstEmail;
        }

        //public async Task<bool> IsEmailExist(string email)
        //{
        //    return await DasContext.User.AnyAsync(s => s.Email == email);
        //}
    }
}