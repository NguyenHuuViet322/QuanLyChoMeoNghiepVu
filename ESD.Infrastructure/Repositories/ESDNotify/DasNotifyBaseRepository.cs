using ESD.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Infrastructure.Repositories.DASNotify
{
    public class DasNotifyBaseRepository<T> : BaseRepository<T> where T : class
    {
        protected ESDNotifyContext DasNotifyContext { get; set; }

        public DasNotifyBaseRepository(ESDNotifyContext repositoryContext) : base(repositoryContext)
        {
            DasNotifyContext = (ESDNotifyContext)base.Context;
        }
    }
}
