using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class ModuleService : BaseMasterService, IModuleService
    {
        #region Properties
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;
        private readonly IPermissionService _permissionService;
        private readonly string _listString = "fa-file-alt,fa-user";
        #endregion

        #region Ctor
        public ModuleService(IDasRepositoryWrapper dasRepository, ILoggerManager logger, IMapper mapper
            , IDistributedCache cache
            , IUserPrincipalService userPrincipalService
             , IPermissionService permission
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _userPrincipalService = userPrincipalService;
            _permissionService = permission;
            _cacheManagementServices = cacheManagementServices;
        }


        #endregion

        #region Get
        public async Task<IEnumerable<Module>> Gets()
        {
            var result = await _dasRepo.Module.GetAllListAsync();
            return result;
        }

        public async Task<IEnumerable<Module>> GetsActive()
        {
            return await _dasRepo.Module.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active).ToListAsync();
        }
        public async Task<Module> Get(object id)
        {
            return await _dasRepo.Module.GetAsync(id);
        }

        public async Task<PaginatedList<VMModule>> SearchByConditionPagging(ModuleCondition condition)
        {
            var temprood = from m in _dasRepo.Module.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active)
                           select new VMModule
                           {
                               ID = m.ID,
                               ParentId = m.ParentId,
                               ParentPath = m.ParentPath,
                               IDChannel = m.IDChannel,
                               Name = m.Name,
                               Icon = m.Icon,
                               SortOrder = m.SortOrder,
                               Url = m.Url,
                               Status = m.Status,
                           };
            var root = await temprood.ToListAsync();

            var temp = from m in _dasRepo.Module.GetAll().Where(x => x.Status == (int)EnumCommon.Status.Active)
                       where (condition.Keyword.IsEmpty() || m.Name.Contains(condition.Keyword.Trim()))
                       select new VMModule
                       {
                           ID = m.ID,
                           ParentId = m.ParentId,
                           ParentPath = m.ParentPath,
                           IDChannel = m.IDChannel,
                           Name = m.Name,
                           Icon = m.Icon,
                           SortOrder = m.SortOrder,
                           Url = m.Url,
                           Status = m.Status,
                       };
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var searched = await temp.ToListAsync();
            var listNode = searched.Select(x => x.ID).ToHashSet();

            var result = RenderTreeOrder(root,0,listNode);

            ////RenderTree
            //var treeModels = Utils.RenderTree(result.Select(n => new TreeModel<VMModule>
            //{
            //    ID = n.ID,
            //    Name = n.Name,
            //    Parent = n.ParentId,
            //    ParentPath = n.ParentPath,
            //    SortOrder = n.SortOrder,
            //    Item = n
            //}).ToList(), null);
            //var list = RenderTreeOrder(treeModels.Select(n => n.Item).ToList(), 0);

            return new PaginatedList<VMModule>(result, (int)total, condition.PageIndex, condition.PageSize);
        }
        public async Task<List<SelectListItemTree>> GetModuleByTree(VMModule vMModule)
        {
            var prPath = $"{vMModule.ParentPath}|{vMModule.ID}|";
            var result = await _dasRepo.Module.GetAllListAsync(m => m.Status == (int)EnumCommon.Status.Active
            && (vMModule.ID == 0 || (vMModule.ID > 0 && m.ID != vMModule.ID && vMModule.ParentId != vMModule.ID && vMModule.ParentPath.IndexOf(prPath) != 0)));

            var treeModels = Utils.RenderTree(result.Select(n => new TreeModel<VMModule>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.ParentId,
                ParentPath = n.ParentPath,
            }).ToList(), null, "");

            return treeModels.Select(n => new SelectListItemTree
            {
                Value = n.ID.ToString(),
                Text = n.Name,
                Selected = n.ID == vMModule.ParentId,
                Level = n.Level
            }).ToList();
        }
        public async Task<List<SelectListItem>> GetListIcon(VMModule vMModule)
        {
            var icons = _listString.Split(",");
            return icons.Select(n => new SelectListItem
            {
                Value = n.ToString(),
                Text = n.ToString(),
                Selected = n.ToString() == vMModule.Icon
            }).ToList();
        }

        public List<SelectListItem> GetModuleCodeDrd(string moduleCode)
        {
            var dicValueAndDes = EnumUltils.GetDescription<EnumModule.Code>();
            if(dicValueAndDes.Count == 0)
            {
                return new List<SelectListItem>();
            }

           return dicValueAndDes.Select(s => new SelectListItem
            {
                Text = $"{s.Key.ToString()} - {s.Value}",
                Selected = (!string.IsNullOrEmpty(moduleCode) && ((int)s.Key).ToString() == moduleCode),
                Value = ((int)s.Key).ToString(),
            }).ToList();



        }

        #endregion

        #region Create
        public async Task<ServiceResult> Create(VMModule vmModule)
        {
            try
            {
                var module = _mapper.Map<Module>(vmModule);
                BindUpdate(module, vmModule);
                var listExist = await _dasRepo.Module.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active 
                && x.Url.ToLower() != "#" 
                && x.Url.ToLower() == module.Url.ToLower());
                if (listExist != null && listExist.Count() > 0)
                {
                    return new ServiceResultError("Đường link đã tồn tại");
                }
                await _dasRepo.Module.InsertAsync(module);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Thêm Menu thành công");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion

        #region Delete
        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                var moduleDelete = await _dasRepo.Module.GetAsync(id);
                if (moduleDelete == null || moduleDelete.Status == (int)EnumCommon.Status.InActive)
                    return new ServiceResultError("Menu này không tồn tại hoặc đã bị xóa");
                moduleDelete.Status = (int)EnumCommon.Status.InActive;

                await _dasRepo.Module.UpdateAsync(moduleDelete);
                await _dasRepo.SaveAync();
                await BindChildParentPath(moduleDelete, moduleDelete.ParentPath, "0");

                return new ServiceResultSuccess("Xóa Menu thành công");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> Deletes(IEnumerable<int> ids)
        {
            try
            {
                var moduleDeletes = await _dasRepo.Module.GetAllListAsync(m => ids.Contains(m.ID));
                if (moduleDeletes == null || moduleDeletes.Count() == 0)
                    return new ServiceResultError("Menu này không tồn tại hoặc đã bị xóa");

                foreach (var module in moduleDeletes)
                {
                    module.Status = (int)EnumCommon.Status.InActive;
                    await BindChildParentPath(module, module.ParentPath, "0");
                }
                await _dasRepo.Module.UpdateAsync(moduleDeletes);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa menu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion 

        #region Update
        public async Task<bool> ReUpdateModule(int IDparent = 1, int IDChannel = 0)
        {
            var listOldDynamic = await _dasRepo.Module.GetAll()
                .Where(x => x.Url.Contains("/Category/Index/"))
                .ToListAsync();
            await _dasRepo.Module.DeleteAsync(listOldDynamic);

            var listCateType = await _dasRepo.CategoryType.GetAll()
                .Where(x => x.Status == (int)EnumCategoryType.Status.Active && x.IsConfig == true)
                .ToListAsync();

            var listNewDynamic = new List<Module>();
            foreach (var item in listCateType)
            {
                var menu = new Module
                {
                    IDChannel = IDChannel,
                    ParentId = IDparent,
                    Url = "/Category/Index/" + item.Code,
                    Name = item.Name,
                    Icon = "fa-file-alt"
                };
                listNewDynamic.Add(menu);
            }
            await _dasRepo.Module.InsertAsync(listNewDynamic);
            await _dasRepo.SaveAync();
            return true;

        }

        public async Task<ServiceResult> Update(VMModule vmModule)
        {
            var moduleUpdate = await _dasRepo.Module.GetAsync(vmModule.ID);
            if (vmModule.Status == (int)EnumCommon.Status.InActive)
                return new ServiceResultError("Menu này không tồn tại hoặc đã bị xóa");
            var oldParent = moduleUpdate.ParentPath;
            BindUpdate(moduleUpdate, vmModule);
            await BindChildParentPath(moduleUpdate, oldParent, $"{moduleUpdate.ParentPath}|{moduleUpdate.ID}");
            var listExist = await _dasRepo.Module.GetAllListAsync(x => x.Status == (int)EnumCommon.Status.Active
            && x.Url.ToLower() != "#"
            && x.Url.ToLower() == moduleUpdate.Url.ToLower()
            && x.ID != moduleUpdate.ID);

            if (listExist != null && listExist.Count() >0)
            {
                return new ServiceResultError("Đường dẫn này đã tồn tại");
            }
            await _dasRepo.Module.UpdateAsync(moduleUpdate);
            await _dasRepo.SaveAync();
            await _permissionService.LoadCacheAllPermission();
            return new ServiceResultSuccess("Cập nhật Menu thành công");
        }
        #endregion

        #region Functions
        private void BindUpdate(Module module, VMModule vmModule)
        {
            module.SortOrder = vmModule.SortOrder;
            module.ParentId = vmModule.ParentId;
            module.ParentPath = "0";
            if (vmModule.ParentId > 0)
            {
                var pr = _dasRepo.Module.Get(vmModule.ParentId);
                if (pr != null)
                    module.ParentPath = pr.ParentPath + "|" + pr.ID;

            }
            module.Name = vmModule.Name?.Trim();
            module.Url = vmModule.Url.IsEmpty() ? "#" : vmModule.Url.Trim();
            module.Icon = vmModule.Icon?.Trim();
            module.Code = vmModule.Code;
        }

        private async Task<bool> BindChildParentPath(Module moduleUpdate, string oldParent, string newParent)
        {
            var parentPath = $"{oldParent}|{moduleUpdate.ID}";
            var childs = await _dasRepo.Module
                .GetAllListAsync(m => m.ParentId == moduleUpdate.ID || m.ParentPath.IndexOf(parentPath) == 0);
            if (childs.IsNotEmpty())
            {
                foreach (var child in childs)
                {
                    child.ParentPath = child.ParentPath.Replace(parentPath, newParent);
                }
                if (newParent == "0")
                {
                    foreach (var child in childs.Where(m => m.ParentId == moduleUpdate.ID))
                    {
                        child.ParentId = 0;
                    }
                }
                await _dasRepo.Module.UpdateAsync(childs);
                await _dasRepo.SaveAync();
            }
            return true;
        }

        private List<VMModule> RenderTreeOrder(IEnumerable<VMModule> tree, int parent, HashSet<int> nodes)
        {
            var result = new List<VMModule>();
            
            foreach (var item in tree.Where(x => x.ParentId.Equals(parent)).OrderBy(x => x.SortOrder))
            {
                if (tree.Where(x => x.ParentId.Equals(item.ID)).Count() > 0)
                {
                    if(CheckInTree(tree,nodes,item))
                        result.Add(item);
                    result.AddRange(RenderTreeOrder(tree, item.ID,nodes));
                }
                else
                {
                    if (CheckInTree(tree, nodes, item))
                        result.Add(item);
                }
            }
            return result;
        }

        private bool CheckInTree(IEnumerable<VMModule> root, HashSet<int> nodes, VMModule itemCheck)
        {
            if (nodes.Contains(itemCheck.ID)) return true;
            var childsHash = root.ToLookup(x => x.ParentId);
            var parentId = itemCheck.ID;
            var listChild = childsHash[parentId].ToList();
            foreach (var child in listChild)
            {
                if (CheckInTree(root,nodes,child))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public async Task<IEnumerable<Module>> GetModuleForCurrentUser()
        {
            var moduleForSuperAdmin = ModuleOfRole.SuperAdmin;
            var moduleCodeForAdmin = ModuleOfRole.Admin;

            UserData userData = await _cacheManagementServices.GetCurrentUserData();

            if (userData.IDOrgan == 0) //supper admin - nếu người dùng có vai trò khác thì chỉ lấy theo quyền supperadmin
            {
                var modules = await _dasRepo.Module.GetAllListAsync(m => m.Status == (int)EnumCommon.Status.Active && (m.IsShow == (int)EnumCommon.IsShow.Show || moduleForSuperAdmin.Contains(m.Code)));
                var moduleChilds = modules.Where(m => moduleForSuperAdmin.Contains(m.Code)).ToList();
                //get all moduleparent
                var moduleParentPaths = moduleChilds.Select(m => m.ParentPath).ToList();
                for (int i = 0; i < moduleParentPaths.Count; i++)
                {
                    moduleParentPaths[i] = string.Concat("|" + moduleParentPaths[i] + "|");
                }
                var moduleParents = modules.Where(m => moduleParentPaths.Any(x => x.Contains("|" + m.ID + "|"))).ToList();
                return moduleChilds.Union(moduleParents);
            }
            else if (userData.IsAdminOrgan == true)//admin cơ quan - nếu người dùng có vai trò khác thì chỉ lấy theo quyền admin cơ quan , do admin cơ quan có ID đơn vị  =0
            {
                var modules = await _dasRepo.Module.GetAllListAsync(m => m.Status == (int)EnumCommon.Status.Active && (m.IsShow == (int)EnumCommon.IsShow.Show || moduleCodeForAdmin.Contains(m.Code)));
                var moduleChilds = modules.Where(m => moduleCodeForAdmin.Contains(m.Code)).ToList();
                //get all moduleparent
                var moduleParentPaths = moduleChilds.Select(m => m.ParentPath).ToList();
                for (int i = 0; i < moduleParentPaths.Count; i++)
                {
                    moduleParentPaths[i] = string.Concat("|" + moduleParentPaths[i] + "|");
                }
                var moduleParents = modules.Where(m => moduleParentPaths.Any(x => x.Contains("|" + m.ID + "|"))).ToList();
                return moduleChilds.Union(moduleParents);
            }
            else
            {
                var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
                //var dict = await _cacheManagementServices.GetPermissionAndSetCache();
                if (dict.IsEmpty())
                {
                    return null;
                }

                var pers = dict.GetValueOrDefault(_userPrincipalService.UserId);
                if (pers.IsEmpty())
                {
                    return null;
                }

                var moduleIds = pers.Select(p => p.IdModule).ToList();
                var modules = await _dasRepo.Module.GetAllListAsync(m => m.Status == (int)EnumCommon.Status.Active && m.IsShow == (int)EnumCommon.IsShow.Show);
                //get all modulechild
                var moduleChilds = modules.Where(m => moduleIds.Contains(m.ID)).ToList();
                if (moduleChilds.IsEmpty())
                {
                    return null;
                }
                //get all moduleparent
                var moduleParentPaths = moduleChilds.Select(m => m.ParentPath).ToList();
                for (int i = 0; i < moduleParentPaths.Count; i++)
                {
                    moduleParentPaths[i] = string.Concat("|" + moduleParentPaths[i] + "|");
                }
                var moduleParents = modules.Where(m => moduleParentPaths.Any(x => x.Contains("|" + m.ID + "|"))).ToList();
                return moduleChilds.Union(moduleParents);
            }

        }
    }
}
