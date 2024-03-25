using System.Threading.Tasks;

namespace DASNotify.Domain.Interfaces.DASNotify
{
    public interface IDasNotifyRepositoryWrapper
    {
        INotificationRepository Notification { get; }
        INotificationPortalRepository NotificationPortal { get; }
        Task SaveAync();
    }
}
