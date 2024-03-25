using DASNotify.Application.Models.CustomModels;
using DASNotify.Domain.Models.DASNotify;
using System.Threading.Tasks;

namespace DASNotify.Application.Interfaces
{
    public interface ISendNotificationServices: IBaseMasterService<Notification>
    {
        Task<ServiceResult> PushToUsers(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, int UserImpactType = 0);
        Task<ServiceResult> PushToUsersPortal(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0);
    }
}
