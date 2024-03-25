using System.Threading.Tasks;
using DASNotify.Application.Models.CustomModels;

namespace DASNotify.Application.Interfaces
{
    public interface IIPAddressClientServices
    {
        Task<ServiceResult> GetPublicIPAddress();
    }
}
