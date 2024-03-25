using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESD.Utility.CacheUtils;
using Microsoft.AspNetCore.Http;
using ESD.Application.Enums;
using Newtonsoft.Json;
using System.Text;

namespace ESD.Application.Services
{
    public class ReaderService : IReaderServices
    {
        #region Properties
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IDistributedCache _cache;
        private readonly IStgFileClientService _fileClientService;
        #endregion Properties

        #region Ctor
        public ReaderService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager loggerManager
            , IUserPrincipalService userPrincipalService
            , IDistributedCache cache, IStgFileClientService fileClientService)
        {
            _dasRepo = dasRepository;
            _mapper = mapper;
            _logger = loggerManager;
            _userPrincipalService = userPrincipalService;
            _cache = cache;
            _fileClientService = fileClientService;
        }
        #endregion

        #region Login & Register
        public async Task<ServiceResult> Authenticate(VMReaderLogin model)
        {
            var reader = await _dasRepo.Reader.SingleOrDefaultAsync(x => x.AccountName == model.UserName && x.Password == StringUltils.Md5Encryption(model.Password) && x.Status == (int)EnumCommon.Status.Active);
            if (reader == null || !string.Equals(reader.AccountName, model.UserName, StringComparison.Ordinal))
            {
                return new ServiceResultError("Thông tin tài khoản không chính xác");
            }
            return new ServiceResultSuccess(reader);
        }

        /// <summary>
        /// Tạo đôc giả
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idOrgan">Truyền >0 => Add user to ReaderInOrgan</param>
        /// <returns></returns>
        public async Task<ServiceResult> Register(VMReaderRegister model, int idOrgan = 0)
        {
            if (await _dasRepo.Reader.AnyAsync(x => x.AccountName == model.AccountName))
            {
                return new ServiceResultError("Tên tài khoản đã tồn tại");
            }
            if (await _dasRepo.Reader.AnyAsync(x => x.Email == model.Email))
            {
                return new ServiceResultError("Tài khoản Email đã được đăng ký");
            }
            //if (await _dasRepo.Reader.AnyAsync(x => x.IdentityNumber == model.IdentityNumber))
            //{
            //    return new ServiceResultError("CMND/CCCD đã được đăng ký");
            //}
            model.Password = StringUltils.Md5Encryption(model.Password);
            var reader = Utils.Bind<Reader>(model.KeyValue());
            //Reader reader = _mapper.Map<Reader>(model);
            //var date = Utils.GetDate(model.Birthday);
            //if (date.HasValue)
            //{
            //    reader.Birthday = date;
            //}
            await _dasRepo.Reader.InsertAsync(reader);
            await _dasRepo.SaveAync();
            if (reader.ID == 0)
            {
                return new ServiceResultError("Đăng ký tài khoản không thành công!");
            }

            if (idOrgan > 0)
            {
                await _dasRepo.ReaderInOrgan.InsertAsync(new ReaderInOrgan
                {
                    Status = (int)EnumCommon.Status.Active,
                    IDOrgan = idOrgan,
                    IDReader = reader.ID
                });
                await _dasRepo.SaveAync();
            }
            return new ServiceResultSuccess("Tạo tài khoản thành công", new
            {
                ID = reader.ID,
                Name = reader.Name
            });
        }
        #endregion Login & Register

