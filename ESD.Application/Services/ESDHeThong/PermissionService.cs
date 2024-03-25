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

namespace ESD.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IMapper _mapper;
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IDistributedCache _cache;
        public PermissionService(IDasRepositoryWrapper dasRepository, IMapper mapper, IDistributedCache cache)
        {
            _mapper = mapper;
            _dasRepo = dasRepository;
            _cache = cache;
        }

        /// <summary>
        /// 1. Check permission for user is exist. If exist => nothing , else => push permission of current to cache
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task AddCachePermission(int UserId)
        {
            Dictionary<int, List<UserPermissionModel>> dicUserPermission;
            dicUserPermission = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            if (dicUserPermission == null) //Chưa tồn tại cache => tạo mới cả tập cache
            {
                var permissions = await GetPemissionForUser(UserId);
                dicUserPermission = new Dictionary<int, List<UserPermissionModel>>();
                dicUserPermission.Add(UserId, permissions);
                await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermission);
            }

            else if (!dicUserPermission.ContainsKey(UserId)) //Chưa tồn tại quyền cho user trong cache => thêm quyền cho user vào
            {
                var permissions = await GetPemissionForUser(UserId);
                dicUserPermission.Add(UserId, permissions);
                await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermission);
            }
        }
        public async Task UpdateCachePermission(int UserId)
        {
            Dictionary<int, List<UserPermissionModel>> dicUserPermission;
            dicUserPermission = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            if (dicUserPermission == null) //Chưa tồn tại cache => tạo mới cả tập cache
            {
                var permissions = await GetPemissionForUser(UserId);
                dicUserPermission = new Dictionary<int, List<UserPermissionModel>>();
                dicUserPermission.Add(UserId, permissions);
                await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermission);
            }
            else if (!dicUserPermission.ContainsKey(UserId)) //Chưa tồn tại quyền cho user trong cache => thêm quyền cho user vào
            {
                var permissions = await GetPemissionForUser(UserId);
                dicUserPermission.Add(UserId, permissions);
                await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermission);
            }
            else
            {
                var permissions = await GetPemissionForUser(UserId);
                dicUserPermission[UserId] = permissions;
                await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermission);
            }
        }
        public async Task UpdateCachePermissionByIdGroupPer(int GroupPermissionId)
        {
            var userIds = await _dasRepo.UserGroupPer.GetAll().Where(s => s.IDGroupPer == GroupPermissionId).Select(s => s.IDUser).ToArrayAsync();
            if (userIds.Length != 0)
            {
                await UpdateCachePermission(userIds);
            }
        }
        public async Task UpdateCachePermission(int[] UserIds)
        {
            var dicUserPermissionNew = await GetPemissionForUser(UserIds);
            //if (dicUserPermissionNew == null || dicUserPermissionNew.Count == 0)
            //    return;

            var dicUserPermissionCache = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            if (dicUserPermissionCache == null) //Chưa tồn tại cache => tạo mới cả tập cache
            {
                await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermissionNew);
                return;
            }

            for (int i = 0; i < UserIds.Length; i++)
            {
                if (dicUserPermissionCache.ContainsKey(UserIds[i]))
                {
                    if (dicUserPermissionNew.ContainsKey(UserIds[i]))
                        dicUserPermissionCache[UserIds[i]] = dicUserPermissionNew[UserIds[i]];
                    else
                    {
                        dicUserPermissionCache[UserIds[i]] = new List<UserPermissionModel>();
                    }
                }
                else
                {
                    if (dicUserPermissionNew.ContainsKey(UserIds[i]))
                        dicUserPermissionCache.Add(UserIds[i], dicUserPermissionNew[UserIds[i]]);
                    else
                    {
                        dicUserPermissionCache.Add(UserIds[i], new List<UserPermissionModel>());
                    }
                }
            }
            await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, dicUserPermissionCache);
        }
        public async Task<IEnumerable<PermissionForGroupPer>> GetPermissionWithModule()
        {

            return await (from per in _dasRepo.Permission.GetAll()
                              //join childModule in _dasRepo.ModuleChild.GetAll() on per.IDModule equals childModule.ID
                          join module in _dasRepo.Module.GetAll() on per.IDModule equals module.ID
                          where module.IsShow == 1
                          select new PermissionForGroupPer
                          {
                              IDPermission = per.ID,
                              IDModule = module.ID,
                              //IDModuleChild = childModule.ID,
                              PermissionName = per.Name,
                              ModuleName = module.Name,
                              Type = per.Type,
                              //ModuleChildName = childModule.Name,
                              IsCheck = false
                          }).ToListAsync();
        }

        public async Task<IEnumerable<GroupPermission>> GetListBasePermission()
        {
            return await _dasRepo.GroupPermission.GetAllListAsync(gp => gp.Status == (int)EnumCommon.Status.Active && gp.IsBase);
        }

        public async Task<IEnumerable<PermissionForGroupPer>> GetPermissionForEditGroupPer(int IdGroupPer)
        {
            //1. Get IDPermissions by IDGroupPermission on table PermissionGroupPer
            var permissionGroupPers = _dasRepo.PermissionGroupPer.GetAll(o => o.IDGroupPermission == IdGroupPer);

            //2. Get Permission information in Table Permission base on IDPermissions (step 1)
            var permissions = await (from per in _dasRepo.Permission.GetAll()
                                     join module in _dasRepo.Module.GetAll() on per.IDModule equals module.ID
                                     select new PermissionForGroupPer
                                     {
                                         IDPermission = per.ID,
                                         PermissionName = per.Name,
                                         IDModule = module.ID,
                                         ModuleName = module.Name,
                                         Type = per.Type,
                                         //IDModuleChild = childModule.ID,
                                         //ModuleChildName = childModule.Name,
                                         IsCheck = false
                                     }).ToListAsync();

            if (permissionGroupPers != null && permissionGroupPers.Count() > 0)
            {
                permissions.ForEach(per =>
                {
                    var permissionGroupPer = permissionGroupPers.FirstOrDefault(s => s.IDPermission == per.IDPermission);
                    if (permissionGroupPer != null)
                    {
                        per.IsCheck = true;
                        per.IDPermissionGroupPer = permissionGroupPer.ID;
                    }
                });
            }


            return permissions;
        }
        #region private function
        private async Task<Dictionary<int, List<UserPermissionModel>>> GetPemissionForUser(int[] UserIds)
        {

            var permisionByUser = (from UGrp in _dasRepo.UserGroupPer.GetAll()
                                   where UserIds.Contains(UGrp.IDUser)
                                   join PGrp in _dasRepo.PermissionGroupPer.GetAll() on UGrp.IDGroupPer equals PGrp.IDGroupPermission
                                   select new { UGrp.IDUser, PGrp.IDPermission }).Distinct();

            var permissionByTeam = from ut in _dasRepo.UserTeam.GetAll()
                                   where UserIds.Contains(ut.IDUser)
                                   join TGrp in _dasRepo.TeamGroupPer.GetAll() on ut.IDTeam equals TGrp.IDTeam
                                   join PGrp in _dasRepo.PermissionGroupPer.GetAll() on TGrp.IDGroupPer equals PGrp.IDGroupPermission
                                   select new { ut.IDUser, PGrp.IDPermission };

            if (permissionByTeam.Count() != 0)
            {
                permisionByUser = permisionByUser.Union(permissionByTeam);
            }

            var lstUserPermission = await (from p in _dasRepo.Permission.GetAll()
                                           join n in permisionByUser on p.ID equals n.IDPermission
                                           join m in _dasRepo.Module.GetAll() on p.IDModule equals m.ID
                                           select new
                                           {
                                               userId = n.IDUser,
                                               idPermission = p.ID,
                                               idModule = p.IDModule,
                                               codeModule = m.Code,
                                               type = p.Type
                                           }).ToListAsync();

            Dictionary<int, List<UserPermissionModel>> dicUserPermission = lstUserPermission.GroupBy(s => s.userId)
                      .ToDictionary(s => s.Key, s => s.Select(k => new UserPermissionModel
                      {
                          IdPermission = k.userId,
                          IdModule = k.idModule,
                          CodeModule = k.codeModule,
                          Type = k.type
                      }).ToList()); //=> lấy được list Permission theo từng userid

            return dicUserPermission;

        }
        private async Task<List<UserPermissionModel>> GetPemissionForUser(int UserId)
        {
            var grpIdByUser = (from UGrp in _dasRepo.UserGroupPer.GetAll()
                               where UGrp.IDUser == UserId
                               select UGrp.IDGroupPer);
            var grpIdByTeam = from ut in _dasRepo.UserTeam.GetAll()
                              where ut.IDUser == UserId
                              join TGrp in _dasRepo.TeamGroupPer.GetAll() on ut.IDTeam equals TGrp.IDTeam
                              select TGrp.IDGroupPer;

            if (grpIdByTeam.Count() != 0)
            {
                grpIdByUser = grpIdByUser.Union(grpIdByTeam);
            }

            var permissionIds = (from PGrp in _dasRepo.PermissionGroupPer.GetAll()
                                 where grpIdByUser.Contains(PGrp.IDGroupPermission)
                                 select PGrp.IDPermission).Distinct();

            var rs = await (from p in _dasRepo.Permission.GetAll()
                            where permissionIds.Contains(p.ID)
                            join m in _dasRepo.Module.GetAll() on p.IDModule equals m.ID
                            select new UserPermissionModel
                            {
                                IdPermission = p.ID,
                                IdModule = p.IDModule,
                                CodeModule = m.Code,
                                Type = p.Type
                            }).ToListAsync();

            return rs;
        }


        /// <summary>
        /// Lấy ra quyền theo module của tất cả các người dùng trong hệ thống 
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<int, List<UserPermissionModel>>> GetPemissionForAllUser()
        {

            var permisionByUser = (from UGrp in _dasRepo.UserGroupPer.GetAll()
                                   join PGrp in _dasRepo.PermissionGroupPer.GetAll() on UGrp.IDGroupPer equals PGrp.IDGroupPermission
                                   select new { UGrp.IDUser, PGrp.IDPermission }).Distinct();

            var permissionByTeam = from ut in _dasRepo.UserTeam.GetAll()
                                   join TGrp in _dasRepo.TeamGroupPer.GetAll() on ut.IDTeam equals TGrp.IDTeam
                                   join PGrp in _dasRepo.PermissionGroupPer.GetAll() on TGrp.IDGroupPer equals PGrp.IDGroupPermission
                                   select new { ut.IDUser, PGrp.IDPermission };

            if (permissionByTeam.Count() != 0)
            {
                permisionByUser = permisionByUser.Union(permissionByTeam);
            }

            var lstUserPermission = await (from p in _dasRepo.Permission.GetAll()
                                           join n in permisionByUser on p.ID equals n.IDPermission
                                           join m in _dasRepo.Module.GetAll() on p.IDModule equals m.ID
                                           select new
                                           {
                                               userId = n.IDUser,
                                               idPermission = p.ID,
                                               idModule = p.IDModule,
                                               codeModule = m.Code,
                                               type = p.Type
                                           }).ToListAsync();

            Dictionary<int, List<UserPermissionModel>> dicUserPermission = lstUserPermission.GroupBy(s => s.userId)
                      .ToDictionary(s => s.Key, s => s.Select(k => new UserPermissionModel
                      {
                          IdPermission = k.userId,
                          IdModule = k.idModule,
                          CodeModule = k.codeModule,
                          Type = k.type
                      }).ToList()); //=> lấy được list Permission theo từng userid

            return dicUserPermission;

        }


        /// <summary>
        /// Đưa quyền của tất cả người dùng vào cache
        /// </summary>
        /// <returns></returns>
        public async Task LoadCacheAllPermission()
        {
            var allUserPermisison = await GetPemissionForAllUser();
            await _cache.RemoveAsync(CacheConst.USER_PERMISSION);
            await _cache.SetCacheValueAsync(CacheConst.USER_PERMISSION, allUserPermisison);
        }

        #endregion

    }
}
