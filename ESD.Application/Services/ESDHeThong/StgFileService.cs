using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ESD.Infrastructure.ContextAccessors;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Data.SqlClient;
using ESD.Utility;
using System.Security.AccessControl;
using System.Security.Principal;
using ESD.Application.Enums;
using ESD.Application.Constants;
using Microsoft.EntityFrameworkCore;
using ESD.Domain.Enums;

namespace ESD.Application.Services
{
    public class StgFileService : IStgFileService
    {
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _folderUpload;
        public StgFileService(IDasRepositoryWrapper dasRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration configuration, IUserPrincipalService userPrincipalService)
        {
            _dasRepo = dasRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _userPrincipalService = userPrincipalService;
            _folderUpload = GetUploadDirectory();
        }

        #region Create

        public async Task<ServiceResult> Upload(VMStgFile model)
        {
            try
            {
                if (model.File != null)
                {
                    var entity = _mapper.Map<VMStgFile, StgFile>(model);

                    // Data synthesis
                    if (string.IsNullOrEmpty(entity.FileName))  // Name
                    {
                        entity.FileName = model.File.FileName;
                    }
                    entity.Size = model.File.Length;    // Size

                    var fileExt = Path.GetExtension(entity.FileName);
                    var extAllowed = _configuration["AllowedFileUploadExts"];
                    if (string.IsNullOrEmpty(extAllowed) || extAllowed.Contains(fileExt.ToLower()))
                    {
                        // Save file
                        var physicalPath = await SavePhysicalFileAsync(model.File, model.FileName, model.FileExtension);
                        if (string.IsNullOrEmpty(physicalPath))
                        {
                            return new ServiceResultError("The file can not save");
                            //return null;
                        }
                        else
                        {
                            entity.PhysicalPath = physicalPath;
                            await _dasRepo.StgFile.InsertAsync(entity);
                            await _dasRepo.SaveAync();
                        }
                    }
                    else
                    {
                        return new ServiceResultError("The request is incorrect format");
                    }

                    return new ServiceResultSuccess("Upload success", entity);
                }
                else
                {
                    return new ServiceResultError("File not found");
                }
            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> UploadFileLarge(List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash)
        {
            if (resumableIdentifier == null || files == null || !files.Any())
            {
                return new ServiceResultError("Không tìm thấy tập tin", HttpStatusCode.NotFound);
            }

            if (resumableTotalSize > CheckFreeSpaceDisk())
            {
                return new ServiceResultError("Không đủ dung lượng bộ nhớ", HttpStatusCode.NotAcceptable);
            }

            try
            {
                var result = new ServiceResult();
                using (Stream ms = new MemoryStream())
                {
                    files.First().CopyTo(ms);

                    await StoreChunk(resumableIdentifier, resumableChunkNumber, ms, resumableFilename, resumableChunkHash);

                    if (AllChunksUploaded(resumableIdentifier, resumableChunkSize, resumableTotalSize, resumableFilename))
                    {
                        result = await MergeAllChunks(resumableIdentifier, resumableChunkSize, resumableTotalSize, resumableFilename);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<byte[]> GetPublicFileById(long fileId)
        {
            try
            {
                var stgFile = await _dasRepo.StgFile.GetAsync(fileId);
                if (stgFile == null)
                {
                    return null;
                }

                var output = GetFileByPhysicalPath(stgFile.PhysicalPath, stgFile.IsEncrypted);
                return output;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<FileBinaryInfo> GetFileById(long fileId)
        {
            try
            {
                var stgFile = await _dasRepo.StgFile.GetAsync(fileId);
                if (stgFile == null)
                {
                    return null;
                }

                var result = new FileBinaryInfo
                {
                    FileName = stgFile.FileName,
                    FileContents = GetFileByPhysicalPath(stgFile.PhysicalPath, stgFile.IsEncrypted)
                };
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ServiceResult> GetPhysicalPathById(long fileId)
        {
            try
            {
                var entity = await _dasRepo.StgFile.GetAsync(fileId);
                return new ServiceResultSuccess("Get file path by id success", entity.PhysicalPath);

            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> MarkFileTemp(long fileId)
        {
            try
            {
                var entity = await _dasRepo.StgFile.GetAsync(fileId);
                if (!entity.IsTemp)
                {
                    entity.IsTemp = true;
                }

                await _dasRepo.StgFile.UpdateAsync(entity);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Get file path by id success", entity);

            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        #endregion

        #region Functions

        private string GetUploadDirectory()
        {
            if (string.IsNullOrWhiteSpace(_hostingEnvironment.WebRootPath))
            {
                _hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            var folderPath = Path.Combine(_hostingEnvironment.WebRootPath,
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString()
            );
            return folderPath;
        }

        private async Task<string> SavePhysicalFileAsync(IFormFile file, string fileName = "", string ext = "")
        {
            try
            {
                var guidFileName = Guid.NewGuid().ToString();
                if (!Directory.Exists(_folderUpload))
                {
                    // If folder is not exists, create folder
                    Directory.CreateDirectory(_folderUpload);
                }

                var filePath = string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(ext)
                    ? Path.Combine(_folderUpload, $"{guidFileName}_{file.FileName}")
                    : Path.Combine(_folderUpload, $"{guidFileName}_{fileName}{ext}");

                if (_configuration["FileEncrypt"] == "true")
                {
                    // encrypt file
                }
                else
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }

                return filePath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private byte[] GetFileByPhysicalPath(string physicalPath, bool? isEncrypted = null)
        {
            if (File.Exists(physicalPath))
            {
                var fileContent = File.ReadAllBytes(physicalPath);
                bool isFileEncrypt = isEncrypted == null ? _configuration["FileEncrypt"] == "true" : isEncrypted.Value;
                if (isFileEncrypt)
                {
                    // encrypt file
                }

                return fileContent;
            }

            return null;
        }

        private long CheckFreeSpaceDisk()
        {
            var dir = new DirectoryInfo(_folderUpload);
            var dDrive = new DriveInfo(dir.Root.Name);

            // When the drive is accessible..
            if (dDrive.IsReady)
            {
                return dDrive.AvailableFreeSpace; //byte
            }
            return 1;
        }

        private async Task StoreChunk(string identifier, int chunkNumber, Stream inputStream, string fileName, string hash)
        {
            var extFile = Path.GetExtension(fileName);
            string path = Path.Combine(_folderUpload, identifier, chunkNumber.ToString());
            string chunkFile = Path.Combine(_folderUpload, identifier, chunkNumber.ToString(), fileName);
            string hashFile = null;
            if (!string.IsNullOrEmpty(hash))
            {
                hashFile = Path.Combine(_folderUpload, identifier, chunkNumber.ToString(), hash + extFile);
            }

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    //WriteErrorResponse(Resources.UploadDirectoryDoesnTExistAndCouldnTCreate);
                    //return false;
                }
            }

            Stream stream = null;
            Stream hashFileStream = null;
            try
            {
                stream = new FileStream(chunkFile, FileMode.Create);
                inputStream.Position = 0;
                await inputStream.CopyToAsync(stream, 16384);

                //Creating <hash number>.txt file just to prove hashing is working
                //using file is to release the lock after hash file is created
                if (!string.IsNullOrEmpty(hashFile))
                {
                    hashFileStream = File.Create(hashFile);
                }

            }
            catch (Exception ex)
            {
                //WriteErrorResponse(Resources.UnableToWriteOutFile);
                //return false;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
                if (hashFileStream != null)
                    hashFileStream.Close();
            }
        }

        private bool AllChunksUploaded(string identifier, long chunkSize, long totalSize, string fileName)
        {
            long noOfChunks = totalSize / chunkSize;

            for (int chunkNo = 1; chunkNo <= noOfChunks; chunkNo++)
            {
                string chunkFile = Path.Combine(_folderUpload, identifier.ToString(), chunkNo.ToString(), fileName);
                if (!File.Exists(chunkFile))
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<ServiceResult> MergeAllChunks(string identifier, long chunkSize, long totalSize, string fileName)
        {
            long noOfChunks = totalSize / chunkSize;
            string newFilePath = Path.Combine(_folderUpload, fileName);

            Stream stream = null;
            try
            {
                stream = new FileStream(newFilePath, FileMode.Create);

                for (int chunkNo = 1; chunkNo <= noOfChunks; chunkNo++)
                {
                    string chunkFile = Path.Combine(_folderUpload, identifier.ToString(), chunkNo.ToString(), fileName);

                    using (Stream fromStream = File.OpenRead(chunkFile))
                    {
                        await fromStream.CopyToAsync(stream, 16384);
                    }
                }

                var entity = new StgFile
                {
                    FileName = fileName,
                    PhysicalPath = Path.Combine(_folderUpload, fileName),
                    IsTemp = false,
                    Size = totalSize
                };

                await _dasRepo.StgFile.InsertAsync(entity);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Tải lên thành công", entity);
            }
            catch (Exception ex)
            {
                //WriteErrorResponse(Resources.UnableToWriteOutFile);
                return new ServiceResultError("Đã có lỗi xảy ra " + ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                // Delete all chunk files
                //for (int chunkNo = 1; chunkNo <= noOfChunks; chunkNo++)
                //{
                //    string chunkFolder = Path.Combine(_folderUpload, identifier, chunkNo.ToString());
                //    string chunkFile = Path.Combine(chunkFolder, fileName);
                //    if (File.Exists(chunkFile))
                //    {
                //        File.Delete(chunkFile);
                //    }

                //    if (Directory.Exists(chunkFolder))
                //    {
                //        Directory.Delete(chunkFolder, true);
                //    }
                //}

                // Delete all chunk files and folder after merge
                string path = Path.Combine(_folderUpload, identifier);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }

        #endregion
        #region Export Template
        public string GetFileSrc()
        {
            return _hostingEnvironment.WebRootPath + @"\templates\Template.docx";

        }
        public string GetFileDes()
        {
            return _hostingEnvironment.WebRootPath + @"\doc\Template.docx";
        }
        public string GetFileHtml()
        {
            return _hostingEnvironment.WebRootPath + @"\doc\doc.html";
        }
        public string GetFilePdf()
        {
            bool exists = System.IO.Directory.Exists(_hostingEnvironment.WebRootPath + @"\Download");

            if (!exists)
                System.IO.Directory.CreateDirectory(_hostingEnvironment.WebRootPath + @"\Download");

            return _hostingEnvironment.WebRootPath + @"\Download\Test.pdf";
        }
        private string GetExeConvertLocation()
        {
            return _hostingEnvironment.WebRootPath + @"\doc\soffice.exe";
        }
        public async Task<string> GetHtmlString(VMDeliveryRecord model, string fileSrc, string fileDes)
        {
            if (!File.Exists(fileDes))
                File.Create(fileDes);
            File.Copy(fileSrc, fileDes, true);
            var file = File.ReadAllText(GetFileHtml());


            model.PlanName = (await _dasRepo.Plan.GetAsync(model.IDPlan) ?? new Plan()).Name;
            var sendUser = await _dasRepo.User.GetAsync(model.IDSendUser) ?? new User();
            model.NameSendUser = sendUser.Name;
            model.SenderPosition = (await _dasRepo.Position.GetAsync(sendUser.IDPosition) ?? new ESD.Domain.Models.DAS.Position()).Name;

            var receiveUserUser = await _dasRepo.User.GetAsync(model.IDReceiveUser) ?? new User();
            model.NameReceiveUser = receiveUserUser.Name;
            model.ReceiverPosition = (await _dasRepo.Position.GetAsync(receiveUserUser.IDPosition) ?? new ESD.Domain.Models.DAS.Position()).Name;

            //Get text need to replace
            var dict = DictReplace(model);

            foreach (KeyValuePair<string, string> item in dict)
            {
                file = file.Replace(item.Key, item.Value);
            }
            return file;
        }
        public bool ReplaceDocxFile(string fileSrc, Dictionary<string, string> dictToReplace)
        {
            try
            {
                var doc = WordprocessingDocument.Open(fileSrc, true);
                var body = doc.MainDocumentPart.Document.Body;

                //Thay thế text trong docx file
                foreach (var text in body.Descendants<Text>())
                {
                    foreach (KeyValuePair<string, string> item in dictToReplace)
                    {
                        if (text.Text.Equals(dictToReplace.Keys))
                        {
                            text.Text = text.Text.Replace(item.Key, item.Value);
                        }
                    }
                }

                doc.MainDocumentPart.Document.Body = body;
                doc.Save();
                doc.Close();
            }
            catch (Exception ex)
            {
                return false;
                //return new ServiceResultError("Đã xảy ra lỗi: " + ex.Message);
            }
            return true;
        }
        private Dictionary<string, string> DictReplace(VMDeliveryRecord model)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("DocumentTitle", model.Title);
            dict.Add("Department", model.Department);
            dict.Add("CreateDate", "Ngày " + model.RecordCreateDate.Day.ToString() + " tháng " + model.RecordCreateDate.Month.ToString() + " năm " + model.RecordCreateDate.Year.ToString());
            dict.Add("PlanName", model.PlanName);
            dict.Add("NameOfSendUser", model.NameSendUser);
            dict.Add("RoleOfSendUser", model.SenderPosition);
            dict.Add("NameOfReceiveUser", model.NameReceiveUser);
            dict.Add("RoleOfReceiveUser", model.ReceiverPosition);
            dict.Add("DocumentName", model.DocumentName);
            dict.Add("DocumentTime", model.DocumentTime);
            dict.Add("TotalDocument", model.TotalDocument.ToString());
            dict.Add("TotalFile", model.TotalDocumentFile.ToString());
            dict.Add("SenderName", model.NameSendUser);
            dict.Add("ReceiverName", model.NameReceiveUser);
            return dict;
        }

        public async Task<ServiceResult> DownloadTemplate(long fileId)
        {
            try
            {
                var entity = await _dasRepo.StgFile.GetAsync(fileId);
                if (entity == null || entity.IsTemp)
                {
                    return new ServiceResultError("Template không tồn tại");
                }

                var wc = new WebClient();
                wc.DownloadFileAsync(new Uri(entity.PhysicalPath), Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\downloads\" + entity.FileName);
                return new ServiceResultSuccess("Download Template thành công. Check tại " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\downloads\" + entity.FileName);

            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> DownloadTemplateDeliveryRecord(VMDeliveryRecord model)
        {
            try
            {
                if (model.IDTemplate == 0)
                    return new ServiceResultError("Chưa chọn mẫu biên bản");
                var entity = await _dasRepo.StgFile.GetAsync(model.IDTemplate);
                if (entity == null || entity.IsTemp)
                {
                    return new ServiceResultError("Mẫu biên bản không tồn tại");
                }
                var url = _hostingEnvironment.WebRootPath + @"\Download\";
                File.Copy(entity.PhysicalPath, url + entity.FileName);
                var dict = DictReplace(model);
                var rs = ReplaceDocxFile(url + entity.FileName, dict);
                if (!rs)
                    return new ServiceResultError("Download thất bại");
                var wc = new WebClient();
                wc.DownloadFileAsync(new Uri(url), Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\downloads\" + entity.FileName);
                return new ServiceResultSuccess("Download biên bản thành công. Check tại " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\downloads\" + entity.FileName);
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Đã có lỗi xảy ra: " + ex.Message);
            }
        }

        #endregion
        #region Backup & Restore
        public async Task<ServiceResult> BackupData()
        {
            try
            {
                // TODO: Check Cannot open backup device Operating system error 5(Access is denied.)
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigUtils.GetConnectionString("DASContext");
                if (string.IsNullOrEmpty(con.Database))
                {
                    return new ServiceResultError("Sai cấu hình chuỗi kết nối!");
                }
                string path = _hostingEnvironment.WebRootPath + @"\database\backup\";
                var dirInfo = new DirectoryInfo(path);

                var security = new DirectorySecurity();
                var identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                var accessRule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
                security.AddAccessRule(accessRule);
                if (!Directory.Exists(path))
                {

                    dirInfo.Create(security);
                }
                else
                {
                    dirInfo.SetAccessControl(security);
                }

                var fileName = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".bak";
                //backup database
                await con.OpenAsync();
                string command = "backup database " + con.Database + " to disk='" + path + fileName + "'";
                var sqlcmd = new SqlCommand(command, con);
                await sqlcmd.ExecuteNonQueryAsync();
                await con.CloseAsync();
                var fileInfo = new FileInfo(path + fileName);
                //Insert data to StgFile
                var stg = new StgFile
                {
                    FileName = fileName,
                    FileType = (int)EnumFile.Type.Backup,
                    PhysicalPath = path + fileName,
                    Size = fileInfo.Length
                };

                await _dasRepo.StgFile.InsertAsync(stg);
                await _dasRepo.SaveAync();
                var wc = new WebClient();
                wc.DownloadFileAsync(new Uri(path + @"\" + fileName), Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\downloads\" + fileName);
                return new ServiceResultSuccess("Sao lưu dữ liệu thành công. Vui lòng kiểm tra thư mục " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\downloads", path + fileName);
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Sao lưu thất bại. Đã có lỗi xảy ra: " + ex.Message);
            }
        }

        public async Task<ServiceResult> RestoreData(VMStgFile model)
        {
            try
            {
                // TODO: Check Cannot open backup device Operating system error 5(Access is denied.)
                var stg = _dasRepo.StgFile.Get(model.ID);
                if (stg == null || stg.ID == 0 || stg.Status == 0)
                    return new ServiceResultError("Không tồn tại file dữ liệu này");
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigUtils.GetConnectionString("DASContext");
                if (string.IsNullOrEmpty(con.Database))
                {
                    return new ServiceResultError("Sai cấu hình chuỗi kết nối!");
                }

                //Dành cho modules restore chọn file backup từ máy tính
                //string path = _hostingEnvironment.WebRootPath + @"\database\restore";
                //if (!Directory.Exists(path))
                //{
                //    var security = new DirectorySecurity();
                //    var identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                //    var accessRule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow);
                //    security.AddAccessRule(accessRule);
                //    var dirInfo = new DirectoryInfo(path);
                //    dirInfo.Create(security);
                //}

                //Copy file backup to folder
                //var fileStream = new FileStream(path + @"\" + model.FileName, FileMode.Create);
                //await model.CopyToAsync(fileStream);
                //fileStream.Close();

                //restore database
                con.Open();
                string command = "use master; restore database " + con.Database + " from disk='" + stg.PhysicalPath + "' with replace";
                var sqlcmd = new SqlCommand(command, con);

                try
                {
                    sqlcmd.ExecuteNonQuery();
                    return new ServiceResultSuccess("Phục hồi dữ liệu thành công");
                }
                catch (Exception ex)
                {
                    return new ServiceResultError("Phục hồi dữ liệu thất bại. Đã có lỗi xảy ra: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Phục hồi dữ liệu thất bại. Đã có lỗi xảy ra: " + ex.Message);
            }
        }

        public byte[] GetDataByPath(string path)
        {
            return GetFileByPhysicalPath(path);
        }

        public async Task<PaginatedList<VMTemplate>> SearchListTemplateConditionPagging(TemplateCondition condition)
        {
            var temp = from template in _dasRepo.Template.GetAll()
                       where template.Status == (int)EnumCommon.Status.Active
                       && (condition.Keyword.IsEmpty() || template.Name.Contains(condition.Keyword)
                       || template.Code.Contains(condition.Keyword) || template.Description.Contains(condition.Keyword))
                       orderby template.CreateDate descending
                       select _mapper.Map<VMTemplate>(template);

            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize);
            if (!IsExisted(result))
                return null;
            var model = new PaginatedList<VMTemplate>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<List<StgFile>> GetActiveDB()
        {
            var rs = await _dasRepo.StgFile.GetAll().Where(x => x.Status == 1 && x.FileType == (int)EnumFile.Type.Backup).ToListAsync();
            return rs;
        }
        #endregion

        #region Private Function
        private bool IsExisted<T>(IEnumerable<T> t)
        {
            if (t == null || t.Count() == 0)
                return false;
            return true;
        }
        #endregion
    }
}
