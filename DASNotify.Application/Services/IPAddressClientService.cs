using DASNotify.Application.Interfaces;
using DASNotify.Application.Models.CustomModels;
using System.Threading.Tasks;

namespace DASNotify.Application.Services
{
    public class IPAddressClientService : IIPAddressClientServices
    {
        private readonly IHttpClientService _httpClientService;

        public IPAddressClientService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }
        public async Task<ServiceResult> GetPublicIPAddress()
        {
            var apiAddress = "http://api.ipify.org";
            var apiUrl = "?format=json";
            var rs = await _httpClientService.GetAsync<IpValue>(apiAddress, apiUrl);
            return new ServiceResultSuccess(rs);
            //return await httpClientService.GetAsync<ServiceResult>(_apiFile, apiUrl, null, null, userPrincipalService.AccessToken);

        }
    }
    public class IpValue
    {
        public string IP { get; set; }
    }
}
