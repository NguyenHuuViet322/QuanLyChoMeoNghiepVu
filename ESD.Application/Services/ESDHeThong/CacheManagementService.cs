using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using ESD.Domain.Enums;
using ESD.Utility;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility.LogUtils;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;
using ESD.Application.Constants;

namespace ESD.Application.Services
{
    public class CacheManagementService : ICacheManagementServices
    {
        private readonly IDistributedCache _cache;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IDasRepositoryWrapper _dasRepo;

        public CacheManagementService(IDasRepositoryWrapper dasRepository
            , ILoggerManager logger
            , IUserPrincipalService userPrincipalService
            , IDistributedCache cache)
        {
            _logger = logger;
            _userPrincipalService = userPrincipalService;
            _dasRepo = dasRepository;
            _cache = cache;
        }

        public async Task<UserData> GetUserDataAndSetCache()
        {
            UserData userData = new UserData();
            var userDatas = await _cache.GetCacheValueAsync<Dictionary<int, UserData>>(CacheConst.USER_DATA);
            if (userDatas.IsNotEmpty())
            {
                userDatas.TryGetValue(_userPrincipalService.UserId, out userData);
            }
            if (userData.IsNotEmpty() && userData.IDOrgan > 0)
                return userData;

            userData = await (from u in _dasRepo.User.GetAll()
                              where u.ID == _userPrincipalService.UserId
                              join a in _dasRepo.Agency.GetAll() on u.IDAgency equals a.ID
                              select new UserData
                              {
                                  IDAgency = u.IDAgency,
                                  IDOrgan = u.IDOrgan,
                                  ParentPath = a.ParentPath,
                                  HasOrganPermission = u.HasOrganPermission
                              }).FirstOrDefaultAsync();
            if (userDatas == null) //Chưa tồn tại cache => tạo mới cả tập cache
            {
                userDatas = new Dictionary<int, UserData>();
                userDatas.Add(_userPrincipalService.UserId, userData);
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
            }
            else if (!userDatas.ContainsKey(_userPrincipalService.UserId)) //Chưa tồn tại quyền cho user trong cache => thêm quyền cho user vào
            {
                userDatas.Add(_userPrincipalService.UserId, userData);
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
            }
            else
            {
                userDatas[_userPrincipalService.UserId] = userData;
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
            }
            return userData;
        }

        public async Task<UserData> GetCurrentUserData()
        {
            UserData userData = new UserData();
            var userDatas = await _cache.GetCacheValueAsync<Dictionary<int, UserData>>(CacheConst.USER_DATA);
            if (userDatas.IsNotEmpty())
            {
                userDatas.TryGetValue(_userPrincipalService.UserId, out userData);
            }
            if (userData.IsNotEmpty() && userData.IDOrgan > 0)
            {
                // update change infos from DB
                var userDataDb = await (from u in _dasRepo.User.GetAll()
                                        where u.ID == _userPrincipalService.UserId
                                        join o in _dasRepo.Organ.GetAll() on u.IDOrgan equals o.ID
                                        join a in _dasRepo.Agency.GetAll() on u.IDAgency equals a.ID into joined
                                        from aj in joined.DefaultIfEmpty()
                                        select new UserData
                                        {
                                            IDAgency = u.IDAgency,
                                            AgencyName = aj.Name,
                                            IDOrgan = u.IDOrgan,
                                            OrganName = o.Name,
                                            ParentPath = aj.ParentPath,
                                            HasOrganPermission = u.HasOrganPermission,
                                            Status = u.Status
                                        }).FirstOrDefaultAsync();
                if (userDataDb != null)
                {
                    if (userData.IDAgency != userDataDb.IDAgency || userData.AgencyName != userDataDb.AgencyName ||
                    userData.IDOrgan != userDataDb.IDOrgan || userData.OrganName != userDataDb.OrganName ||
                    userData.ParentPath != userDataDb.ParentPath ||
                    userData.HasOrganPermission != userDataDb.HasOrganPermission ||
                    userData.Status != userDataDb.Status)
                    {
                        userData.IDAgency = userDataDb.IDAgency;
                        userData.AgencyName = userDataDb.AgencyName;
                        userData.IDOrgan = userDataDb.IDOrgan;
                        userData.OrganName = userDataDb.OrganName;
                        userData.ParentPath = userDataDb.ParentPath;
                        userData.HasOrganPermission = userDataDb.HasOrganPermission;
                        userData.Status = userDataDb.Status;

                        userDatas[_userPrincipalService.UserId] = userData;
                        await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
                    }
                }


                return userData;
            }

            userData = await (from u in _dasRepo.User.GetAll().AsNoTracking()
                              where u.ID == _userPrincipalService.UserId
                              join o in _dasRepo.Organ.GetAll().AsNoTracking() on u.IDOrgan equals o.ID
                              join a in _dasRepo.Agency.GetAll().AsNoTracking() on u.IDAgency equals a.ID into joined
                              from aj in joined.DefaultIfEmpty()
                              select new UserData
                              {
                                  IDAgency = u.IDAgency,
                                  AgencyName = aj.Name,
                                  IDOrgan = u.IDOrgan,
                                  OrganName = o.Name,
                                  ParentPath = aj.ParentPath,
                                  HasOrganPermission = u.HasOrganPermission,
                                  Status = u.Status
                              }).FirstOrDefaultAsync();


            if (!userData.IsNotEmpty())
            {
                return new UserData();
            }

            var checkAdmin = await ((from ugp in _dasRepo.UserGroupPer.GetAll().AsNoTracking()
                                     where ugp.IDUser == _userPrincipalService.UserId
                                     join gp in _dasRepo.GroupPermission.GetAll().AsNoTracking() on ugp.IDGroupPer equals gp.ID
                                     where gp.IDOrgan == userData.IDOrgan && gp.IsAdminOrgan && userData.Status == (int)EnumCommon.Status.Active
                                     select gp.ID).LongCountAsync());
            userData.IsAdminOrgan = checkAdmin > 0;

            if (userDatas == null) //Chưa tồn tại cache => tạo mới cả tập cache
            {
                userDatas = new Dictionary<int, UserData>
                {
                    { _userPrincipalService.UserId, userData }
                };
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
            }
            else if (!userDatas.ContainsKey(_userPrincipalService.UserId)) //Chưa tồn tại quyền cho user trong cache => thêm quyền cho user vào
            {
                userDatas.Add(_userPrincipalService.UserId, userData);
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
            }
            else
            {
                userDatas[_userPrincipalService.UserId] = userData;
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, userDatas);
            }
            return userData;
        }
    }
}
