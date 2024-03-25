using ESD.Application.Models.CustomModels;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ISendNotificationServices
    {
        Task<ServiceResult> PushToUsers(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, int UserImpactType = 0, string accessToken = "");
        Task<ServiceResult> PushToUsersPortal(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, string accessToken = "");
    }
}
