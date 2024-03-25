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
    public class CategoryTypeService : BaseMasterService, ICategoryTypeServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IModuleService _module;
        private readonly string[] _defaultCodes = new[] { "Name", "Code" };
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IDefaultDataService _defaultDataService;
        private ICacheManagementServices _cacheManagementServices;

        #endregion

        #region Ctor
        public CategoryTypeService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IModuleService module
            , IUserPrincipalService userPrincipalService
            , IDefaultDataService defaultDataService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _module = module;
            _userPrincipalService = userPrincipalService;
            _defaultDataService = defaultDataService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion

        #region Gets  

        public async Task<IEnumerable<CategoryType>> Gets()
        {
            return await _dasRepo.CategoryType.GetAllListAsync();
        }
        public async Task<IEnumerable<CategoryType>> GetsActive()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var rs = await _dasRepo.CategoryType.GetAllListAsync(n => n.Status == (int)EnumCategoryType.Status.Active && userData.IDOrgan == n.IDOrgan);
            return rs.OrderBy(n => n.Name);
        }

        public async Task<CategoryType> Get(object id)
        {
            return await _dasRepo.CategoryType.GetAsync(id);
        }

        public async Task<PaginatedList<VMCategoryType>> SearchByConditionPagging(CategoryTypeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from ct in _dasRepo.CategoryType.GetAll()
                       where (condition.Keyword.IsEmpty() || ct.Name.Contains(condition.Keyword)) && ct.Status == (int)EnumCategoryType.Status.Active
                      && ct.IDOrgan == userData.IDOrgan
                       orderby ct.UpdatedDate ?? ct.CreateDate descending
                       select _mapper.Map<VMCategoryType>(ct);


            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMCategoryType>(result, (int)total, condition.PageIndex, condition.PageSize);
        }
        /// <summary>
        /// Get cau hinh hoac cau hinh tam
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vmCategoryType"></param>
        /// <returns></returns>
        private async Task<List<VMUpdateCategoryTypeField>> GetCategoryTypeFields(VMCategoryType vmCategoryType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            if (Utils.IsEmpty(vmCategoryType))
                vmCategoryType = new VMCategoryType();

            if (vmCategoryType.IsConfig > 0)
            {
                //Su dung config >  lấy cấu hình
                return await (from typeField in _dasRepo.CategoryTypeField.GetAll()
                              where typeField.IDCategoryType == vmCategoryType.ID
                                  && typeField.IDOrgan == userData.IDOrgan
                              orderby typeField.Priority
                              select _mapper.Map<VMUpdateCategoryTypeField>(typeField)).ToListAsync();
            }
            else
            {
                var cateTypes = await GetsActive();
                //Fake dữ liệu
                return _defaultDataService.GetDefaultCategoryFields(cateTypes, vmCategoryType.Name, vmCategoryType.Code); 
            }
        }

        public async Task<IEnumerable<DataType>> GetDataTypes()
        {
            return await _dasRepo.DataType.GetAllListAsync();
        }
        public Dictionary<int, string> GetByTree(VMCategoryType vmCategoryType, IEnumerable<CategoryType> categoryTypes)
        {
            var prPath = $"{vmCategoryType.ParentPath}|{vmCategoryType.ID}|";
            var results = categoryTypes.Where(n => vmCategoryType.ID == 0 || (vmCategoryType.ID > 0 && n.ID != vmCategoryType.ID && vmCategoryType.ParentId != vmCategoryType.ID && vmCategoryType.ParentPath.IndexOf(prPath) != 0));
            //Render tree
            var treeModels = Utils.RenderTree(results.Select(n => new TreeModel<VMCategoryType>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.ParentId.GetValueOrDefault(0),
                ParentPath = n.ParentPath ?? string.Empty,
            }).ToList(), null, "--");

            return treeModels.ToDictionary(n => (int)n.ID, n => n.Name);
        }
        #endregion

        #region Create

        public async Task<VMCategoryType> Create()
        {
            var model = new VMCategoryType();
            var categoryTypes = await GetsActive();

            model.DictCategoryTypes = categoryTypes.ToDictionary(n => n.ID, n => n.Name);
            model.DictParents = GetByTree(model, categoryTypes);
            model.DictInputTypes = Utils.EnumToDic<EnumCategoryType.InputType>();
            model.DictDefaultValueTypes = Utils.EnumToDic<EnumCategoryType.DefaultValue>();
            //Generate default fields 
            model.CategoryTypeFields = await GetCategoryTypeFields(model);
            if (model.CategoryTypeFields.IsNotEmpty())
            {
                foreach (var item in model.CategoryTypeFields)
                {
                    item.IsDefault = _defaultCodes.Contains(item.Code);
                }
            }
            //Update Module
            return model;
        }

        public async Task<ServiceResult> Create(VMUpdateCategoryType vmCategoryType)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var categoryType = _mapper.Map<CategoryType>(vmCategoryType);
                BindUpdate(categoryType, vmCategoryType, userData.IDOrgan);
                categoryType.Status = (int)EnumCategoryType.Status.Active;
                if (await _dasRepo.CategoryType.IsCodeExist(categoryType.Code, (int)EnumCategoryType.Status.Active, categoryType.ID, userData.IDOrgan))
                {
                    return new ServiceResultError("Mã loại danh mục đã tồn tại");
                }
                await _dasRepo.CategoryType.InsertAsync(categoryType);
                await _dasRepo.SaveAync();
                await SyncCategoryTypeFields(vmCategoryType.CategoryTypeFields, categoryType);
                //Update Module
                return new ServiceResultSuccess("Thêm loại danh mục thành công");
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion

        #region Update
        public async Task<VMCategoryType> Update(int? id)
        {
            var cateType = await Get(id.Value);
            var model = _mapper.Map<VMCategoryType>(cateType);
            var categoryTypes = await GetsActive();

            model.DictCategoryTypes = categoryTypes.Where(n => n.ID != id.Value).ToDictionary(n => n.ID, n => n.Name);
            model.DictParents = GetByTree(model, categoryTypes);

            model.DictInputTypes = Utils.EnumToDic<EnumCategoryType.InputType>();
            model.DictDefaultValueTypes = Utils.EnumToDic<EnumCategoryType.DefaultValue>();

            var categoryTypeFields = await GetCategoryTypeFields(model);
            model.IsUsed = await IsUsed(cateType);
            var categoryTypeFieldCounts = await GetCategoryTypeFieldCounts(categoryTypeFields.Select(n => n.ID).ToArray());
            model.CategoryTypeFields = categoryTypeFields.OrderBy(n => n.Priority);
            if (model.CategoryTypeFields.IsNotEmpty())
            {
                foreach (var item in model.CategoryTypeFields)
                {
                    var isUsed = categoryTypeFieldCounts.FirstOrNewObj(n => n.ID == item.ID).IsUsed;
                    item.IsDelete = !isUsed;
                    item.IsUpdate = !isUsed;
                    item.IsDefault = _defaultCodes.Contains(item.Code);
                }
            }
            //Update Module
            return model;
        }
        public async Task<ServiceResult> Update(VMUpdateCategoryType vmCategoryType)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var categoryType = await _dasRepo.CategoryType.GetAsync(vmCategoryType.ID);
                if (categoryType == null || categoryType.Status == (int)EnumCategoryType.Status.InActive)
                    return new ServiceResultError("Loại danh mục này hiện không tồn tại hoặc đã bị xóa");
                var oldParent = vmCategoryType.ParentPath;
                BindUpdate(categoryType, vmCategoryType, userData.IDOrgan);
                if (await _dasRepo.CategoryType.IsCodeExist(categoryType.Code, (int)EnumCategoryType.Status.Active, categoryType.ID, userData.IDOrgan))
                {
                    return new ServiceResultError("Mã loại danh mục đã tồn tại");
                }
                await _dasRepo.CategoryType.UpdateAsync(categoryType);
                await SyncCategoryTypeFields(vmCategoryType.CategoryTypeFields, categoryType);
                await BindChildParentPath(categoryType, oldParent, $"{categoryType.ParentPath}|{categoryType.ID}");
                await _dasRepo.SaveAync();
                //Update Module
                return new ServiceResultSuccess("Cập nhật loại danh mục thành công");
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
                var category = await _dasRepo.CategoryType.GetAsync(id);
                if (category == null || category.Status == (int)EnumCategoryType.Status.InActive)
                    return new ServiceResultError("Loại danh mục này hiện không tồn tại hoặc đã bị xóa");

                var isUsed = await _dasRepo.Category.AnyAsync(n => n.IdCategoryType == id);
                if (isUsed)
                    return new ServiceResultError("Loại danh mục này hiện đang được sử dụng, không được phép xoá");

                category.Status = (int)EnumCategoryType.Status.InActive;
                await _dasRepo.CategoryType.UpdateAsync(category);

                var childs = await _dasRepo.CategoryTypeField.GetAllListAsync(n => n.IDCategoryType == id);
                foreach (var child in childs)
                {
                    child.Status = (int)EnumCategoryType.Status.InActive;
                }
                await _dasRepo.CategoryTypeField.UpdateAsync(childs);
                await _dasRepo.SaveAync();
                await BindChildParentPath(category, category.ParentPath, "0");

                //Update Module
                return new ServiceResultSuccess("Xóa loại danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);

            }
        }

        public async Task<ServiceResult> Delete(IEnumerable<int> ids)
        {
            try
            {
                var categoryTypeDeletes = await _dasRepo.CategoryType.GetAllListAsync(n => ids.Contains(n.ID));
                if (categoryTypeDeletes == null || categoryTypeDeletes.Count() == 0)
                    return new ServiceResultError("Loại danh mục đã chọn hiện không tồn tại hoặc đã bị xóa");

                var categoryTypeUsed = await _dasRepo.Category.GetAllListAsync(n => ids.Contains(n.IdCategoryType));
                if (categoryTypeUsed.IsNotEmpty())
                {
                    var usedIds = categoryTypeUsed.Select(n => n.IdCategoryType).Distinct().ToArray();
                    var deletedNames = categoryTypeDeletes.Where(m => usedIds.Contains(m.ID)).Select(n => n.Name);
                    return new ServiceResultError("Loại danh mục " + string.Join(", ", deletedNames) + " hiện đang được sử dụng, không được phép xoá");
                }

                foreach (var item in categoryTypeDeletes)
                {
                    item.Status = (int)EnumCategoryType.Status.InActive;
                    await BindChildParentPath(item, item.ParentPath, "0");
                }
                await _dasRepo.CategoryType.UpdateAsync(categoryTypeDeletes);
                var childs = await _dasRepo.CategoryTypeField.GetAllListAsync(n => ids.Contains(n.IDCategoryType));
                foreach (var child in childs)
                {
                    child.Status = (int)EnumCategoryType.Status.InActive;
                }
                await _dasRepo.CategoryTypeField.UpdateAsync(childs);
                await _dasRepo.SaveAync();
                //Update Module
                return new ServiceResultSuccess("Xóa loại danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion

        #region Funtions

        private void BindUpdate(CategoryType categoryTypeUpdate, VMUpdateCategoryType vmCategoryType, int idOrgan)
        {
            categoryTypeUpdate.ParentId = vmCategoryType.ParentId;
            categoryTypeUpdate.ParentPath = "0";
            if (vmCategoryType.ParentId > 0)
            {
                var pr = _dasRepo.CategoryType.Get(vmCategoryType.ParentId);
                if (pr != null)
                    categoryTypeUpdate.ParentPath = pr.ParentPath + "|" + pr.ID;

            }
            categoryTypeUpdate.IsConfig = vmCategoryType.IsConfig > 0;
            categoryTypeUpdate.Code = vmCategoryType.Code;
            categoryTypeUpdate.Name = vmCategoryType.Name;
            categoryTypeUpdate.Description = vmCategoryType.Description;
            categoryTypeUpdate.IDOrgan = idOrgan;
        }

        private async Task<bool> SyncCategoryTypeFields(IEnumerable<VMUpdateCategoryTypeField> categoryTypeFields, CategoryType categoryType)

        {
            if (categoryType.IsConfig)
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var inserts = new List<CategoryTypeField>();
                var updates = new List<CategoryTypeField>();
                var deletes = new List<CategoryTypeField>();
                var dbCategoryTypeFields = await _dasRepo.CategoryTypeField.GetAllListAsync(n => n.IDCategoryType == categoryType.ID);
                if (categoryTypeFields.IsNotEmpty())
                {
                    var permissionGroupPers = new List<CategoryTypeField>();
                    var dtNow = DateTime.Now;
                    foreach (var item in categoryTypeFields)
                    {
                        item.IDCategoryType = categoryType.ID;
                        if (item.InputType != (int)EnumCategoryType.InputType.CategoryType)
                        {
                            item.IDCategoryTypeRelated = 0;
                        }
                        var detail = dbCategoryTypeFields.FirstOrDefault(n => n.ID == item.ID);
                        if (detail != null)
                        {
                            detail.Bind(item.KeyValue());
                            detail.IDOrgan = userData.IDOrgan;
                            updates.Add(detail);
                        }
                        else
                        {
                            var newItem = _mapper.Map<CategoryTypeField>(item);
                            newItem.ID = 0;
                            newItem.IDOrgan = userData.IDOrgan;
                            inserts.Add(newItem);
                        }
                    }
                    if (Utils.IsNotEmpty(dbCategoryTypeFields))
                    {
                        var updateIds = categoryTypeFields.Select(n => n.ID).ToArray();
                        //lấy ds các bản ghi đc thêm vào db nhưng ko có trong ds id trên form => xóa
                        deletes = dbCategoryTypeFields.Where(n => !updateIds.Contains(n.ID)).ToList();
                    }
                }
                if (deletes.IsNotEmpty() || updates.IsNotEmpty() || inserts.IsNotEmpty())
                {
                    if (deletes.IsNotEmpty())
                    {
                        await _dasRepo.CategoryTypeField.DeleteAsync(deletes);
                    }
                    if (updates.IsNotEmpty())
                    {
                        await _dasRepo.CategoryTypeField.UpdateAsync(updates);
                    }
                    if (inserts.IsNotEmpty())
                    {
                        await _dasRepo.CategoryTypeField.InsertAsync(inserts);
                    }
                    await _dasRepo.SaveAync();
                    return true;
                }
            }
            else
            {
                var dbCategoryTypeFields = await _dasRepo.CategoryTypeField.GetAllListAsync(n => n.IDCategoryType == categoryType.ID);
                if (dbCategoryTypeFields.IsNotEmpty())
                {
                    await _dasRepo.CategoryTypeField.DeleteAsync(dbCategoryTypeFields);
                }
                await _dasRepo.SaveAync();
                return true;
            }
            return false;
        }

        private async Task<bool> IsUsed(CategoryType categoryType)
        {
            return await _dasRepo.Category.AnyAsync(n => n.IdCategoryType == categoryType.ID && n.Status == (int)EnumCategory.Status.Active);

        }

        /// <summary>
        /// Lay ra cac truong da dc su dung hay chua
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<IEnumerable<VMCategoryTypeField>> GetCategoryTypeFieldCounts(int[] ids)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from cf in _dasRepo.CategoryField.GetAll()
                       where ids.IsNotEmpty() && ids.Contains(cf.IDCategoryTypeField) && cf.Status == (int)EnumCategory.Status.Active
                                  && cf.IDOrgan == userData.IDOrgan

                       group cf by cf.IDCategoryTypeField into g
                       select new VMCategoryTypeField { ID = g.Key, IsUsed = g.Count() > 0 };

            return await temp.ToListAsync();

        }
        private async Task<bool> BindChildParentPath(CategoryType categoryType, string oldParent, string newParent)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var parentPath = $"{oldParent}|{categoryType.ID}";
            var childs = await _dasRepo.CategoryType.GetAllListAsync(n => (n.ParentId == categoryType.ID || n.ParentPath.IndexOf(parentPath) == 0) && n.IDOrgan == userData.IDOrgan);
            if (childs.IsNotEmpty())
            {
                foreach (var child in childs)
                {
                    child.ParentPath = child.ParentPath.Replace(parentPath, newParent);
                }
                if (newParent == "0")
                {
                    foreach (var child in childs.Where(n => n.ParentId == categoryType.ID))
                    {
                        child.ParentId = 0;
                    }
                }
                await _dasRepo.CategoryType.UpdateAsync(childs);
                await _dasRepo.SaveAync();
            }

            return true;
        }

        #endregion
    }
}