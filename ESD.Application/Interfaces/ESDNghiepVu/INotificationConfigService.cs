using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels.DasKTNN;
using System.Collections;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces.DasKTNN
{
    public interface INotificationConfigService
    {
        Task<VMNotificationConfig> SearchByCondition(NotificationConfigCondition condition);
        Task<ServiceResult> Update(Hashtable Data);
    }
}
