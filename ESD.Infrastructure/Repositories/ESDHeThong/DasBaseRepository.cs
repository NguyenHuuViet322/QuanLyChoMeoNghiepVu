using ESD.Domain.Interfaces.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DasBaseRepository<T> : BaseRepository<T> where T : class
    {
        protected ESDContext DasContext { get; set; }
        public DasBaseRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
            DasContext = (ESDContext)base.Context;
        }
    }
}