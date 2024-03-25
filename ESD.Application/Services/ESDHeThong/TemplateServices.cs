using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using ESD.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using Newtonsoft.Json;
using ESD.Utility;
using ESD.Application.Constants;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;

namespace ESD.Application.Services
{
    public class TemplateServices : BaseMasterService, ITemplateServices
    {
        private readonly IMapper _mapper;
        private readonly IStgFileClientService _stgFileClientService;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;
        #region Ctor
        public TemplateServices(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IStgFileClientService stgFileClientService
            , IUserPrincipalService userPrincipalService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _stgFileClientService = stgFileClientService;
            _userPrincipalService = userPrincipalService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion
        #region Create & Search
        public async Task<ServiceResult> Create(Template model)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //check exist unique field
            var listExist = await _dasRepo.DeliveryRecord.GetAll().Where(m => (m.Status != (int)EnumTemplate.Status.Inactive)).ToListAsync();
            if (IsExisted(listExist))
                if (IsExisted(listExist.Where(m => m.Code == model.Code)))
                    return new ServiceResultError("Mã số biên bản đã tồn tại!");

            //update data
            model.Status = (int)EnumTemplate.Status.Active;
            model.IDOrgan = userData.IDOrgan;
            await _dasRepo.Template.InsertAsync(model);
            await _dasRepo.SaveAync();
            if (model.ID == 0)
                return new ServiceResultError("Thêm mới mẫu biên bản không thành công");
            return new ServiceResultSuccess("Thêm mới mẫu biên bản thành công");
        }

        public async Task<ServiceResult> Create(VMTemplate model)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //check exist unique field
            var listExist = await _dasRepo.Template.GetAll().Where(m => (m.Status != (int)EnumTemplate.Status.Inactive)).ToListAsync();
            if (IsExisted(listExist))
                if (IsExisted(listExist.Where(m => m.Code == model.Code)))
                    return new ServiceResultError("Mã số biên bản đã tồn tại!");
            model.IDOrgan = userData.IDOrgan;
            var temp = _mapper.Map<Template>(model);

            if (model.File != null)
            {
                var stgFile = new VMStgFile
                {
                    File = model.File,
                    FileName = model.File.FileName,
                    FileType = (int)EnumFile.Type.Template,
                    IsTemp = false
                };

                var resultUpload = await _stgFileClientService.Upload(stgFile);
                if (resultUpload.Code == null || resultUpload.Data == null || !resultUpload.Code.Equals(CommonConst.Success))
                {
                    return new ServiceResultError("Thêm mới biên bản không thành công!");
                }

                var objUpload = JsonConvert.DeserializeObject<VMStgFile>(resultUpload.Data.ToString());
                temp.IDStgFile = objUpload.ID;

                // set IsTemp for old avatar file
                //if (model.IDStgFile.HasValue)
                //{
                //    await _stgFileClientService.MarkFileTemp(model.IDStgFile.Value);
                //}
            }
            await _dasRepo.Template.InsertAsync(temp);
            await _dasRepo.SaveAync();
            if (temp.ID == 0)
                return new ServiceResultError("Thêm mới mẫu biên bản không thành công");
            var rs = await SyncCategoryTypeFields(model.TemplateParam, temp);
            if (!rs)
                return new ServiceResultSuccess("Thêm mới tham số Template thất bại");
            return new ServiceResultSuccess("Thêm mới mẫu biên bản thành công");
        }

        public async Task<Template> Get(object id)
        {
            return await _dasRepo.Template.GetAsync(id);
        }

