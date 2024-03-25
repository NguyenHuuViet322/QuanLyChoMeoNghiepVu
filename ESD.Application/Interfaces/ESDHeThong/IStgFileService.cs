using System.Collections.Generic;
using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using Microsoft.AspNetCore.Http;
using ESD.Domain.Models.DAS;

namespace ESD.Application.Interfaces
{
    public interface IStgFileService
    {
        Task<ServiceResult> Upload(VMStgFile model);

        Task<ServiceResult> UploadFileLarge(List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash);

        Task<byte[]> GetPublicFileById(long fileId);
        Task<FileBinaryInfo> GetFileById(long fileId);
        Task<ServiceResult> GetPhysicalPathById(long fileId);
        Task<ServiceResult> MarkFileTemp(long fileId);
        Task<string> GetHtmlString(VMDeliveryRecord model, string fileSrc, string fileDes);
        string GetFileSrc();
        string GetFileDes();
        string GetFileHtml();
        string GetFilePdf();
        //Task<ServiceResult> ReplaceDocxFile(string fileSrc, Dictionary<string, string> dictToReplace);
        Task<ServiceResult> BackupData();
        Task<ServiceResult> RestoreData(VMStgFile model);
        byte[] GetDataByPath(string path);
        Task<List<StgFile>> GetActiveDB();
        Task<PaginatedList<VMTemplate>> SearchListTemplateConditionPagging(TemplateCondition condition);
        Task<ServiceResult> DownloadTemplate(long id);
        Task<ServiceResult> DownloadTemplateDeliveryRecord(VMDeliveryRecord model);
    }
}
