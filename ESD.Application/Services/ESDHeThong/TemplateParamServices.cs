using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;

namespace ESD.Application.Services
{
    public class TemplateParamServices : BaseMasterService, ITemplateParamServices
    {
        #region Ctor
        public TemplateParamServices(IDasRepositoryWrapper dasRepository) : base(dasRepository)
        {

        }
        #endregion
        public Task<ServiceResult> Create(TemplateParam model)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<TemplateParam> Get(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TemplateParam>> Gets()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Update(TemplateParam model)
        {
            throw new NotImplementedException();
        }
    }
}
