using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAS.FileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StgFileController : BaseController
    {
        private readonly ILogger<StgFileController> _logger;
        private readonly IStgFileService _stgFileService;
        public StgFileController(ILogger<StgFileController> logger, IStgFileService stgFileService)
        {
            _logger = logger;
            _stgFileService = stgFileService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return new ObjectResult("OK");
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] VMStgFile model)
        {
            var rs = await _stgFileService.Upload(model);
            return new ObjectResult(rs);
        }

        [HttpPost]
        [Route("public-upload")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicUpload([FromForm] VMStgFile model)
        {
            var rs = await _stgFileService.Upload(model);
            return new ObjectResult(rs);
        }

        [HttpPost]
        [Route("upload-large")]
        public async Task<IActionResult> UploadLarge([FromForm] List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash)
        {
            var rs = await _stgFileService.UploadFileLarge(files, resumableIdentifier, resumableChunkNumber, resumableChunkSize, resumableTotalSize, resumableFilename, resumableChunkHash);
            return new ObjectResult(rs);
        }

        [HttpGet("public/{fileId}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicFileById(long fileId)
        {
            var rs = await _stgFileService.GetPublicFileById(fileId);
            return new ObjectResult(rs);
        }

        [HttpGet("get-file-by-id/{fileId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFileById(long fileId)
        {
            var rs = await _stgFileService.GetFileById(fileId);
            //return File(rs.FileContents, rs.MimeType, rs.FileName);
            return new ObjectResult(rs);
        }

        [HttpGet("get-physical-path-by-id/{fileId}")]
        public async Task<IActionResult> GetPhysicalPathById(long fileId)
        {
            var rs = await _stgFileService.GetPhysicalPathById(fileId);
            return new ObjectResult(rs);
        }

        [HttpGet("mark-file-temp/{fileId}")]
        public async Task<IActionResult> MarkFileTemp(long fileId)
        {
            var rs = await _stgFileService.MarkFileTemp(fileId);
            return new ObjectResult(rs);
        }

        [HttpGet("{downloadHash}")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Download(long fileId)
        {
            return Ok();
        }

        [HttpPost]
        [Route("backupdata")]
        public async Task<IActionResult> BackupData()
        {
            var rs = await _stgFileService.BackupData();
            return new ObjectResult(rs);
        }

        [HttpPost]
        [Route("restoredata")]
        public async Task<IActionResult> RestoreData([FromForm] VMStgFile model)
        {
            var rs = await _stgFileService.RestoreData(model);
            return new ObjectResult(rs);
        }

        [HttpPost]
        [Route("getactivedb")]
        public async Task<IActionResult> GetActiveDB()
        {
            var rs = await _stgFileService.GetActiveDB();
            return new ObjectResult(rs);
        }

        [HttpGet("download-template/{fileId}")]
        public async Task<IActionResult> DownloadTemplate(long fileId)
        {
            var rs = await _stgFileService.DownloadTemplate(fileId);
            return new ObjectResult(rs);
        }

    }
}
