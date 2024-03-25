using AutoMapper;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Utility;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class LanguageService : BaseMasterService, ILanguageServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        #endregion

        #region Ctor
        public LanguageService(IDasRepositoryWrapper dasRepository, IMapper mapper, ILoggerManager logger) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region Get
        public async Task<Language> Get(object id)
        {
            return await _dasRepo.Language.GetAsync(id);
        }
        public async Task<IEnumerable<Language>> Gets()
        {
            return await _dasRepo.Language.GetAllListAsync();
        }
        public async Task<VMLanguage> GetLanguage(int id)
        {
            var temp = from r in _dasRepo.Language.GetAll()
                       where r.ID == id
                       select new VMLanguage
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description,
                           CreatedBy = r.CreatedBy,
                           CreateDate = r.CreateDate,
                           UpdatedDate = r.UpdatedDate,
                           UpdatedBy = r.UpdatedBy
                       };
            var result = await temp.FirstOrDefaultAsync();
            return result;

        }
        public async Task<VMLanguage> GetLanguageDetail(int id)
        {
            var temp = from r in _dasRepo.Language.GetAll()
                       where r.ID == id
                       select new VMLanguage
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description
                       };
            var result = await temp.FirstOrDefaultAsync();
            return result;
        }
        public async Task<PaginatedList<VMLanguage>> SearchByConditionPagging(LanguageCondition condition)
        {
            var temp = from r in _dasRepo.Language.GetAll().Where(x => x.Status == (int)EnumLanguage.Status.Active)
                       where (string.IsNullOrEmpty(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Code.Contains(condition.Keyword))
                       orderby r.ID descending
                       select new VMLanguage
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description
                       };
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            PaginatedList<VMLanguage> model = new PaginatedList<VMLanguage>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<IEnumerable<VMLanguage>> GetListByCondition(LanguageCondition condition)
        {
            var temp = from r in _dasRepo.Language.GetAll().Where(x => x.Status == (int)EnumLanguage.Status.Active)
                       where (string.IsNullOrEmpty(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Code.Contains(condition.Keyword))
                       orderby r.ID descending
                       select new VMLanguage
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description
                       };
            var result = await temp.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Language>> GetsActive()
        {
            return await _dasRepo.Language.GetAllListAsync(l => l.Status == (int)EnumCommon.Status.Active);
        }
        #endregion

        #region Create
        public async Task<ServiceResult> Create(Language model)
        {
            await _dasRepo.Language.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }

        public async Task<ServiceResult> CreateLanguage(VMLanguage vmLanguage)
        {
            try
            {
                List<Language> listExistLanguage;
                listExistLanguage = await _dasRepo.Language.GetAll().Where(x=>x.Status == 1).Where(x => x.Name == vmLanguage.Name).ToListAsync();
                if (listExistLanguage != null && listExistLanguage.Count() > 0)
                    return new ServiceResultError("Tên ngôn ngữ đã tồn tại");
                listExistLanguage = await _dasRepo.Language.GetAll().Where(x => x.Status == 1).Where(x => x.Code == vmLanguage.Code).ToListAsync();
                if (listExistLanguage != null && listExistLanguage.Count() > 0)
                    return new ServiceResultError("Mã ngôn ngữ đã tồn tại");
                Language language = _mapper.Map<Language>(vmLanguage);
                await _dasRepo.Language.InsertAsync(language);
                await _dasRepo.SaveAync();
                if (language.ID == 0)
                    return new ServiceResultError("Thêm mới ngôn ngữ không thành công");

                return new ServiceResultSuccess("Thêm mới ngôn ngữ thành công!");
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
        public async Task<ServiceResult> Update(Language model)
        {
            await _dasRepo.Language.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật Ngôn ngữ thành công");
        }
        public async Task<ServiceResult> UpdateLanguage(VMLanguage vmLanguage)
        {
            try
            {
                var language = await _dasRepo.Language.GetAsync(vmLanguage.ID);
                List<Language> listExistLanguage;
                listExistLanguage = await _dasRepo.Language.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(m => m.Name == vmLanguage.Name && m.Name != language.Name)
                    .ToListAsync();
                if (listExistLanguage != null && listExistLanguage.Count() > 0)
                    return new ServiceResultError("Tên Ngôn ngữ đã tồn tại!");
                listExistLanguage = await _dasRepo.Language.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(m => m.Code == vmLanguage.Code && m.Code != language.Code).ToListAsync();
                if (listExistLanguage != null && listExistLanguage.Count() > 0)
                    return new ServiceResultError("Mã Ngôn ngữ đã tồn tại!");

                _mapper.Map(vmLanguage, language);
                await _dasRepo.Language.UpdateAsync(language);
                await _dasRepo.SaveAync();
                if (language.ID == 0)
                    return new ServiceResultError("Cập nhật Ngôn ngữ không thành công!");

                return new ServiceResultSuccess("Cập nhật Ngôn ngữ thành công!");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
            
        }
        #endregion

        #region Delete
        public async Task<ServiceResult> DeleteLanguage(int id)
        {
            try
            {
                //1. Cập nhật Language = 0 ở ProfileTemplate
                //var profileTemplate = await _dasRepo.ProfileTemplate.GetAll().Where(m => m.Language == id).ToListAsync();
                //if (profileTemplate != null && profileTemplate.Count() > 0)
                //{
                //    //Không cần update ProfileTemplate
                //}
                //else
                //{
                //    foreach (var item in profileTemplate)
                //    {
                //        item.Language = 0;
                //    }
                //    await _dasRepo.ProfileTemplate.UpdateAsync(profileTemplate);
                //    await _dasRepo.SaveAync();
                //}
                //2. Xóa Language theo logic 
                var language = await _dasRepo.Language.GetAsync(id);
                if (language == null || language.Status == (int)EnumLanguage.Status.InActive)
                    return new ServiceResultError("Ngôn ngữ này hiện không tồn tại hoặc đã bị xóa");
                language.Status = (int)EnumLanguage.Status.InActive;
                await _dasRepo.Language.UpdateAsync(language);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa Ngôn ngữ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
            
        }
        public async Task<ServiceResult> DeleteMultiLanguage(IEnumerable<int> ids)
        {
            try
            {
                var languageDeletes = await _dasRepo.Language.GetAllListAsync(n => ids.Contains(n.ID));
                if (languageDeletes == null || languageDeletes.Count() == 0)
                    return new ServiceResultError("Ngôn ngữ đã chọn hiện không tồn tại hoặc đã bị xóa");
                //1. Cập nhật Language = 0 ở ProfileTemplate
                //var profileTemplate = await _dasRepo.ProfileTemplate.GetAll().Where(m => ids.Contains(m.Language)).ToListAsync();
                //if (profileTemplate != null && profileTemplate.Count() > 0)
                //{
                //    //Không cần update ProfileTemplate
                //}
                //else
                //{
                //    foreach (var item in profileTemplate)
                //    {
                //        item.Language = 0;
                //    }
                //    await _dasRepo.ProfileTemplate.UpdateAsync(profileTemplate);
                //    await _dasRepo.SaveAync();
                //}
                //foreach (var item in languageDeletes)
                //{
                //    item.Status = (int)EnumLanguage.Status.InActive;
                //}
                //2. Xóa Language
                await _dasRepo.Language.UpdateAsync(languageDeletes);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa ngôn ngữ thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }        
        }
        public async Task<ServiceResult> Delete(object id)
        {
            try
            {
                var language = await _dasRepo.Language.GetAsync(id);
                if (language == null || language.Status == (int)EnumLanguage.Status.InActive)
                    return new ServiceResultError("Ngôn ngữ này hiện không tồn tại hoặc đã bị xóa");
                language.Status = (int)EnumLanguage.Status.InActive;
                await _dasRepo.Language.UpdateAsync(language);
                await _dasRepo.SaveAync();
                if (language == null)
                    return new ServiceResultError("Ngôn ngữ không tồn tại");

                return new ServiceResultSuccess("Đã xóa Ngôn ngữ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
         
        }
        #endregion

        #region Functions
        public async Task<bool> IsCodeExist(string code)
        {
            return await _dasRepo.Language.AnyAsync(s => s.Code == code);
        }
        public async Task<bool> IsNameExist(string name)
        {
            return await _dasRepo.Language.AnyAsync(s => s.Name == name);
        }
        #endregion

    }
}
