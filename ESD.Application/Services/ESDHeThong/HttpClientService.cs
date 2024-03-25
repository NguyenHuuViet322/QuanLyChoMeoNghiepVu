using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public enum HttpClientMethod
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        HEAD = 4,
        OPTIONS = 5,
        PATCH = 6,
        MERGE = 7,
        COPY = 8
    }

    public enum AccessTokenType
    {
        None = 0,
        Bearer = 1,
        Basic = 2
    }

    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
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

        Task<T> PostWithFormUrlEncoded<T>(string domain, string apiEndpoint, Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null, string accessToken = "",
            AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();

        Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint, string fileControlName, Stream file,
            string fileName, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null, string accessToken = "",
            AccessTokenType accessTokenType = AccessTokenType.Bearer)
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

        Task<T> PostWithFilePublicAsync<T>(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null)
            where T : new();

        Task<T> PostWithMultiFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, List<IFormFile> files, string data = null,
            Dictionary<string, string> requestParams = null,
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
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new();

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

        Task<byte[]> Download(string downloadLink);

        string GetHeader(string key);

        Task<string> GetAccessTokenAsync(HttpContext httpContext);

        Task<string> GetAccessTokenAsync();
    }

    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HttpClientService> _logger;
        public HttpClientService(
            IHttpContextAccessor httpContextAccessor,
            //IMemoryCache memoryCache,
            IConfiguration configuration,
            ILogger<HttpClientService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            //_memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
        }

        #region Get

        public async Task<T> GetAsync<T>(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}";
                if (!string.IsNullOrWhiteSpace(apiEndpoint))
                {
                    requestUri += $"/{apiEndpoint}";
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
                AddRequestHeader(client, headers);
                requestUri = PopulateRequestParam(requestParams, requestUri);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                }
                else
                {
                    await AddAccessToken(client);
                }
                var response = await client.GetAsync(requestUri);
                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
                AddRequestHeader(client, headers);
                requestUri = PopulateRequestParam(requestParams, requestUri);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                }
                else
                {
                    await AddAccessToken(client);
                }

                var response = await client.GetByteArrayAsync(requestUri);
                return response;
            }
        }

        public async Task<string> GetAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
                AddRequestHeader(client, headers);
                requestUri = PopulateRequestParam(requestParams, requestUri);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                }
                else
                {
                    await AddAccessToken(client);
                }

                var response = await client.GetStringAsync(requestUri);
                return response;
            }

        }

        #endregion

        #region Post

        public async Task<T> PostAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
                AddRequestHeader(client, headers);
                requestUri = PopulateRequestParam(requestParams, requestUri);

                var dataStr = string.Empty;
                if (data != null)
                {
                    if (!(data is string))
                    {
                        dataStr = JsonConvert.SerializeObject(data);// new JsonSerializerSettings()
                        //{
                        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                        //});
                    }
                    else
                    {
                        dataStr = data.ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                }
                else
                {
                    await AddAccessToken(client);
                }

                var response = await client.PostAsync(requestUri, new StringContent(dataStr, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"error on post async {JsonConvert.SerializeObject(response)}");
                }

                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<bool> PostAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
                AddRequestHeader(client, headers);
                requestUri = PopulateRequestParam(requestParams, requestUri);

                var dataStr = string.Empty;
                if (data != null)
                {
                    if (!(data is string))
                    {
                        dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                    }
                    else
                    {
                        dataStr = data.ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                }
                else
                {
                    await AddAccessToken(client);
                }

                var response = await client.PostAsync(requestUri, new StringContent(dataStr, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"error on post async {JsonConvert.SerializeObject(response)}");
                }

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> PostHttpsAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer, bool removeCharSet = false)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var client = new HttpClient(clientHandler))
            {

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var requestUri = $"{domain}/{apiEndpoint}";

                var dataStr = string.Empty;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
                AddRequestHeader(client, headers);

                if (data != null)
                {
                    if (!(data is string))
                    {
                        dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                    }
                    else
                    {
                        dataStr = data.ToString();
                    }
                }

                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);

                using (var contentBody = new StringContent(dataStr, Encoding.UTF8, "application/json"))
                {
                    if (removeCharSet)
                    {
                        contentBody.Headers.ContentType.CharSet = "";
                    }
                    var response = await client.PostAsync(requestUri, contentBody);

                    var result = response.IsSuccessStatusCode;
                    if (!result)
                    {
                        _logger.LogError($"error on post async {JsonConvert.SerializeObject(response)}");
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    return result;
                }

            }
        }

        public async Task<T> PostWithFormUrlEncoded<T>(string domain, string apiEndpoint,
            Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));  // ACCEPT header

                Console.WriteLine("Response: " + domain + "#body 1:" + JsonConvert.SerializeObject(body));
                FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(body);
                Console.WriteLine("Response: " + domain + "#body 2:" + JsonConvert.SerializeObject(formUrlEncodedContent));
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.PostAsync(requestUri, formUrlEncodedContent);

                if (domain.Contains("/token") || domain.Contains("/userinfo"))
                {
                    var rs = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Response: " + domain + "#" + response.StatusCode + "#" + rs + "#body:" + JsonConvert.SerializeObject(body));
                }

                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<bool> PostWithFileAsync(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                AddRequestHeader(client, headers);

                if (string.IsNullOrWhiteSpace(fileControlName))
                {
                    fileControlName = "file";
                }

                MultipartFormDataContent form = ObjectToFormData(data);

                Stream stream = file.OpenReadStream();
                var streamContent = new StreamContent(stream);

                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"files\"",
                    FileName = "\"\""
                }; // the extra quotes are key here

                form.Add(streamContent, fileControlName, file.FileName);

                requestUri = PopulateRequestParam(requestParams, requestUri);

                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);

                var response = await client.PostAsync(requestUri, form);

                var result = response.IsSuccessStatusCode;
                if (!result)
                {
                    _logger.LogError("error on postwithfileasync", response);
                }

                return result;
            }
        }

        public async Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                AddRequestHeader(client, headers);

                if (string.IsNullOrWhiteSpace(fileControlName))
                {
                    fileControlName = "file";
                }
                MultipartFormDataContent form = ObjectToFormData(data);

                if (file != null)
                {
                    var streamContent = new StreamContent(file.OpenReadStream())
                    {
                        Headers =
                    {
                        ContentLength = file.Length,
                        ContentType = new MediaTypeHeaderValue(file.ContentType)
                    }
                    };

                    form.Add(streamContent, fileControlName, file.FileName);
                }

                requestUri = PopulateRequestParam(requestParams, requestUri);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.PostAsync(requestUri, form);

                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<T> PostWithFilePublicAsync<T>(string domain, string apiEndpoint, string fileControlName,
            IFormFile file,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null) where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                AddRequestHeader(client, headers);

                if (string.IsNullOrWhiteSpace(fileControlName))
                {
                    fileControlName = "file";
                }

                MultipartFormDataContent form = ObjectToFormData(data);

                if (file != null)
                {
                    var streamContent = new StreamContent(file.OpenReadStream())
                    {
                        Headers =
                        {
                            ContentLength = file.Length,
                            ContentType = new MediaTypeHeaderValue(file.ContentType)
                        }
                    };

                    form.Add(streamContent, fileControlName, file.FileName);
                }

                requestUri = PopulateRequestParam(requestParams, requestUri);
                var response = await client.PostAsync(requestUri, form);

                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, Stream file, string fileName, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                AddRequestHeader(client, headers);

                if (string.IsNullOrWhiteSpace(fileControlName))
                {
                    fileControlName = "file";
                }
                MultipartFormDataContent form = ObjectToFormData(data);

                var streamContent = new StreamContent(file);
                form.Add(streamContent, fileControlName, fileName);
                requestUri = PopulateRequestParam(requestParams, requestUri);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.PostAsync(requestUri, form);

                var responseStr = await response.Content.ReadAsStringAsync();

                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<T> PostWithMultiFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, List<IFormFile> files, string data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                AddRequestHeader(client, headers);

                if (string.IsNullOrWhiteSpace(fileControlName))
                {
                    fileControlName = "files";
                }

                var form = new MultipartFormDataContent();
                if (data != null)
                {
                    HttpContent stringContent = new StringContent(data);
                    form.Add(stringContent, "edoc");
                }

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        Stream stream = file.OpenReadStream();
                        var streamContent = new StreamContent(stream);
                        form.Add(streamContent, fileControlName, file.FileName);
                    }
                }

                requestUri = PopulateRequestParam(requestParams, requestUri);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.PostAsync(requestUri, form);

                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<string> PostWithByteArrayAsync(string domain, string apiEndpoint,
            string fileName, string fileControlName, byte[] bytes, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";

                AddRequestHeader(client, headers);

                var data = new ByteArrayContent(bytes);
                data.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                MultipartFormDataContent form = new MultipartFormDataContent();

                if (string.IsNullOrWhiteSpace(fileControlName)) fileControlName = "file";

                form.Add(data, fileControlName, fileName);

                requestUri = PopulateRequestParam(requestParams, requestUri);

                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);

                var response = await client.PostAsync(requestUri, form);

                var result = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogError("error on postwithfileasync", response);
                }

                return result;
            }
        }

        #endregion

        #region Put

        public async Task<T> PutAsync<T>(string domain, string apiEndpoint,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header

                AddRequestHeader(client, headers);

                var dataStr = string.Empty;
                if (data != null)
                {
                    dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                }

                requestUri = PopulateRequestParam(requestParams, requestUri);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.PutAsync(requestUri, new StringContent(dataStr, Encoding.UTF8, "application/json"));
                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<bool> PutAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header

                AddRequestHeader(client, headers);

                var dataStr = string.Empty;
                if (data != null)
                {
                    dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                }

                requestUri = PopulateRequestParam(requestParams, requestUri);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.PutAsync(requestUri, new StringContent(dataStr, Encoding.UTF8, "application/json"));

                var result = response.IsSuccessStatusCode;

                if (!result)
                {
                    _logger.LogError("error on put async", response);
                }

                return result;
            }
        }

        #endregion

        #region Delete

        public async Task<T> DeleteAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header

                AddRequestHeader(client, headers);
                requestUri = PopulateRequestParam(requestParams, requestUri);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                var response = await client.DeleteAsync(requestUri);
                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<bool> DeleteAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"{domain}/{apiEndpoint}";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header

                AddRequestHeader(client, headers);
                await ProcessAccessTokenAsync(client, accessToken, accessTokenType);
                requestUri = PopulateRequestParam(requestParams, requestUri);
                var response = await client.DeleteAsync(requestUri);
                var result = response.IsSuccessStatusCode;
                if (!result)
                {
                    _logger.LogError("error on deleteasync", response);
                }
                return result;
            }
        }

        #endregion

        public async Task<string> GetAccessTokenAsync()
        {
            await Task.Run(() =>
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return string.Empty;
                }

                var accessToken = Convert.ToString(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
                return accessToken != null && accessToken.Contains(" ") ? accessToken.Split(" ")[1] : accessToken;
            });
            return null;
        }

        public async Task<string> GetAccessTokenAsync(HttpContext httpContext)
        {
            if (httpContext == null)
                return string.Empty;

            return await httpContext.GetTokenAsync("access_token");
        }

        public async Task<byte[]> Download(string downloadLink)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetByteArrayAsync(downloadLink);
                return response;
            }
        }

        public string GetHeader(string key)
        {
            if (_httpContextAccessor.HttpContext == null) return string.Empty;
            return _httpContextAccessor.HttpContext?.Request.Headers[key];
        }

        #region Functions

        private int GetRequestTimeout()
        {
            var requestTimeoutStr = Environment.GetEnvironmentVariable("ASPNETCORE_REQUEST_TIMEOUT");
            var requestTimeout = 300;
            if (!string.IsNullOrWhiteSpace(requestTimeoutStr)) requestTimeout = int.Parse(requestTimeoutStr);
            return requestTimeout;
        }

        private void AddRequestHeader(HttpClient client, Dictionary<string, string> headers)
        {
            client.Timeout = TimeSpan.FromSeconds(GetRequestTimeout());
            if (client != null && headers != null)
            {
                foreach (var item in headers)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
        }

        private MultipartFormDataContent ObjectToFormData(object data)
        {
            if (data == null)
                return new MultipartFormDataContent();

            var form = new MultipartFormDataContent();
            var properties = data.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(data) != null && property.PropertyType != typeof(IFormFile))
                {
                    form.Add(new StringContent(property.GetValue(data)?.ToString()), property.Name);
                }
            }

            return form;
        }

        private string PopulateRequestParam(Dictionary<string, string> requestParams, string requestUri)
        {
            if (requestParams != null && requestParams.Count > 0)
            {
                requestUri += "?";

                foreach (var item in requestParams)
                {
                    requestUri += $"{item.Key}={item.Value}&";
                }

                if (requestUri.EndsWith("&")) requestUri = requestUri.Substring(0, requestUri.Length - 1);
            }

            return requestUri;
        }

        /// <summary>
        /// Add access token get from header client 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task AddAccessToken(HttpClient client)
        {
            var accessTokenFromHeader = await GetAccessTokenAsync();
            string token;
            if (!string.IsNullOrWhiteSpace(accessTokenFromHeader) && (accessTokenFromHeader != "null"))
            {
                token = accessTokenFromHeader;
            }
            else
            {
                token = null;
            }
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Add access token to header client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenType"></param>
        /// <returns></returns>
        private async Task ProcessAccessTokenAsync(HttpClient client, string accessToken, AccessTokenType accessTokenType)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                if (accessTokenType == AccessTokenType.Bearer)
                {
                    //client.SetBearerToken(accessToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else if (accessTokenType == AccessTokenType.Basic)
                {
                    var arr = accessToken.Split("@#$");
                    //client.SetBasicAuthentication(arr[0], arr[1]);

                    var authenticationString = $"{arr[0]}:{arr[1]}";
                    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                }
                else
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", accessToken);
                }
            }
            else
            {
                await AddAccessToken(client);
            }
        }

        private async Task<T> ProcessReponseAsync<T>(HttpResponseMessage response) where T : new()
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    var newInstance = Activator.CreateInstance(typeof(T));
                    SetObjectValue(newInstance, "Success", true);
                    return (T)newInstance;

                }
                else
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(content);
                    }
                    catch (Exception)
                    {
                        return (T)Convert.ChangeType(content, typeof(T));
                    }
                }
            }
            else
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response error" + response.StatusCode + "#" + content);
                    var op = JsonConvert.DeserializeObject<T>(content);
                    if (op == null)
                    {
                        var newInstance = Activator.CreateInstance(typeof(T));
                        SetObjectValue(newInstance, "Success", false);

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            SetObjectValue(newInstance, "Error", "ERROR_UNAUTHORIZED");
                            SetObjectValue(newInstance, "Message", "Xác thực thất bại");
                        }
                        else if (response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            SetObjectValue(newInstance, "Error", "ERROR_FORBIDDEN");
                            SetObjectValue(newInstance, "Message", "Không có quyền truy cập API");
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            SetObjectValue(newInstance, "Error", "ERROR_NOT_FOUND");
                            SetObjectValue(newInstance, "Message", "Không tìm thấy API");
                        }

                        return (T)newInstance;
                    }
                }
                catch (Exception)
                {
                    var newInstance = Activator.CreateInstance(typeof(T));
                    SetObjectValue(newInstance, "Success", false);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        SetObjectValue(newInstance, "Error", "ERROR_UNAUTHORIZED");
                        SetObjectValue(newInstance, "Message", "Xác thực thất bại");
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        SetObjectValue(newInstance, "Error", "ERROR_FORBIDDEN");
                        SetObjectValue(newInstance, "Message", "Không có quyền truy cập API");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        SetObjectValue(newInstance, "Error", "ERROR_NOT_FOUND");
                        SetObjectValue(newInstance, "Message", "Không tìm thấy API");
                    }

                    return (T)newInstance;
                }

                var responseStr = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                _logger.LogError(responseStr);
            }

            return default(T);
        }

        private void SetObjectValue(object obj, string prop, object value)
        {
            try
            {
                if (obj != null && obj.GetType().GetProperty(prop) != null)
                {
                    obj.GetType().GetProperty(prop)?.SetValue(obj, value);
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        #endregion
    }


}
