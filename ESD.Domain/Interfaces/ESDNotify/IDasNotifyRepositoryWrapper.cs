using ESD.Domain.Interfaces.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DASNotify
{
    public interface IDasNotifyRepositoryWrapper
    {
        INotificationRepository Notification { get; }
        Task SaveAync();
    }
}
