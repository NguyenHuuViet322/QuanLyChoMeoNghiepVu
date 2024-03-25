using DASNotify.Domain.Interfaces.DASNotify;
using DASNotify.Domain.Models.DASNotify;
using DASNotify.Infrastructure.Contexts;

namespace DASNotify.Infrastructure.Repositories.DASNotify
{
    public class NotificationPortalRepository : DasNotifyBaseRepository<NotificationPortal>, INotificationPortalRepository
    {
        public NotificationPortalRepository(DASNotifyContext repositoryContext)
            : base(repositoryContext)
        {
        }

    }

}
