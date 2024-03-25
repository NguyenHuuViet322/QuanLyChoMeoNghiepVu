using ESD.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Infrastructure.Repositories.ESDNghiepVu
{
    public class ESDNghiepVuRepository<T> : BaseRepository<T> where T : class
    {
        protected ESDNGHIEPVUContext DASKTNNContext { get; set; }

        public ESDNghiepVuRepository(ESDNGHIEPVUContext repositoryContext) : base(repositoryContext)
        {
            DASKTNNContext = (ESDNGHIEPVUContext)base.Context;
        }
    }
}