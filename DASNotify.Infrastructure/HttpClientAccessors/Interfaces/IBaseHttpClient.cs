using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DASNotify.Infrastructure.HttpClientAccessors.Interfaces
{
    public interface IBaseHttpClient
    {
        Task<T> GetAsync<T>(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null, Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new();
        Task<byte[]> GetByteArrayAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<string> GetAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> PostAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
           Dictionary<string, string> headers = null,
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
           where T : new();
        Task<bool> PostAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<bool> PostHttpsAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer, bool removeCharSet = false);
        Task<T> PostWithFormUrlEncoded<T>(string domain, string apiEndpoint,
            Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<bool> PostWithFileAsync(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<T> PostWithFilePublicAsync<T>(string domain, string apiEndpoint, string fileControlName,
            IFormFile file,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null) where T : new();
        Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, Stream file, string fileName, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<T> PostWithMultiFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, List<IFormFile> files, string data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<string> PostWithByteArrayAsync(string domain, string apiEndpoint,
            string fileName, string fileControlName, byte[] bytes, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> PutAsync<T>(string domain, string apiEndpoint,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<bool> PutAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> DeleteAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<bool> DeleteAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> PostWithByteArrayAsync<T>(string domain, string apiEndpoint,
           string fileName, string fileControlName, byte[] bytes, Dictionary<string, string> requestParams = null,
           Dictionary<string, string> headers = null,
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new();
    }
}
