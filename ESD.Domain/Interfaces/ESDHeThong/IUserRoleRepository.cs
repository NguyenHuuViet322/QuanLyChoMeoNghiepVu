using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IUserRoleRepository : IBaseRepository<UserRole>
    {
        //Task<bool> IsEmailExist(string email);
    }
}