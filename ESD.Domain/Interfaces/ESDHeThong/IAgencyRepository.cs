using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IAgencyRepository : IBaseRepository<Agency>
    {
        //Task<bool> IsEmailExist(string email);
        Task<List<string>> GetEmailByAgencyID(int AgencyID, IDasRepositoryWrapper dasRepo);
    }
}