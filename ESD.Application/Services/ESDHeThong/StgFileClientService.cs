using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESD.Infrastructure.ContextAccessors;
using ESD.Domain.Models.DAS;

namespace ESD.Application.Services
{
    public class StgFileClientService : IStgFileClientService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly string _apiFile;

        public StgFileClientService(
            IConfiguration configuration,
            IHttpClientService httpClientService, IUserPrincipalService userPrincipalService)
        {
            _httpClientService = httpClientService;
            _userPrincipalService = userPrincipalService;
            _apiFile = configuration["FileDomain"];

            if (string.IsNullOrWhiteSpace(_apiFile))
                throw new Exception("Not found domain File Service, please check appsettings config");
        }

        public async Task<ServiceResult> GetPublicFileById(long id)
        {
            try
            {
                var apiUrl = $"api/stgFile/public/{id}/download";
                var response = await _httpClientService.GetByteArrayAsync(_apiFile, apiUrl, null, null, _userPrincipalService.AccessToken);
                return new ServiceResultSuccess("Upload file success", response);
            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> GetPhysicalPathById(long id)
        {
            var apiUrl = $"api/stgFile/get-physical-path-by-id/{id}";
            return await _httpClientService.GetAsync<ServiceResult>(_apiFile, apiUrl, null, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> MarkFileTemp(long id)
        {
            var apiUrl = $"api/stgFile/mark-file-temp/{id}";
            return await _httpClientService.GetAsync<ServiceResult>(_apiFile, apiUrl, null, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> Upload(VMStgFile model)
        {
            var apiUrl = "api/stgFile/upload";
            return await _httpClientService.PostWithFileAsync<ServiceResult>(_apiFile, apiUrl, null, model.File, model, null, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> UploadPublic(VMStgFile model)
        {
            var apiUrl = "api/stgFile/public-upload";
            return await _httpClientService.PostWithFilePublicAsync<ServiceResult>(_apiFile, apiUrl, null, model.File, model);
        }

        public async Task<ServiceResult> UploadLargeFile(List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash)
        {
            var apiUrl = "api/stgFile/upload-large";
            var requestParam = new Dictionary<string, string>
            {
                {"resumableIdentifier", resumableIdentifier},
                {"resumableChunkNumber", resumableChunkNumber.ToString()},
                {"resumableChunkSize", resumableChunkSize.ToString()},
                {"resumableTotalSize", resumableTotalSize.ToString()},
                {"resumableFilename", resumableFilename},
                {"resumableChunkHash", resumableChunkHash}
            };
            return await _httpClientService.PostWithMultiFileAsync<ServiceResult>(_apiFile, apiUrl, null, files, null, requestParam, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> BackupData()
        {
            var apiUrl = "api/stgFile/backupdata";
            return await _httpClientService.PostAsync<ServiceResult>(_apiFile, apiUrl, null, null, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> RestoreData(VMStgFile model)
        {
            var apiUrl = "api/stgFile/restoredata";
            return await _httpClientService.PostWithFileAsync<ServiceResult>(_apiFile, apiUrl, null, null, model, null, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> GetActiveDB()
        {
            try
            {
                var apiUrl = "api/stgFile/getactivedb";
                var result = await _httpClientService.PostAsync<List<StgFile>>(_apiFile, apiUrl, null, null, null, _userPrincipalService.AccessToken);

                return new ServiceResultSuccess("Success", result);
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Đã có lỗi xảy ra: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DownloadTemplate(long id)
        {
            var apiUrl = $"api/stgFile/download-template/{id}";
            return await _httpClientService.GetAsync<ServiceResult>(_apiFile, apiUrl, null, null, _userPrincipalService.AccessToken);
        }

        public async Task<ServiceResult> GetFileById(long id)
        {
            try
            {
                var apiUrl = $"api/stgFile/get-file-by-id/{id}";
                var result = await _httpClientService.GetAsync(_apiFile, apiUrl, null, null, _userPrincipalService.AccessToken);

                return new ServiceResultSuccess("Success", result);
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Đã có lỗi xảy ra: " + ex.Message);
            }
        }
    }
}
