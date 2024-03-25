using DASNotify.Infrastructure.HttpClientAccessors.Interfaces;
using Microsoft.AspNetCore.Http;
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

namespace DASNotify.Infrastructure.HttpClientAccessors.Implementations
{
    public class BaseHttpClient : IBaseHttpClient

    {
        private readonly HttpClient _httpClient;
        public BaseHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        #region Get
        public async Task<T> GetAsync<T>(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null, Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new()
        {

            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<string> GetAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion Get

        #region Post

        public async Task<T> PostAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
           Dictionary<string, string> headers = null,
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
           where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var dataStr = string.Empty;
            if (data != null)
            {
                if (!(data is string))
                {
                    dataStr = JsonConvert.SerializeObject(data);
                }
                else
                {
                    dataStr = data.ToString();
                }
            }
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            AppendHttpContent(message, dataStr);


            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> PostAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var dataStr = string.Empty;
            if (data != null)
            {
                if (!(data is string))
                {
                    dataStr = JsonConvert.SerializeObject(data);
                }
                else
                {
                    dataStr = data.ToString();
                }
            }
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            AppendHttpContent(message, dataStr);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                //if (response.IsSuccessStatusCode)
                //{
                //    return response.IsSuccessStatusCode;
                //}
                //else
                //{
                //    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                //}
                return response.IsSuccessStatusCode;
            }
        }

        #region Chưa giải quyết
        public Task<bool> PostHttpsAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null, Dictionary<string, string> headers = null, string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer, bool removeCharSet = false)
        {
            throw new NotImplementedException();
        }
        //public async Task<bool> PostHttpsAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
        //    Dictionary<string, string> headers = null,
        //    string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer, bool removeCharSet = false)
        //{
        //    HttpClientHandler clientHandler = new HttpClientHandler();
        //    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //    using (var client = new HttpClient(clientHandler))
        //    {

        //        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //        var requestUri = $"{domain}/{apiEndpoint}";

        //        var dataStr = string.Empty;

        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header
        //        AddRequestHeader(client, headers);

        //        if (data != null)
        //        {
        //            if (!(data is string))
        //            {
        //                dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
        //                {
        //                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //                    ContractResolver = new CamelCasePropertyNamesContractResolver()
        //                });
        //            }
        //            else
        //            {
        //                dataStr = data.ToString();
        //            }
        //        }

        //        await ProcessAccessTokenAsync(client, accessToken, accessTokenType);

        //        using (var contentBody = new StringContent(dataStr, Encoding.UTF8, "application/json"))
        //        {
        //            if (removeCharSet)
        //            {
        //                contentBody.Headers.ContentType.CharSet = "";
        //            }
        //            var response = await client.PostAsync(requestUri, contentBody);

        //            var result = response.IsSuccessStatusCode;
        //            if (!result)
        //            {
        //                _logger.LogError($"error on post async {JsonConvert.SerializeObject(response)}");
        //            }

        //            var content = await response.Content.ReadAsStringAsync();

        //            return result;
        //        }

        //    }
        //}
        #endregion

