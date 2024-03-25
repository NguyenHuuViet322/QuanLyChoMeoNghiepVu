using ESD.Domain.Interfaces;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.DASNotify;
using ESD.Infrastructure.Contexts;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DASNotify
{
    public class NotificationRepository : DasNotifyBaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ESDNotifyContext repositoryContext)
            : base(repositoryContext)
        {
        }

    }

}
