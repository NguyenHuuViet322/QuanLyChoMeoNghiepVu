using ESD.Application.Interfaces;
using ESD.Domain.Interfaces.DAS;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using ESD.Utility.CacheUtils;
using ESD.Application.Models.CustomModels;
using System.Threading.Tasks;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Constants;
using System.Linq;
using ESD.Application.Enums;

namespace ESD.Application.Services
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IDistributedCache _cache;
        private readonly IUserPrincipalService _userPrincipalService;
        public AuthorizeService(IDistributedCache cache
            , IUserPrincipalService userPrincipalService)
        {
            _userPrincipalService = userPrincipalService;
            _cache = cache;
        }

        /// <summary>
        /// Check xem người dùng hiện tại có quyền được truyền vào không
        /// 1. Lấy ra quyền của tất cả người dùng
        /// 2. lấy ra quyền theo người dùng hiện tại
        /// 3. kiểm tra trong những quyền mà người dùng hiện tại có tồn tại quyền truyền vào hay không
        /// </summary>
        /// <param name="CodeModule"></param>
        /// <param name="Permissions"></param>
        /// <returns></returns> 
        public async Task<bool> CheckPermission(int CodeModule, int[] Permissions)
        {
            ////Fix cứng quyền cho superadmin
            //// TODO: by pass
            //return true;
            //var moduleForSuperAdmin = new int[] {
            //    (int)EnumModule.Code.M20010 // cơ quan
            //    , (int)EnumModule.Code.S9023 // quản trị cơ quan
            //    ,  (int)EnumModule.Code.M20030 // dm dùng chung
            //    , (int)EnumModule.Code.NKHT //nhật ký hệ thống
            //    , (int)EnumModule.Code.NKND //nhật ký người dùng
            //    , (int)EnumModule.Code.CHTS // cấu hình tham số hệ thống
            //    , (int)EnumModule.Code.S9030 // quản lý nhóm quyền 
            //    , (int)EnumModule.Code.SCHEMA // quản lý CSDL
            //    , (int)EnumModule.Code.TABLEINFO // quản lý TABLE
            //    , (int)EnumModule.Code.INPUTINFO // quản lý NHAP LIEU
            //};

            if (_userPrincipalService.IsSuperUser)  //is super admin,
                return true;

            if (CodeModule == 0 || Permissions.Length == 0)
                return false;

            //1
            var permissionCache = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            if (permissionCache == null || !permissionCache.ContainsKey(_userPrincipalService.UserId))
                return false;
            //2
            List<UserPermissionModel> permissons = new List<UserPermissionModel>();
            permissionCache.TryGetValue(_userPrincipalService.UserId, out permissons);
            if (permissons.Count == 0)
                return false;
            //3
            var isAccess = permissons.Any(x => x.CodeModule == CodeModule && Permissions.Contains(x.Type));
            return isAccess;
        }



    }
}
