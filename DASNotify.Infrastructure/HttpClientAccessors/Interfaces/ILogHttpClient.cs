using DASNotify.Domain.Models.CustomModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DASNotify.Infrastructure.HttpClientAccessors.Interfaces
{
    public interface ILogHttpClient
    {
        Task<bool> InserLog(LogInfo info, string accessToken);
        Task<T> PostLog<T>(string apiUrl, object data = null, Dictionary<string, string> requestParams = null, string accessToken = "") where T : new();
    }
}
