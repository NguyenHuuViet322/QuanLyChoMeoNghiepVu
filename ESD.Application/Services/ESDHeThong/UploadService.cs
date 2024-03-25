using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility.CustomClass;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class UploadService : IUploadServices
    {
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly IStgFileClientService _fileClientService;
        private readonly IUserPrincipalService _iUserPrincipalService;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _folderUpload;

        public UploadService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IStgFileClientService fileClientService
            , IConfiguration configuration
            , IWebHostEnvironment hostingEnvironment
            , IUserPrincipalService userPrincipalService
            , IDistributedCache cache)
        {
            _dasRepo = dasRepository;
            _mapper = mapper;
            _fileClientService = fileClientService;
            _cache = cache;
            _configuration = configuration;
            _iUserPrincipalService = userPrincipalService;
            _hostingEnvironment = hostingEnvironment;
            _folderUpload = GetUploadDirectoryV2();
        }

         
        public VMFile Upload(IFormFile file)
        {

                var model = new VMFile()
                {
                    File = file,
                };

                var baseStorage = GetBaseStorage();
                if (model.File != null)
                {
                    var maxFileSize = _configuration["MaxFileSize"];
                    if (!string.IsNullOrEmpty(maxFileSize))
                    {
                        var maxSize = Convert.ToInt64(maxFileSize);
                        if (model.File.Length > maxSize) return model;
                    }
                    // Name
                    if (string.IsNullOrEmpty(model.FileName)) model.FileName = model.File.FileName;

                    // Size
                    model.Size = model.File.Length;

                    var fileExt = Path.GetExtension(model.FileName);
                    var extAllowed = _configuration.GetSection("AllowedFileUploadExts").Get<string[]>();
                    if (extAllowed == null ||  extAllowed.Contains(fileExt.ToLower()))
                    {
                        model.PhysicalPath = SavePhysicalFileAsync(model.File, model.FileName);
                        if (string.IsNullOrEmpty(model.PhysicalPath))
                        {
                            throw new LogicException("Không tim thấy vị trí lưu tệp");
                        }

                    }
                    else
                    {
                        throw new LogicException("Không hỗ trợ tải lên tệp tin có định dạng "+ fileExt);
                    }

                    return model;
                }
                else
                {
                    throw new LogicException("File lỗi");
                }
            
        }
        private string GetBaseStorage()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_hostingEnvironment.WebRootPath))
                {
                    _hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                return _hostingEnvironment.WebRootPath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetUploadDirectoryV2()
        {
            var baseStorage = GetBaseStorage();
            return Path.Combine(baseStorage,
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString());
        }

        private string SavePhysicalFileAsync(IFormFile file, string fileName = "", string ext = "")
        {
            try
            {
                // check fileName contain special char
                fileName = fileName.Replace(@"../", string.Empty);
                fileName = fileName.Replace(@"..\", string.Empty);
                var guidFileName = Guid.NewGuid().ToString();
                if (!Directory.Exists(_folderUpload))
                {
                    // If folder is not exists, create folder
                    Directory.CreateDirectory(_folderUpload);
                }

                var fullPath = string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(ext)
                    ? Path.Combine(_folderUpload, $"{guidFileName}_{file.FileName}")
                    : Path.Combine(_folderUpload, $"{guidFileName}_{fileName}{ext}");

                if (_configuration["FileEncrypt"] == "true")
                {
                    // encrypt file
                }
                else
                {
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        Task.Run(() => file.CopyToAsync(fileStream)).Wait();
                    }
                }

                var filePath = string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(ext)
                    ? Path.Combine(
                        DateTime.Now.Year.ToString(),
                        DateTime.Now.Month.ToString(),
                        DateTime.Now.Day.ToString(),
                        $"{guidFileName}_{file.FileName}")
                    : Path.Combine(
                        DateTime.Now.Year.ToString(),
                        DateTime.Now.Month.ToString(),
                        DateTime.Now.Day.ToString(),
                        $"{guidFileName}_{fileName}{ext}");
                return filePath;
            }
            catch (Exception)
            {
                return null;
            }
        }
         
    }
}
