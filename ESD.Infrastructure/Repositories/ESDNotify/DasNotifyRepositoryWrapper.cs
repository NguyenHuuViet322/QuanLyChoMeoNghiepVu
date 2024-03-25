using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DASNotify
{
    public class DasNotifyRepositoryWrapper : IDasNotifyRepositoryWrapper
    {
        #region ctor
        private readonly ESDNotifyContext _repoContext;

        public DasNotifyRepositoryWrapper(ESDNotifyContext repositoryContext)
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

        #endregion

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
