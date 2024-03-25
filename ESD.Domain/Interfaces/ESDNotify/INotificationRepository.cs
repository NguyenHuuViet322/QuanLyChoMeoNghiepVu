using ESD.Domain.Models.DAS;
using ESD.Domain.Models.DASNotify;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DASNotify
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
    }
}