        public async Task<IEnumerable<Template>> Gets()
        {
            return await _dasRepo.Template.GetAllListAsync(x => x.Status != (int)EnumTemplate.Status.Inactive);
        }
        public async Task<VMTemplate> GetTemplate(int id)
        {
            var temp = await _dasRepo.Template.GetAsync(id);
            if (temp == null || temp.ID == 0 || temp.Status == (int)EnumTemplate.Status.Inactive)
                return null;
            var model = _mapper.Map<VMTemplate>(temp);
            return model;
        }
        public async Task<IEnumerable<Template>> GetActiveTemplate()
        {
            return await _dasRepo.Template.GetAllListAsync(x => x.Status != (int)EnumTemplate.Status.Inactive && x.IDStgFile != null);
        }
        public async Task<IEnumerable<VMTemplateParam>> GetTemplateParam(int id)
        {
            var temp = from template in _dasRepo.TemplateParam.GetAll()
                       where template.IDTemplate == id
                       select _mapper.Map<VMTemplateParam>(template);

            return await temp.ToListAsync();
        }
        public async Task<PaginatedList<VMTemplate>> SearchListTemplateConditionPagging(TemplateCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from template in _dasRepo.Template.GetAll()
                       where template.Status == (int)EnumCommon.Status.Active
                       && (condition.Keyword.IsEmpty() || template.Name.Contains(condition.Keyword)
                       || template.Code.Contains(condition.Keyword) || template.Description.Contains(condition.Keyword))
                       && (template.IDOrgan == userData.IDOrgan || template.IDOrgan == 0)
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
        #endregion
        #region Delete
        public async Task<ServiceResult> Delete(object id)
        {
            var record = await _dasRepo.Template.GetAsync(id);
            if (record == null || record.Status == (int)EnumTemplate.Status.Inactive)
                return new ServiceResultError("Không tồn tại biên bản này");
            record.Status = (int)EnumDeliveryRecord.Status.Inactive;
            await _dasRepo.Template.UpdateAsync(record);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa mẫu biên bản thành công");
        }
        public async Task<ServiceResult> DeleteListTemplate(int[] ids)
        {
            var lstTemp = await _dasRepo.Template.GetAllListAsync(n => ids.Contains(n.ID));
            if (!IsExisted(lstTemp))
                return new ServiceResultError("Không tồn tại các mẫu biên bản này");
            foreach (var item in lstTemp)
            {
                item.Status = (int)EnumTemplate.Status.Inactive;
            }

            await _dasRepo.Template.UpdateAsync(lstTemp);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa mẫu biên bản thành công");
        }
        #endregion
        #region Update
        public async Task<ServiceResult> Update(Template model)
        {
            await _dasRepo.Template.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật mẫu biên bản thành công");
        }

        public async Task<ServiceResult> Update(VMTemplate model)
        {
            //check exist unique field
            var temp = await _dasRepo.Template.GetAsync(model.ID);
            if (temp == null || temp.Status == (int)EnumTemplate.Status.Inactive)
                return new ServiceResultError("Template không tồn tại!");

            //check exist unique field
            var listExist = await _dasRepo.Template.GetAll().Where(m => (m.Status != (int)EnumTemplate.Status.Inactive) && m.ID != model.ID).ToListAsync();
            if (IsExisted(listExist))
                if (IsExisted(listExist.Where(m => m.Code == model.Code)))
                    return new ServiceResultError("Mã số biên bản đã tồn tại!");

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            if (model.File != null)
            {
                var stgFile = new VMStgFile
                {
                    File = model.File,
                    FileName = model.File.FileName,
                    FileType = (int)EnumFile.Type.Template,
                    IsTemp = false
                };

                var resultUpload = await _stgFileClientService.Upload(stgFile);
                if (resultUpload.Code == null || resultUpload.Data == null || !resultUpload.Code.Equals(CommonConst.Success))
                {
                    return new ServiceResultError("Cập nhật mẫu biên bản không thành công!");
                }

                var objUpload = JsonConvert.DeserializeObject<VMStgFile>(resultUpload.Data.ToString());
                temp.IDStgFile = objUpload.ID;

                // set IsTemp for old avatar file
                if (model.IDStgFile.HasValue)
                {
                    await _stgFileClientService.MarkFileTemp(model.IDStgFile.Value);
                }
                model.IDStgFile = objUpload.ID;
            }
            else
                model.IDStgFile = temp.IDStgFile;
            model.IDOrgan = userData.IDOrgan;
            _mapper.Map(model, temp);
            await _dasRepo.Template.UpdateAsync(temp);
            await _dasRepo.SaveAync();
            var rs = await SyncCategoryTypeFields(model.TemplateParam, temp);
            if (!rs)
                return new ServiceResultSuccess("Cập nhật tham số Template thất bại");
            return new ServiceResultSuccess("Cập nhật Template thành công");
        }
        #endregion
        #region Function

        private bool IsExisted<T>(IEnumerable<T> entity)
        {
            if (entity == null || entity.Count() == 0)
                return false;
            return true;
        }
        private async Task<bool> SyncCategoryTypeFields(IEnumerable<VMTemplateParam> model, Template entity)

        {
            var inserts = new List<TemplateParam>();
            var updates = new List<TemplateParam>();
            var deletes = new List<TemplateParam>();
            var dbTempPara = await _dasRepo.TemplateParam.GetAllListAsync(n => n.IDTemplate == entity.ID);
            if (model.IsNotEmpty())
            {
                foreach (var item in model)
                {
                    item.IDTemplate = entity.ID;
                    var detail = dbTempPara.FirstOrDefault(n => n.ID == item.ID);
                    if (detail != null)
                    {
                        detail.Bind(item.KeyValue());
                        updates.Add(detail);
                    }
                    else
                    {
                        var newItem = _mapper.Map<TemplateParam>(item);
                        inserts.Add(newItem);
                    }
                }
                if (Utils.IsNotEmpty(dbTempPara))
                {
                    var updateIds = model.Select(n => n.ID).ToArray();
                    //lấy ds các bản ghi đc thêm vào db nhưng ko có trong ds id trên form => xóa
                    deletes = dbTempPara.Where(n => !updateIds.Contains(n.ID)).ToList();
                }
            }
            if (deletes.IsNotEmpty() || updates.IsNotEmpty() || inserts.IsNotEmpty())
            {
                if (deletes.IsNotEmpty())
                {
                    await _dasRepo.TemplateParam.DeleteAsync(deletes);
                }
                if (updates.IsNotEmpty())
                {
                    await _dasRepo.TemplateParam.UpdateAsync(updates);
                }
                if (inserts.IsNotEmpty())
                {
                    await _dasRepo.TemplateParam.InsertAsync(inserts);
                }
                await _dasRepo.SaveAync();
                return true;
            }

            return false;
        }
        #endregion
    }
}
