using ESD.Domain.Models.CustomModels;
using ESD.Infrastructure.HttpClientAccessors.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Infrastructure.HttpClientAccessors.Implementations
{
    public class LogHttpClient : ILogHttpClient
    {
        private readonly IBaseHttpClientFactory _clientFactory;
        private readonly string _apiLog;

        public LogHttpClient(IConfiguration configuration
            , IBaseHttpClientFactory factory)
        {
            _clientFactory = factory;
            _apiLog = configuration["LogDomain"];
            if (string.IsNullOrWhiteSpace(_apiLog))
            {
                throw new Exception("Not found domain Log Service, please check appsettings config");
            }
        }

        public async Task<bool> InserLog(LogInfo info, string accessToken)
        {
            var client = _clientFactory.Create();
            var response = await client.PostAsync(_apiLog, "api/Log/LogCrud", info, null, null, accessToken);
            return response;
        }

        public async Task<T> PostLog<T>(string apiUrl, object data = null, Dictionary<string, string> requestParams = null, string accessToken = "") where T : new()
        {
            var client = _clientFactory.Create();
            return await client.PostAsync<T>(_apiLog, apiUrl, data, requestParams, null, accessToken);
        }
    }
}
