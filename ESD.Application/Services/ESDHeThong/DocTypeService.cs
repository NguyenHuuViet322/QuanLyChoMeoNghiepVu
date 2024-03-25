using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class DocTypeService : BaseMasterService, IDocTypeServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IDefaultDataService _defaultDataService;
        private readonly ICacheManagementServices _cacheManagementServices;

        #endregion

        #region Ctor
        public DocTypeService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService userPrincipalService
            , IDefaultDataService defaultDataService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
            _defaultDataService = defaultDataService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion

        #region Gets  

        public async Task<IEnumerable<DocType>> Gets()
        {
            return await _dasRepo.DocType.GetAllListAsync();
        }
        public async Task<IEnumerable<DocType>> GetsActive()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var rs = await _dasRepo.DocType.GetAllListAsync(n => n.Status == (int)EnumDocType.Status.Active && (n.IDOrgan == userData.IDOrgan));
            return rs.OrderBy(n => n.Name);
        }

        public async Task<DocType> Get(object id)
        {
            return await _dasRepo.DocType.GetAsync(id);
        }  
        public async Task<VMDocTypeField> GetDocTypeField(object id)
        {
            return _mapper.Map<VMDocTypeField>(await _dasRepo.DocTypeField.GetAsync(id));
        }

        public async Task<PaginatedList<VMDocType>> SearchByConditionPagging(DocTypeCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from dt in _dasRepo.DocType.GetAll()
                       where (condition.Keyword.IsEmpty() || dt.Name.Contains(condition.Keyword))
                       && dt.Status == (int)EnumDocType.Status.Active
                       && (dt.IDOrgan == userData.IDOrgan) 
                       orderby dt.UpdatedDate ?? dt.CreateDate descending
                       select _mapper.Map<VMDocType>(dt);


            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMDocType>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<IEnumerable<DataType>> GetDataTypes()
        {
            return await _dasRepo.DataType.GetAllListAsync();
        }
        #endregion Gets

        #region Create

        public async Task<VMDocType> Create()
        {
            var model = new VMDocType();
            model.DictCategoryTypes = (await GetsCategoryTypes()).ToDictionary(n => n.ID, n => n.Name);
            //Generate default fields 
            model.DocTypeFields = await GetDocTypeFields(model, 0);
            model.DictInputTypes = Utils.EnumToDic<EnumDocType.InputType>();
            model.DictType = Utils.EnumToDic<EnumDocType.Type>();
            //if (model.DocTypeFields.IsNotEmpty())
            //{
            //    foreach (var item in model.DocTypeFields)
            //    {
            //        item.IsBase = item.IsBase;
            //    }
            //}
            return model;
        }
        public async Task<ServiceResult> Create(VMUpdateDocType vmDocType)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var docType = _mapper.Map<DocType>(vmDocType);
                docType.IDOrgan = userData.IDOrgan;
                BindUpdate(docType, vmDocType);
                docType.Status = (int)EnumDocType.Status.Active;
                if (await _dasRepo.DocType.IsCodeExist(docType.Code, (int)EnumDocType.Status.Active))
                {
                    return new ServiceResultError("Mã khung biên mục đã tồn tại");
                }
                await _dasRepo.DocType.InsertAsync(docType);
                await _dasRepo.SaveAync();
                await SyncDocTypeFields(vmDocType.DocTypeFields, docType);
                return new ServiceResultSuccess("Thêm khung biên mục thành công");
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Create

        #region Update
        public async Task<VMDocType> Update(int? id)
        {
            var docType = await Get(id.Value);


            var model = _mapper.Map<VMDocType>(docType);
            model.DictCategoryTypes = (await GetsCategoryTypes()).ToDictionary(n => n.ID, n => n.Name);
            model.DictInputTypes = Utils.EnumToDic<EnumDocType.InputType>();
            var docTypeFields = await GetDocTypeFields(model, docType.Type);
            model.DictType = Utils.EnumToDic<EnumDocType.Type>();
            model.IsUsed = await IsUsed(docType);
            var docTypeFieldCounts = await GetDocTypeFieldCounts(docTypeFields.Select(n => n.ID).ToArray());
            model.DocTypeFields = docTypeFields.OrderBy(n => n.Priority);
            if (model.DocTypeFields.IsNotEmpty())
            {
                foreach (var item in model.DocTypeFields)
                {
                    var isUsed = docTypeFieldCounts.FirstOrNewObj(n => n.ID == item.ID).IsUsed;
                    item.IsDelete = !isUsed;
                    item.IsUpdate = !isUsed;
                    //item.IsBase = _defaultCodes.Contains(item.Code);
                }
            }
            return model;
        }
        public async Task<ServiceResult> Update(VMUpdateDocType vmDocType)
        {
            try
            {
                var docType = await _dasRepo.DocType.GetAsync(vmDocType.ID);
                if (docType == null || docType.Status == (int)EnumDocType.Status.InActive)
                    return new ServiceResultError("Khung biên mục này hiện không được phép chỉnh sửa");

                if (docType.IsBase)
                    return new ServiceResultError("Khung biên mục này hiện không được phép sửa");


                BindUpdate(docType, vmDocType);
                docType.IDOrgan = docType.IDOrgan;
                if (await _dasRepo.DocType.IsCodeExist(docType.Code, (int)EnumDocType.Status.InActive, docType.ID))
                {
                    return new ServiceResultError("Mã khung biên mục đã tồn tại");
                }
                await _dasRepo.DocType.UpdateAsync(docType);
                await SyncDocTypeFields(vmDocType.DocTypeFields, docType);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật khung biên mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                var docType = await _dasRepo.DocType.GetAsync(id);
                if (docType == null || docType.Status == (int)EnumDocType.Status.InActive)
                    return new ServiceResultError("Khung biên mục này hiện không tồn tại hoặc đã bị xóa");

                if (docType.IsBase)
                    return new ServiceResultError("Khung biên mục này không được phép xóa");

                //var isUsed = await _dasRepo.Doc.AnyAsync(n => n.IdDocType == id);
                //if (isUsed)
                //    return new ServiceResultError("Khung biên mục này hiện đang được sử dụng, không được phép xoá");

                docType.Status = (int)EnumDocType.Status.InActive;
                await _dasRepo.DocType.UpdateAsync(docType);

                var childs = await _dasRepo.DocTypeField.GetAllListAsync(n => n.IDDocType == id);
                foreach (var child in childs)
                {
                    child.Status = (int)EnumDocType.Status.InActive;
                }
                await _dasRepo.DocTypeField.UpdateAsync(childs);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa khung biên mục thành công");
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
                var docTypes = await _dasRepo.DocType.GetAllListAsync(n => ids.Contains(n.ID));
                if (docTypes == null || docTypes.Count() == 0)
                    return new ServiceResultError("Khung biên mục đã chọn hiện không tồn tại hoặc đã bị xóa");


                if (docTypes.Any(n => n.IsBase))
                {
                    var names = new List<string>();
                    foreach (var item in docTypes)
                    {
                        if (item.IsBase)
                            names.Add(item.Name);
                    }
                    return new ServiceResultError($"Khung biên mục {string.Join(", ", names)} không được phép xoá!");
                }
                foreach (var pos in docTypes)
                {
                    pos.Status = (int)EnumDocType.Status.InActive;
                }
                await _dasRepo.DocType.UpdateAsync(docTypes);
                var childs = await _dasRepo.DocTypeField.GetAllListAsync(n => ids.Contains(n.IDDocType));
                foreach (var child in childs)
                {
                    child.Status = (int)EnumDocType.Status.InActive;
                }
                await _dasRepo.DocTypeField.UpdateAsync(childs);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa khung biên mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete

        #region ChangeType

        public async Task<VMDocType> ChangeType(int id, int type)
        {
            var docType = await Get(id) ?? new DocType();
            var model = _mapper.Map<VMDocType>(docType);
            model.DictCategoryTypes = (await GetsCategoryTypes()).ToDictionary(n => n.ID, n => n.Name);
            model.DictInputTypes = Utils.EnumToDic<EnumDocType.InputType>();
            var docTypeFields = await GetDocTypeFields(model, type);
            model.DictType = Utils.EnumToDic<EnumDocType.Type>();
            var docTypeFieldCounts = await GetDocTypeFieldCounts(docTypeFields.Select(n => n.ID).ToArray());
            model.DocTypeFields = docTypeFields.OrderBy(n => n.Priority);
            return model;
        }


        #endregion ChangeType

        #region Funtions
        /// <summary>
        /// Get cau hinh hoac cau hinh tam
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vmDocType"></param>
        /// <returns></returns>
        private async Task<List<VMDocTypeField>> GetDocTypeFields(VMDocType vmDocType, int type)
        {
            if (Utils.IsEmpty(vmDocType))
                vmDocType = new VMDocType();

            if (vmDocType.ID > 0)
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                //Su dung config > lấy cấu hình
                return await (from dt in _dasRepo.DocTypeField.GetAll()
                              where dt.IDDocType == vmDocType.ID && (dt.IDOrgan == userData.IDOrgan)
                              orderby dt.Priority
                              select _mapper.Map<VMDocTypeField>(dt)).ToListAsync();
            }
            else
            {
                var cateTypes = await GetsCategoryTypes();
                return _defaultDataService.GetDefaultDocTypeFields(cateTypes, vmDocType, type);
            }
        }
        private void BindUpdate(DocType docType, VMUpdateDocType vmDocType)
        {
            docType.Code = vmDocType.Code;
            docType.Name = vmDocType.Name;
            docType.Type = vmDocType.Type.GetValueOrDefault(0);
        }

        private async Task<bool> SyncDocTypeFields(IEnumerable<VMDocTypeField> docTypeFields, DocType docType)
        {
            //if (docType.IsConfig)
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var inserts = new List<DocTypeField>();
                var updates = new List<DocTypeField>();
                var deletes = new List<DocTypeField>();
                var dbDocTypeFields = await _dasRepo.DocTypeField.GetAllListAsync(n => n.IDDocType == docType.ID);
                if (docTypeFields.IsNotEmpty())
                {
                    var permissionGroupPers = new List<DocTypeField>();
                    var dtNow = DateTime.Now;
                    foreach (var item in docTypeFields)
                    {
                        item.IDDocType = docType.ID;
                        if (item.InputType != (int)EnumCategoryType.InputType.CategoryType)
                        {
                            item.IDCategoryTypeRelated = 0;
                        }
                        var detail = dbDocTypeFields.FirstOrDefault(n => n.ID == item.ID);
                        if (detail != null)
                        {
                            detail.Bind(item.KeyValue());
                            detail.IsBase = item.IsBase == 1;
                            updates.Add(detail);
                        }
                        else
                        {
                            var newItem = _mapper.Map<DocTypeField>(item);
                            newItem.ID = 0;
                            newItem.IsBase = item.IsBase == 1;
                            newItem.IDOrgan = userData.IDOrgan;
                            inserts.Add(newItem);
                        }
                    }
                    if (Utils.IsNotEmpty(dbDocTypeFields))
                    {
                        var updateIds = docTypeFields.Select(n => n.ID).ToArray();
                        //lấy ds các bản ghi đc thêm vào db nhưng ko có trong ds id trên form => xóa
                        deletes = dbDocTypeFields.Where(n => !updateIds.Contains(n.ID)).ToList();
                    }
                }
                if (deletes.IsNotEmpty() || updates.IsNotEmpty() || inserts.IsNotEmpty())
                {
                    if (deletes.IsNotEmpty())
                    {
                        await _dasRepo.DocTypeField.DeleteAsync(deletes);
                    }
                    if (updates.IsNotEmpty())
                    {
                        await _dasRepo.DocTypeField.UpdateAsync(updates);
                    }
                    if (inserts.IsNotEmpty())
                    {
                        await _dasRepo.DocTypeField.InsertAsync(inserts);
                    }
                    await _dasRepo.SaveAync();
                    return true;
                }
            }
            //else
            //{
            //    var dbDocTypeFields = await _dasRepo.DocTypeField.GetAllListAsync(n => n.IDDocType == docType.ID);
            //    if (dbDocTypeFields.IsNotEmpty())
            //    {
            //        await _dasRepo.DocTypeField.DeleteAsync(dbDocTypeFields);
            //    }
            //    await _dasRepo.SaveAync();
            //    return true;
            //}
            return false;
        }

        private async Task<bool> IsUsed(DocType DocType)
        {
            return false;
        }

        /// <summary>
        /// Lay ra cac truong da dc su dung hay chua
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<IEnumerable<VMDocTypeField>> GetDocTypeFieldCounts(int[] ids)
        {
            var temp = from cf in _dasRepo.DocField.GetAll()
                       where ids.IsNotEmpty() && ids.Contains(cf.IDDocTypeField)
                       group cf by cf.IDDocTypeField into g
                       select new VMDocTypeField { ID = g.Key, IsUsed = g.Count() > 0 };

            return await temp.ToListAsync();
        }
        private async Task<IEnumerable<CategoryType>> GetsCategoryTypes()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var rs = await _dasRepo.CategoryType.GetAllListAsync(n => n.Status == (int)EnumCategoryType.Status.Active && n.IDOrgan == userData.IDOrgan);
            return rs.OrderBy(n => n.Name);
        }

        #endregion Funtions
    }
}