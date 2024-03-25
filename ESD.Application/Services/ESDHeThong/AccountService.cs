using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Application.Constants;
using Newtonsoft.Json;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Domain.Enums;
using Microsoft.AspNetCore.Http;
using ESD.Application.Enums;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;
using ESD.Infrastructure.ContextAccessors;

namespace ESD.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly IStgFileClientService _fileClientService;
        private readonly IUserPrincipalService _iUserPrincipalService;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _folderUpload;

        public AccountService(IDasRepositoryWrapper dasRepository
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

        public async Task<ServiceResult> Authenticate(VMLogin model)
        {            
            int maxFailShortBlock = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_LOGIN_FAILD_SHORT_BLOCK))?? "").ToString(), out maxFailShortBlock) ? maxFailShortBlock : 5;
            int maxFailLongBlock = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_LOGIN_FAILD_LONG_BLOCK))??"").ToString(), out maxFailLongBlock) ? maxFailLongBlock : 15;            
            int timeBetweenLock = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.TIME_LOCK_BETWEEN_LOGIN))??"").ToString(), out timeBetweenLock) ? timeBetweenLock : 5;
           
            Dictionary<string, int> countByCache;
            countByCache = (await _cache.GetCacheValueAsync<Dictionary<string, int>>(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName)));

            var user = await _dasRepo.User.SingleOrDefaultAsync(x => x.AccountName == model.UserName && x.Password == StringUltils.Md5Encryption(model.Password) && x.Status == (int)EnumCommon.Status.Active);
            if (user == null || !string.Equals(user.AccountName, model.UserName, StringComparison.Ordinal)) //Đăng nhập sai & hoặc account không trùng
            {
                var existUser = await _dasRepo.User.SingleOrDefaultAsync(x => x.AccountName == model.UserName);
                if (existUser != null && string.Equals(existUser.AccountName, model.UserName, StringComparison.Ordinal))
                {   //Check Status
                    if (existUser.Status == (int)EnumCommon.Status.InActive)
                    {
                        return new ServiceResultError("Tài khoản của bạn đã bị khóa");
                    }

                    //Check longBlock
                    existUser.CountLoginFail++;
                    if (existUser.CountLoginFail >= maxFailLongBlock)
                    {
                        if (countByCache != null)
                        {
                            await _cache.RemoveAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName));
                        }

                        existUser.Status = (int)EnumCommon.Status.InActive;
                        await _dasRepo.User.UpdateAsync(existUser);
                        await _dasRepo.SaveAync();
                        return new ServiceResultError($"Bạn đã đăng nhập sai quá {maxFailLongBlock} lần liên tiếp, tài khoản của bạn đã bị khóa");
                    }
                    else
                    {
                        await _dasRepo.User.UpdateAsync(existUser);
                        await _dasRepo.SaveAync();
                    }
                    //Check shortBlock
                    if (countByCache != null)
                    {
                        countByCache[existUser.AccountName] = countByCache[existUser.AccountName] + 1;
                        if (countByCache[existUser.AccountName] >= maxFailShortBlock)
                        {
                            await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName), countByCache, timeBetweenLock * 60);
                            return new ServiceResultError($"Bạn đã đăng nhập sai quá {maxFailShortBlock} lần liên tiếp, thử lại trong vòng {timeBetweenLock} phút");
                        }
                        await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName), countByCache, timeBetweenLock * 60);
                    }
                    else
                    {
                        var countFail = new Dictionary<string, int> {
                            { model.UserName ,1 }
                        };
                        await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName), countFail, timeBetweenLock * 60);
                    }

                    return new ServiceResultError("Thông tin tài khoản không chính xác");
                }
                else
                {
                    return new ServiceResultError("Thông tin tài khoản không chính xác");
                }
            }


            if (countByCache != null)
            {
                if (user == null )
                    countByCache[user.AccountName] = countByCache[user.AccountName] + 1;
                if (countByCache[user.AccountName] >= maxFailShortBlock)
                {
                    await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName), countByCache, timeBetweenLock * 60);
                    //await _cache.RemoveAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName));
                    return new ServiceResultError($"Bạn đã đăng nhập sai quá {maxFailShortBlock} lần liên tiếp, thử lại trong vòng {timeBetweenLock} phút");
                }
                if (user == null )
                    await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName), countByCache, timeBetweenLock * 60);
            }
            //Thành công => xóa cache nếu có && update lại count
            if (countByCache != null)
            {
                await _cache.RemoveAsync(string.Concat(CacheConst.USER_COUNT_LOGIN_FAIL, model.UserName));
            }
            if (user.CountLoginFail != 0)
            {
                user.CountLoginFail = 0;
                await _dasRepo.User.UpdateAsync(user);
                await _dasRepo.SaveAync();
            }
            return new ServiceResultSuccess(user);
        }

        public Task<bool> Register(VMRegister model)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> ChangePassword(VMAccount vmAccount)
        {
            var user = await _dasRepo.User.GetAsync(vmAccount.userID);
            Dictionary<string, int> countByCache;
            countByCache = (await _cache.GetCacheValueAsync<Dictionary<string, int>>(string.Concat(CacheConst.USER_CHANGE_PWD_FAIL, user.AccountName)));
            if (!user.Password.Equals(StringUltils.Md5Encryption(vmAccount.OldPassword)))
            {
                var maxChangePWDFaild = (long)(await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_CHANGE_PWD_FAILD));
                if (countByCache != null)
                {
                    countByCache[user.AccountName] = countByCache[user.AccountName] + 1;
                    await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_CHANGE_PWD_FAIL, user.AccountName), countByCache, 300);
                    if (countByCache[user.AccountName] >= maxChangePWDFaild)
                    {
                        return new ServiceResultError($"Bạn đã nhập sai quá {maxChangePWDFaild} lần liên tiếp, thử lại trong vòng 5 phút");
                    }
                }
                else
                {
                    var countFail = new Dictionary<string, int> {
                        { user.AccountName ,1 }
                    };
                    await _cache.SetCacheValueAsync(string.Concat(CacheConst.USER_CHANGE_PWD_FAIL, user.AccountName), countFail, 300);
                }

                return new ServiceResultError("Mật khẩu hiện tại không đúng");
            }
            if (countByCache != null)
            {
                await _cache.RemoveAsync(string.Concat(CacheConst.USER_CHANGE_PWD_FAIL, user.AccountName));
            }
            user.Password = StringUltils.Md5Encryption(vmAccount.Password);
            await _dasRepo.User.UpdateAsync(user);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật mật khẩu thành công");
        }

        public async Task<int> GetUserIDByEmail(string accountName)
        {
            var user = await _dasRepo.User.GetAllListAsync(model => (model.Email == accountName));
            if (!IsExisted(user))
            {
                return 0;
            }
            else
            {
                return user.FirstOrDefault().ID;
            }

        }

        public async Task<ServiceResult> SendEmailWithEmailAddress(string body, string title, string email, string emailType)
        {
            var request = await _dasRepo.Email.SendEmailWithEmailAddress(body, title, email, emailType);
            if (!request.IsSuccess)
            {
                return new ServiceResultError("Gửi email thất bại", request);
            }

            return new ServiceResultSuccess();
        }

        public async Task<VMUserProfile> GetCurrentUser(int id)
        {
            var temp = from u in _dasRepo.User.GetAll().Where(u => u.ID == id && u.Status == (int)EnumCommon.Status.Active)
                       from a in _dasRepo.Organ.GetAll().Where(a => a.ID == u.IDOrgan && a.Status == (int)EnumCommon.Status.Active).DefaultIfEmpty()
                       from d in _dasRepo.Agency.GetAll().Where(d => d.ID == u.IDAgency && d.Status == (int)EnumCommon.Status.Active).DefaultIfEmpty()
                       from p in _dasRepo.Position.GetAll().Where(p => p.ID == u.IDPosition && p.Status == (int)EnumCommon.Status.Active).DefaultIfEmpty()
                       select new VMUserProfile
                       {
                           ID = u.ID,
                           AccountName = u.AccountName,
                           Name = u.Name,
                           IdentityNumber = u.IdentityNumber,
                           Email = u.Email,
                           Phone = u.Phone,
                           Address = u.Address,
                           Avatar = u.Avatar,
                           PositionName = p.Name,
                           OrganName = a.Name,
                           AgencyName = d.Name,
                           PhysicalPath = u.PhysicalPath,
                       };


            var userProfile = await temp.FirstOrDefaultAsync();
            if (userProfile != null && userProfile.Avatar.HasValue)
            {
                var avatarContent = await _fileClientService.GetPublicFileById(userProfile.Avatar.Value);
                if (avatarContent.Code.Equals(CommonConst.Success))
                {
                    userProfile.SrcAvatar = $"data:image/png;base64, {Encoding.ASCII.GetString((byte[])avatarContent.Data).Trim('"')}";
                }
            }

            return userProfile;
        }

        public async Task<VMUserProfile> GetCurrentReader(int id)
        {
            var temp = from u in _dasRepo.Reader.GetAll().Where(u => u.ID == id && u.Status == (int)EnumCommon.Status.Active)
                       select new VMUserProfile
                       {
                           ID = u.ID,
                           AccountName = u.AccountName,
                           Name = u.Name,
                           IdentityNumber = u.IdentityNumber,
                           Email = u.Email,
                           Phone = u.Phone,
                           Address = u.Address,
                           Avatar = u.Avatar
                       };


            var userProfile = await temp.FirstOrDefaultAsync();
            if (userProfile != null && userProfile.Avatar.HasValue)
            {
                var avatarContent = await _fileClientService.GetPublicFileById(userProfile.Avatar.Value);
                if (avatarContent.Code.Equals(CommonConst.Success))
                {
                    userProfile.SrcAvatar = $"data:image/png;base64, {Encoding.ASCII.GetString((byte[])avatarContent.Data).Trim('"')}";
                }
            }

            return userProfile;
        }

        public async Task<ServiceResult> UpdateUserProfile(VMUserProfile vmUser)
        {
            var user = await _dasRepo.User.GetAsync(vmUser.ID);
            IEnumerable<User> existUsers = await _dasRepo.User.GetAllListAsync(m => (m.Email == vmUser.Email && m.Email != user.Email)
            || (m.IdentityNumber == vmUser.IdentityNumber && m.IdentityNumber != user.IdentityNumber));
            var listExistUser = existUsers.ToList();
            if (IsExisted(listExistUser.ToList()))
                if (IsExisted(listExistUser.Where(m => m.Email == vmUser.Email && m.Email != user.Email)))
                    return new ServiceResultError("Email đã tồn tại!");
                else
                    return new ServiceResultError("Số CMND/Hộ chiếu đã tồn tại!");

            // upload avatar
            if (vmUser.File != null)
            {
                var vmFile = new VMFile
                {
                    File = vmUser.File,
                    FileName = vmUser.File.FileName,
                    FileType = (int)EnumFile.Type.Avatar,
                    //IsTemp = false
                };
                string msg = string.Empty;
                var resultUpload = Upload(vmFile, ref msg);
                if (!string.IsNullOrEmpty(msg)) return new ServiceResultError("Upload avatar không thành công!", resultUpload);

                //var objUpload = JsonConvert.DeserializeObject<VMStgFile>(resultUpload.Data.ToString());
                //vmUser.Avatar = objUpload.ID;
                vmUser.FileName = resultUpload.FileName;
                vmUser.PhysicalPath = resultUpload.PhysicalPath;
                vmUser.FileType = resultUpload.FileType;
                vmUser.Size = resultUpload.Size;
                // set IsTemp for old avatar file
                if (user.Avatar.HasValue)
                {
                    await _fileClientService.MarkFileTemp(user.Avatar.Value);
                }
            }
            else
            {
                vmUser.Avatar = user.Avatar;
                vmUser.PhysicalPath = user.PhysicalPath;
                vmUser.FileName = user.FileName;
                vmUser.PhysicalPath = user.PhysicalPath;
                vmUser.FileType = user.FileType;
                vmUser.Size = user.Size;
            }    

            UpdateData(vmUser, user);
            _mapper.Map(vmUser, user);

            await _dasRepo.User.UpdateAsync(user);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Sửa người dùng thành công!");
        }

        public async Task<ServiceResult> UploadLargeFile(List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash)
        {
            var resultUpload = await _fileClientService.UploadLargeFile(files, resumableIdentifier, resumableChunkNumber, resumableChunkSize, resumableTotalSize, resumableFilename, resumableChunkHash);
            if (resultUpload.Code == null || resultUpload.Data == null ||
                !resultUpload.Code.Equals(CommonConst.Success))
            {
                return new ServiceResultError("Upload file không thành công!");
            }

            return resultUpload;
        }

        #region Private method

        private bool IsExisted(User user)
        {
            if (user == null || user.ID == 0 || user.Status != (int)EnumCommon.Status.Active)
                return false;
            return true;
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }

        private void UpdateData(VMUserProfile vmUser, User user)
        {
            vmUser.AccountName = user.AccountName;
        }

        #endregion Private method

        public VMFile Upload(VMFile model, ref string msg)
        {
            try
            {
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
                    if (extAllowed.Contains(fileExt.ToLower()))
                    {
                        model.PhysicalPath = SavePhysicalFileAsync(model.File, model.FileName);
                        if (string.IsNullOrEmpty(model.PhysicalPath))
                        {
                            msg = " The file can not save";
                            return model;
                        }

                    }
                    else
                    {
                        msg = "The request is incorrect format";
                        return model;
                    }

                    return model;
                }
                else
                {
                    msg = "File lỗi";
                    return model;
                }
            }
            catch (Exception ex)
            {
                msg = ex.StackTrace;
                return model;
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

        public Task<List<User>> GetUserByModule(int codemodule, int idper)
        {
            var temp = from u in _dasRepo.User.GetAll()
                       from a in _dasRepo.UserGroupPer.GetAll().Where(a => a.IDUser == u.ID).DefaultIfEmpty()
                       from d in _dasRepo.PermissionGroupPer.GetAll().Where(d => d.IDGroupPermission == a.IDGroupPer).DefaultIfEmpty()
                       from g in _dasRepo.GroupPermission.GetAll().Where(g => g.ID == d.IDGroupPermission).DefaultIfEmpty()
                       from p in _dasRepo.Permission.GetAll().Where(p => p.ID == d.IDPermission && p.IDModule==codemodule && p.Type==idper).DefaultIfEmpty()
                       where (g.ActiveNotification==1)
                       select u;
            return  temp.ToListAsync();
        }
    }
}
