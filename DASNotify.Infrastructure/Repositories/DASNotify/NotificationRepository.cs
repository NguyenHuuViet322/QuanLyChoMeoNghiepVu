using DASNotify.Domain.Interfaces.DASNotify;
using DASNotify.Domain.Models.DASNotify;
using DASNotify.Infrastructure.Contexts;

namespace DASNotify.Infrastructure.Repositories.DASNotify
{
    public class NotificationRepository : DasNotifyBaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(DASNotifyContext repositoryContext)
            : base(repositoryContext)
        {
        }

    }

}
