using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.MobileApiModel;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using static ESD.Application.Enums.EnumBorrow;

namespace ESD.Application.Services
{
    public class DataApiServices : IDataApiServices
    {
        #region Properties

        protected IDasRepositoryWrapper _dasRepo;
        protected IDasNotifyRepositoryWrapper _dasNotifyRepo;
        protected IUserPrincipalService _userPrincipalService;
        protected IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ICacheManagementServices _cacheManagementServices;
        private const string ProcessCode = "CONFIG_ORGAN_PROCESS";
        private readonly ILoggerManager _logger;
        private readonly IStgFileClientService _fileClientService;
        #endregion

        #region Ctor

        public DataApiServices(IDasRepositoryWrapper dasRepo
            , IUserPrincipalService userPrincipalService
            , IDasNotifyRepositoryWrapper dasNotifyRepo
            , ICacheManagementServices cacheManagementServices
            , IMapper mapper
            , ILoggerManager logger
            , IDistributedCache cache
            )
        {
            _dasRepo = dasRepo;
            _dasNotifyRepo = dasNotifyRepo;
            _userPrincipalService = userPrincipalService;
            _mapper = mapper;
            _cacheManagementServices = cacheManagementServices;
            _logger = logger;
            _cache = cache;
        }

        #endregion

        #region Task #27788: Đăng ký tài khoản Người dùng khai thác

        public async Task<ServiceResult> Register(VMMobileReaderRegister model)
        {
            if (await _dasRepo.Reader.AnyAsync(x => x.AccountName == model.AccountName))
            {
                return new ServiceResultError("Tên tài khoản đã tồn tại");
            }
            if (await _dasRepo.Reader.AnyAsync(x => x.Email == model.Email))
            {
                return new ServiceResultError("Tài khoản Email đã được đăng ký");
            }
            model.Password = StringUltils.Md5Encryption(model.Password);
            var reader = Utils.Bind<Reader>(model.KeyValue());
            //reader.Birthday = Utils.GetDate(model.Birthday);
            await _dasRepo.Reader.InsertAsync(reader);
            await _dasRepo.SaveAync();
            if (reader.ID == 0)
            {
                return new ServiceResultError("Đăng ký tài khoản không thành công!");
            }
            return new ServiceResultSuccess("Tạo tài khoản thành công");
        }

        public async Task<ServiceResult> Authenticate(VMMobileReaderLogin loginModel)
        {
            var reader = await _dasRepo.Reader.SingleOrDefaultAsync(x => x.AccountName == loginModel.UserName && x.Password == StringUltils.Md5Encryption(loginModel.Password) && x.Status == (int)EnumCommon.Status.Active);
            if (reader == null || !string.Equals(reader.AccountName, loginModel.UserName, StringComparison.Ordinal))
            {
                return new ServiceResultError("Thông tin tài khoản không chính xác");
            }
            return new ServiceResultSuccess(reader);
        }

        public async Task<ServiceResult> ChangePassword(VMMobileChangePassword model)
        {
            try
            {
                var idReader = _userPrincipalService.UserId;
                int timeBetweenLock = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.TIME_LOCK_BETWEEN_LOGIN_READER)) ?? "").ToString(), out timeBetweenLock) ? timeBetweenLock : 5;
                int maxChangePWDFaild = int.TryParse(((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_CHANGE_PWD_FAILD_READER)) ?? "").ToString(), out maxChangePWDFaild) ? maxChangePWDFaild : 5;
                if (idReader <= 0)
                {
                    return new ServiceResultError("Chưa đăng nhập không đổi được mật khẩu");
                }
                var reader = await _dasRepo.Reader.GetAsync(idReader);
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
                            {
                                reader.AccountName ,1

                            }};
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
                return new ServiceResultError(ex.Message);
            }
        }

      
        #endregion

    }
}