        public async Task<T> PostWithFormUrlEncoded<T>(string domain, string apiEndpoint,
            Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
            AppendAccessToken(message, accessToken, accessTokenType);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));  // ACCEPT header
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(body);
            message.Content = formUrlEncodedContent;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> PostWithFileAsync(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);

            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

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

            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                //if (response.IsSuccessStatusCode)
                //{
                //    return response.IsSuccessStatusCode;
                //}
                //else
                //{
                //    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                //}
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, IFormFile file, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

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
            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }

        }

        public async Task<T> PostWithFilePublicAsync<T>(string domain, string apiEndpoint, string fileControlName,
            IFormFile file,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null) where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AddRequestHeader(message, headers);

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
            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<T> PostWithFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, Stream file, string fileName, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            if (string.IsNullOrWhiteSpace(fileControlName))
            {
                fileControlName = "file";
            }
            MultipartFormDataContent form = ObjectToFormData(data);

            var streamContent = new StreamContent(file);
            form.Add(streamContent, fileControlName, fileName);

            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<T> PostWithMultiFileAsync<T>(string domain, string apiEndpoint,
            string fileControlName, List<IFormFile> files, string data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

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
            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }

        }

        public async Task<string> PostWithByteArrayAsync(string domain, string apiEndpoint,
            string fileName, string fileControlName, byte[] bytes, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            var data = new ByteArrayContent(bytes);
            data.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            MultipartFormDataContent form = new MultipartFormDataContent();

            if (string.IsNullOrWhiteSpace(fileControlName))
            {
                fileControlName = "file";
            }

            form.Add(data, fileControlName, fileName);

            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }
        public async Task<T> PostWithByteArrayAsync<T>(string domain, string apiEndpoint,
           string fileName, string fileControlName, byte[] bytes, Dictionary<string, string> requestParams = null,
           Dictionary<string, string> headers = null,
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            var data = new ByteArrayContent(bytes);
            data.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            MultipartFormDataContent form = new MultipartFormDataContent();

            if (string.IsNullOrWhiteSpace(fileControlName))
            {
                fileControlName = "file";
            }

            form.Add(data, fileControlName, fileName);

            message.Content = form;
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return await ProcessReponseAsync<T>(response);
            }
        }

        #endregion Post

        #region Put
        public async Task<T> PutAsync<T>(string domain, string apiEndpoint,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Put, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            var dataStr = string.Empty;
            if (data != null)
            {
                dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            AppendHttpContent(message, dataStr);

            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on put async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> PutAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Put, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            var dataStr = string.Empty;
            if (data != null)
            {
                dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }

            AppendHttpContent(message, dataStr);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region Delete
        public async Task<T> DeleteAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on Delete async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> DeleteAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// Thêm param vào URI
        /// </summary>
        /// <param name="requestParams"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private string PopulateRequestParam(Dictionary<string, string> requestParams, string requestUri)
        {
            if (requestParams != null && requestParams.Count > 0)
            {
                requestUri += "?";

                foreach (var item in requestParams)
                {
                    requestUri += $"{item.Key}={item.Value}&";
                }

                if (requestUri.EndsWith("&"))
                {
                    requestUri = requestUri.Substring(0, requestUri.Length - 1);
                }
            }

            return requestUri;
        }

        private void AddRequestHeader(HttpRequestMessage requestMessage, Dictionary<string, string> headers)
        {
            //client.Timeout = TimeSpan.FromSeconds(GetRequestTimeout());
            if (requestMessage != null && headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }
        }
        private void AppendHttpContent(HttpRequestMessage requestMessage, string httpContentStr)
        {
            //client.Timeout = TimeSpan.FromSeconds(GetRequestTimeout());
            if (requestMessage != null && !string.IsNullOrWhiteSpace(httpContentStr))
            {
                requestMessage.Content = new StringContent(httpContentStr, Encoding.UTF8, "application/json");
            }
        }

        /// <summary>
        /// Append Access Token vào HttpRequest
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenType"></param>
        private void AppendAccessToken(HttpRequestMessage requestMessage, string accessToken, AccessTokenType accessTokenType)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                if (accessTokenType == AccessTokenType.Bearer)
                {
                    //client.SetBearerToken(accessToken);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else if (accessTokenType == AccessTokenType.Basic)
                {
                    var arr = accessToken.Split("@#$");
                    var authenticationString = $"{arr[0]}:{arr[1]}";
                    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                }
                else
                {
                    requestMessage.Headers.TryAddWithoutValidation("Authorization", accessToken);
                }
            }
            else
            {
                //lấy access token mặc địch: HttpClien hiện tại đang static
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

                //_logger.LogError(responseStr);
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

        private int GetRequestTimeout()
        {
            var requestTimeoutStr = Environment.GetEnvironmentVariable("ASPNETCORE_REQUEST_TIMEOUT");
            var requestTimeout = 300;
            if (!string.IsNullOrWhiteSpace(requestTimeoutStr))
            {
                requestTimeout = int.Parse(requestTimeoutStr);
            }

            return requestTimeout;
        }

        private MultipartFormDataContent ObjectToFormData(object data)
        {
            if (data == null)
            {
                return new MultipartFormDataContent();
            }

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

        
        #endregion Private
    }
}
