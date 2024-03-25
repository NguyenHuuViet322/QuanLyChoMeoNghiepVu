using DASNotify.Domain.Interfaces.DASNotify;
using DASNotify.Infrastructure.Contexts;
using System.Threading.Tasks;

namespace DASNotify.Infrastructure.Repositories.DASNotify
{
    public class DasNotifyRepositoryWrapper : IDasNotifyRepositoryWrapper
    {
        #region ctor
        private readonly DASNotifyContext _repoContext;

        public DasNotifyRepositoryWrapper(DASNotifyContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
        #endregion

        #region properties
        private INotificationRepository _notification;

        public INotificationRepository Notification
        {
            get
            {
                if (_notification == null)
                {
                    _notification = new NotificationRepository(_repoContext);
                }
                return _notification;
            }
        }

        private INotificationPortalRepository _notificationPortal;

        public INotificationPortalRepository NotificationPortal
        {
            get
            {
                if (_notificationPortal == null)
                {
                    _notificationPortal = new NotificationPortalRepository(_repoContext);
                }
                return _notificationPortal;
            }
        }

        #endregion

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
