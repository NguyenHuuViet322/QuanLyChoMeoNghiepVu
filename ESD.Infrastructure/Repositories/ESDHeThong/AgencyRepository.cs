using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class AgencyRepository : DasBaseRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {

        }

        public async Task<List<string>> GetEmailByAgencyID(int AgencyID, IDasRepositoryWrapper dasRepo)
        {
            List<string> lstEmail = new List<string>();
            var Agency = await dasRepo.Agency.GetAsync(AgencyID);
            if (!IsExisted(Agency))
                return null;

            //get all list Organ
            var agencies = await dasRepo.Organ.GetAllListAsync(a => a.Status == 1);
            foreach (Organ Organ in agencies)
            {
                if (Organ.Email != null)
                {
                    lstEmail.Add(Organ.Email);
                }
            }

            return lstEmail;
        }

        private bool IsExisted(Agency Agency)
        {
            //Check Agency active
            if (Agency == null || Agency.ID == 0 || Agency.Status != 1)
                return false;
            return true;
        }
        //public async Task<bool> IsEmailExist(string email)
        //{
        //    return await DasContext.User.AnyAsync(s => s.Email == email);
        //}
    }
}