        #region By admin System
        #region Get & Search
        public async Task<PaginatedList<VMReader>> SearchByConditionPagging(ReaderCondition condition, bool isExport = false)
        {
            var temp = from r in _dasRepo.Reader.GetAll()
                       where (condition.Keyword.IsEmpty() || r.AccountName.Contains(condition.Keyword) || r.Email.Contains(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Phone.Contains(condition.Keyword))
                       && (condition.IDStatus == -1 || condition.IDStatus == r.Status)
                       orderby r.CreateDate descending
                       select _mapper.Map<VMReader>(r);
            if (isExport)
            {
                var export = await temp.ToListAsync();
                return new PaginatedList<VMReader>(export, export.Count, 1, export.Count);
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            if (result.Count == 0)
            {
                return null;
            }

            PaginatedList<VMReader> model = new PaginatedList<VMReader>(result, (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<VMReader> GetReader(int id)
        {
            var reader = await _dasRepo.Reader.GetAsync(id);
            var model = _mapper.Map<VMReader>(reader);
            model.Birthday = Utils.DateToString(reader.Birthday);
            if (model != null && model.Avatar.HasValue)
            {
                var avatarContent = await _fileClientService.GetPublicFileById(model.Avatar.Value);
                if (avatarContent.Code.Equals(CommonConst.Success))
                {
                    model.SrcAvatar = $"data:image/png;base64, {Encoding.ASCII.GetString((byte[])avatarContent.Data).Trim('"')}";
                }
            }
            return model;
        }
        #endregion Get & Search

        #region Update
        public async Task<ServiceResult> UpdateReader(VMReader model)
        {
            try
            {
                var existReader = await _dasRepo.Reader.GetAsync(model.ID);
                if (existReader == null)
                {
                    return new ServiceResultError("Độc giả không tồn tại hoặc đã bị xóa");
                }
                if (await _dasRepo.Reader.AnyAsync(x => x.AccountName == model.AccountName && model.AccountName != existReader.AccountName))
                {
                    return new ServiceResultError("Tên tài khoản đã tồn tại");
                }
                if (await _dasRepo.Reader.AnyAsync(x => x.Email == model.Email && model.Email != existReader.Email))
                {
                    return new ServiceResultError("Email đã tồn tại");
                }
                //if (await _dasRepo.Reader.AnyAsync(x => x.IdentityNumber == model.IdentityNumber && model.IdentityNumber != existReader.IdentityNumber))
                //{
                //    return new ServiceResultError("CMND/CCCD đã được đăng ký");
                //}
                var accountName = existReader.AccountName;
                var avatar = existReader.Avatar;
                _mapper.Map(model, existReader);
                //var date = Utils.GetDate(model.Birthday);
                //if (date.HasValue)
                //{
                //    existReader.Birthday = date;
                //    existReader.Birthday = date;
                //}               
                existReader.AccountName = accountName;
                existReader.Avatar = avatar;
                await _dasRepo.Reader.UpdateAsync(existReader);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật độc giả thành công");
            }
            catch (Exception ex)
            {

                return new ServiceResultError(ex.ToString());
            }

        }
        public async Task<ServiceResult> UpdateProfileReader(VMReader model)
        {
            try
            {
                var existReader = await _dasRepo.Reader.GetAsync(model.ID);
                if (existReader == null)
                {
                    return new ServiceResultError("Độc giả không tồn tại hoặc đã bị xóa");
                }
                if (await _dasRepo.Reader.AnyAsync(x => x.AccountName == model.AccountName && model.AccountName != existReader.AccountName))
                {
                    return new ServiceResultError("Tên tài khoản đã tồn tại");
                }
                if (await _dasRepo.Reader.AnyAsync(x => x.Email == model.Email && model.Email != existReader.Email))
                {
                    return new ServiceResultError("Email đã tồn tại");
                }

                // upload avatar
                if (model.File != null)
                {
                    var stgFile = new VMStgFile
                    {
                        File = model.File,
                        FileName = model.File.FileName,
                        FileType = (int)EnumFile.Type.Avatar,
                        IsTemp = false
                    };

                    var resultUpload = await _fileClientService.Upload(stgFile);
                    if (resultUpload.Code == null || resultUpload.Data == null || !resultUpload.Code.Equals(CommonConst.Success))
                    {
                        return new ServiceResultError("Upload avatar không thành công!");
                    }

                    var objUpload = JsonConvert.DeserializeObject<VMStgFile>(resultUpload.Data.ToString());
                    model.Avatar = objUpload.ID;

                    // set IsTemp for old avatar file
                    if (existReader.Avatar.HasValue)
                    {
                        await _fileClientService.MarkFileTemp(existReader.Avatar.Value);
                    }
                }
                else
                    model.Avatar = existReader.Avatar;
                var accountName = existReader.AccountName;
                _mapper.Map(model, existReader);
              
                existReader.AccountName = accountName;
                await _dasRepo.Reader.UpdateAsync(existReader);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật độc giả thành công");
            }
            catch (Exception ex)
            {

                return new ServiceResultError(ex.ToString());
            }
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteReader(int id)
        {
            try
            {
                var existReader = await _dasRepo.Reader.GetAsync(id);
                if (existReader == null)
                {
                    return new ServiceResultError("Độc giả không tồn tại hoặc đã bị xóa");
                }
                if (await _dasRepo.CatalogingBorrow.AnyAsync(x => x.IDReader == id))
                {
                    return new ServiceResultError("Độc giả phát sinh phiếu mượn, không được xóa");
                }
                await _dasRepo.Reader.DeleteAsync(existReader);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa độc giả thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        public async Task<ServiceResult> DeleteReaders(IEnumerable<int> ids)
        {
            try
            {
                var existReaders = await _dasRepo.Reader.GetAllListAsync(x => ids.Contains(x.ID));
                if (existReaders == null || existReaders.Count() == 0)
                {
                    return new ServiceResultError("Độc giả không tồn tại hoặc đã bị xóa");
                }
                if (await _dasRepo.CatalogingBorrow.AnyAsync(x => ids.Contains(x.IDReader)))
                {
                    return new ServiceResultError("Độc giả phát sinh phiếu mượn, không được xóa ");
                }
                await _dasRepo.Reader.DeleteAsync(existReaders);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa dộc giả thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete
        #endregion By admin System

        #region By admin Organ
        public async Task<PaginatedList<VMReader>> SearchByConditionPaggingOrgan(ReaderCondition condition, bool isExport = false)
        {
            var temp = from a in _dasRepo.ReaderInOrgan.GetAll()
                       join r in _dasRepo.Reader.GetAll() on a.IDReader equals r.ID
                       where a.IDOrgan == _userPrincipalService.IDOrgan
                       && (condition.Keyword.IsEmpty() || r.AccountName.Contains(condition.Keyword) || r.Email.Contains(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Phone.Contains(condition.Keyword))
                       && (condition.IDStatus == -1 || condition.IDStatus == a.Status)
                       orderby r.CreateDate descending
                       select new VMReader
                       {
                           IDOrgan = a.IDOrgan,
                           ID = r.ID,
                           AccountName = r.AccountName,
                           Name = r.Name,
                           IdentityNumber = r.IdentityNumber,
                           Email = r.Email,
                           Phone = r.Phone,
                           Birthday = r.Birthday.HasValue ? r.Birthday.Value.ToString("dd/MM/yyy") : "",
                           Birthplace = r.Birthplace,
                           Address = r.Address,
                           Gender = r.Gender,
                           Status = r.Status,
                           StatusByOrgan = a.Status
                       };
            if (isExport)
            {
                var export = await temp.ToListAsync();
                return new PaginatedList<VMReader>(export, export.Count, 1, export.Count);
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            if (result.Count == 0)
            {
                return null;
            }

            PaginatedList<VMReader> model = new PaginatedList<VMReader>(result, (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<VMReader> GetReaderOrgan(int id)
        {
            var temp = from a in _dasRepo.ReaderInOrgan.GetAll()
                       join r in _dasRepo.Reader.GetAll() on a.IDReader equals r.ID
                       where a.IDOrgan == _userPrincipalService.IDOrgan
                       && r.ID == id
                       select new VMReader
                       {
                           IDOrgan = a.IDOrgan,
                           ID = r.ID,
                           AccountName = r.AccountName,
                           Name = r.Name,
                           IdentityNumber = r.IdentityNumber,
                           Email = r.Email,
                           Phone = r.Phone,
                           Birthday = r.Birthday.HasValue ? r.Birthday.Value.ToString("dd/MM/yyy") : "",
                           Birthplace = r.Birthplace,
                           Address = r.Address,
                           Gender = r.Gender,
                           Status = r.Status,
                           StatusByOrgan = a.Status
                       };
            var reader = await temp.FirstOrDefaultAsync();

            return reader;
        }
        public async Task<ServiceResult> UpdateReaderOrgan(VMReader model)
        {
            try
            {
                if (model.IDOrgan != _userPrincipalService.IDOrgan || model.Status != (int)EnumCommon.Status.Active)
                {
                    return new ServiceResultError("Không có quyền chỉnh sửa độc giả");
                }
                var existReader = await _dasRepo.Reader.GetAsync(model.ID);
                if (existReader == null)
                {
                    return new ServiceResultError("Độc giả không tồn tại hoặc đã bị xóa");
                }
                var isInOrgan = await _dasRepo.ReaderInOrgan.FirstOrDefaultAsync(x => x.IDOrgan == _userPrincipalService.IDOrgan && x.IDReader == model.ID);
                if (isInOrgan == null)
                {
                    return new ServiceResultError("Không có quyền chỉnh sửa độc giả");
                }
                if (await _dasRepo.Reader.AnyAsync(x => x.AccountName == model.AccountName && model.AccountName != existReader.AccountName))
                {
                    return new ServiceResultError("Tên tài khoản đã tồn tại");
                }
                if (await _dasRepo.Reader.AnyAsync(x => x.Email == model.Email && model.Email != existReader.Email))
                {
                    return new ServiceResultError("Email đã tồn tại");
                }
                //if (await _dasRepo.Reader.AnyAsync(x => x.IdentityNumber == model.IdentityNumber && model.IdentityNumber != existReader.IdentityNumber))
                //{
                //    return new ServiceResultError("CMND/CCCD đã được đăng ký");
                //}

                //Update ReaderInOrgan
                isInOrgan.Status = model.StatusByOrgan;
                await _dasRepo.ReaderInOrgan.UpdateAsync(isInOrgan);
                await _dasRepo.SaveAync();

                var avatar = existReader.Avatar;
                var accountName = existReader.AccountName;
                _mapper.Map(model, existReader);
                //var date = Utils.GetDate(model.Birthday);
                //if (date.HasValue)
                //{
                //    existReader.Birthday = date;
                //    existReader.Birthday = date;
                //}           
                existReader.Avatar = avatar;
                existReader.AccountName = accountName;
                await _dasRepo.Reader.UpdateAsync(existReader);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật độc giả thành công");
            }
            catch (Exception ex)
            {

                return new ServiceResultError(ex.ToString());
            }

        }

        public async Task<ServiceResult> RegisterByOrgan(VMReaderRegister model)
        {
            int statusInOrgan = model.Status;
            model.Status = (int)EnumCommon.Status.Active;// Khi tạo ở Organ thì trạng thái trên hệ thống là Active
            if (await _dasRepo.Reader.AnyAsync(x => x.AccountName == model.AccountName))
            {
                return new ServiceResultError("Tên tài khoản đã tồn tại");
            }
            if (await _dasRepo.Reader.AnyAsync(x => x.Email == model.Email))
            {
                return new ServiceResultError("Tài khoản Email đã được đăng ký");
            }
            //if (await _dasRepo.Reader.AnyAsync(x => x.IdentityNumber == model.IdentityNumber))
            //{
            //    return new ServiceResultError("CMND/CCCD đã được đăng ký");
            //}
            model.Password = StringUltils.Md5Encryption(model.Password);
            var reader = Utils.Bind<Reader>(model.KeyValue());
            //Reader reader = _mapper.Map<Reader>(model);
            //var date = Utils.GetDate(model.Birthday);
            //if (date.HasValue)
            //{
            //    reader.Birthday = date;
            //}
            await _dasRepo.Reader.InsertAsync(reader);
            await _dasRepo.SaveAync();
            if (reader.ID == 0)
            {
                return new ServiceResultError("Đăng ký tài khoản không thành công!");
            }
            ReaderInOrgan readerInOrgan = new ReaderInOrgan
            {
                Status = statusInOrgan,
                IDOrgan = _userPrincipalService.IDOrgan,
                IDReader = reader.ID
            };
            await _dasRepo.ReaderInOrgan.InsertAsync(readerInOrgan);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Tạo tài khoản thành công");
        }

        public async Task<bool> IsReaderInOrgan(int idOrgan, int idReader)
        {
            return await _dasRepo.ReaderInOrgan.AnyAsync(x => x.IDOrgan == idOrgan && x.IDReader == idReader);
        }
        /// <summary>
        /// Thêm độc giả vào danh sách của cơ quan
        /// </summary>
        /// <param name="idOrgan"></param>
        /// <param name="idReader"></param>
        /// <returns></returns>
        public async Task<bool> AddReaderInOrgan(int idOrgan, int idReader)
        {
            try
            {
                if (await _dasRepo.ReaderInOrgan.AnyAsync(x => x.IDOrgan == idOrgan && x.IDReader == idReader))
                {
                    return true;
                }
                else
                {
                    ReaderInOrgan readerInOrgan = new ReaderInOrgan
                    {
                        Status = (int)EnumCommon.Status.Active,
                        IDOrgan = _userPrincipalService.IDOrgan,
                        IDReader = idReader
                    };
                    await _dasRepo.ReaderInOrgan.InsertAsync(readerInOrgan);
                    await _dasRepo.SaveAync();
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        #endregion By admin Organ

        #region ChangePassword
        public async Task<ServiceResult> ChangePassword(VMAccount model)
        {
            try
            {
                int timeBetweenLock = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.TIME_LOCK_BETWEEN_LOGIN_READER)) ?? "").ToString(), out timeBetweenLock) ? timeBetweenLock : 5;
                int maxChangePWDFaild = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_CHANGE_PWD_FAILD_READER)) ?? "").ToString(), out maxChangePWDFaild) ? maxChangePWDFaild : 5;
                if (_userPrincipalService.UserId == -1)
                {
                    return new ServiceResultError("Chưa đăng nhập không đổi được mật khẩu");
                }
                var reader = await _dasRepo.Reader.GetAsync(_userPrincipalService.UserId);
                Dictionary<string, int> countByCache;
                countByCache = (await _cache.GetCacheValueAsync<Dictionary<string, int>>(string.Concat(CacheConst.READER_CHANGE_PWD_FAIL, reader.AccountName)));
                if (!reader.Password.Equals(StringUltils.Md5Encryption(model.OldPassword)))
                {
                    if (countByCache != null)
                    {
                        countByCache[reader.AccountName] = countByCache[reader.AccountName] + 1;
                        await _cache.SetCacheValueAsync(string.Concat(CacheConst.READER_CHANGE_PWD_FAIL, reader.AccountName), countByCache, timeBetweenLock * 60);
                        if (countByCache[reader.AccountName] >= maxChangePWDFaild)
                        {
                            return new ServiceResultError($"Bạn đã nhập sai quá {maxChangePWDFaild} lần liên tiếp, thử lại trong vòng {timeBetweenLock} phút");
                        }
                    }
                    else
                    {
                        var countFail = new Dictionary<string, int> {
                        { reader.AccountName ,1 }
                    };
                        await _cache.SetCacheValueAsync(string.Concat(CacheConst.READER_CHANGE_PWD_FAIL, reader.AccountName), countFail, timeBetweenLock * 60);
                    }

                    return new ServiceResultError("Mật khẩu hiện tại không đúng");
                }
                if (countByCache != null)
                {
                    await _cache.RemoveAsync(string.Concat(CacheConst.READER_CHANGE_PWD_FAIL, reader.AccountName));
                }
                reader.Password = StringUltils.Md5Encryption(model.Password);
                await _dasRepo.Reader.UpdateAsync(reader);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật mật khẩu thành công");
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }


        #endregion ChangePassword

    }
}
