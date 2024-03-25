using System;
using System.Collections.Generic;
using System.Text;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class TemplateParamRepository : DasBaseRepository<TemplateParam>, ITemplateParamRepository
    {
        public TemplateParamRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
