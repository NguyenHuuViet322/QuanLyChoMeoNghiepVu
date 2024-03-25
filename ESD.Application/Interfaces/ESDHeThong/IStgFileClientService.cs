using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IStgFileClientService
    {
        Task<ServiceResult> GetPublicFileById(long id);

        Task<ServiceResult> GetPhysicalPathById(long id);
        Task<ServiceResult> GetFileById(long id);
        Task<ServiceResult> MarkFileTemp(long id);

        Task<ServiceResult> Upload(VMStgFile model);

        Task<ServiceResult> UploadPublic(VMStgFile model);

        Task<ServiceResult> UploadLargeFile(List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash);
        Task<ServiceResult> BackupData();
        Task<ServiceResult> RestoreData(VMStgFile model);
        Task<ServiceResult> GetActiveDB();
    }
}
