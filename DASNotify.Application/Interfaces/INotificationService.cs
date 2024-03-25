using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DASNotify.Application.Models.CustomModels;
using DASNotify.Application.Models.ViewModels;
using DASNotify.Domain.Models.DASNotify;

namespace DASNotify.Application.Interfaces
{
    public interface INotificationService : IBaseMasterService<Notification>
    {
        Task<ServiceResult> PushToUsers(VMSendNotification model);
        Task<ServiceResult> PushToUsersPortal(VMSendNotificationPortal model);
    }
}
