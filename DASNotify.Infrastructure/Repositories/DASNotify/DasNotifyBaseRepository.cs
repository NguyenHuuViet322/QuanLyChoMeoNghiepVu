using DASNotify.Infrastructure.Contexts;

namespace DASNotify.Infrastructure.Repositories.DASNotify
{
    public class DasNotifyBaseRepository<T> : BaseRepository<T> where T : class
    {
        protected DASNotifyContext DasNotifyContext { get; set; }

        public DasNotifyBaseRepository(DASNotifyContext repositoryContext) : base(repositoryContext)
        {
            DasNotifyContext = (DASNotifyContext)base.Context;
        }
    }
}
