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
using ESD.Utility.LogUtils;
using ESD.Application.Enums;
using ESD.Domain.Enums;

namespace ESD.Application.Services
{
    public class SercureLevelService : BaseMasterService, ISercureLevelServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        #endregion

        #region Ctor
        public SercureLevelService(IDasRepositoryWrapper dasRepository, IMapper mapper, ILoggerManager logger) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region Get
        public async Task<SercureLevel> Get(object id)
        {
            return await _dasRepo.SercureLevel.GetAsync(id);
        }
        public async Task<IEnumerable<SercureLevel>> Gets()
        {
            return await _dasRepo.SercureLevel.GetAllListAsync();

        }
        public async Task<VMSercureLevel> GetSercureLevel(int id)
        {
            var temp = from r in _dasRepo.SercureLevel.GetAll()
                       where r.ID == id
                       orderby r.ID descending
                       select new VMSercureLevel
                       { 
                            ID = r.ID,
                            IDChannel = r.IDChannel,
                            Name = r.Name,
                            Code = r.Code,
                            Description = r.Description,
                            CreatedBy = r.CreatedBy,
                            CreateDate = r.CreateDate,
                            UpdatedDate = r.UpdatedDate,
                            UpdatedBy = r.UpdatedBy

                       };
            var result = await temp.FirstOrDefaultAsync();
            return result;
        }
        public async Task<PaginatedList<VMSercureLevel>> SearchByConditionPagging(SercureLevelCondition condition)
        {
            var temp = from r in _dasRepo.SercureLevel.GetAll().Where(x => x.Status == (int)EnumSercureLevel.Status.Active)
                       where (string.IsNullOrEmpty(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Code.Contains(condition.Keyword))
                       orderby r.ID descending
                       select new VMSercureLevel
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
            PaginatedList<VMSercureLevel> model = new PaginatedList<VMSercureLevel>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }
        public async Task<IEnumerable<VMSercureLevel>> GetListByCondition(SercureLevelCondition condition)
        {
            var temp = from r in _dasRepo.SercureLevel.GetAll().Where(x => x.Status == (int)EnumSercureLevel.Status.Active)
                       where (string.IsNullOrEmpty(condition.Keyword) || r.Name.Contains(condition.Keyword) || r.Code.Contains(condition.Keyword))
                       orderby r.ID descending
                       select new VMSercureLevel
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description
                       };
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMSercureLevel>> GetsActive()
        {
            var temp = from r in _dasRepo.SercureLevel.GetAll().Where(x => x.Status == (int)EnumSercureLevel.Status.Active)
                       select new VMSercureLevel
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Code = r.Code,
                           Name = r.Name,
                           Status = r.Status,
                           Description = r.Description
                       };
            return await temp.ToListAsync();
        }
        #endregion

        #region Create
        public async Task<ServiceResult> Create(SercureLevel model)
        {
            await _dasRepo.SercureLevel.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }
        public async Task<ServiceResult> CreateSercureLevel(VMSercureLevel vmSercureLevel)
        {
            try
            {
                List<SercureLevel> listSercureLevel;
                listSercureLevel = await _dasRepo.SercureLevel.GetAll().Where(x => x.Status == (int)EnumSercureLevel.Status.Active).Where(m => m.Code == vmSercureLevel.Code).ToListAsync();
                if (listSercureLevel != null && listSercureLevel.Count() > 0)
                    return new ServiceResultError("Mã cấp độ bảo mật đã tồn tại");
             
                SercureLevel sercureLevel = _mapper.Map<SercureLevel>(vmSercureLevel);
                await _dasRepo.SercureLevel.InsertAsync(sercureLevel);
                await _dasRepo.SaveAync();
                if (sercureLevel.ID == 0)
                    return new ServiceResultError("Thêm mới cấp độ bảo mật không thành công");
                return new ServiceResultSuccess("Thêm mới cấp độ bảo mật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
            
        }
        #endregion

        #region Update
        public async Task<ServiceResult> Update(SercureLevel model)
        {
            await _dasRepo.SercureLevel.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật cấp đô bảo mật thành công");
        }
        public async Task<ServiceResult> UpdateSercureLevel(VMSercureLevel vmSercureLevel)
        {
            try
            {
                var sercureLevel = await _dasRepo.SercureLevel.GetAsync(vmSercureLevel.ID);
                List<SercureLevel> listExistSercureLevel;
                listExistSercureLevel = await _dasRepo.SercureLevel.GetAll()
                    .Where(x => x.Status == (int)EnumCommon.Status.Active)
                    .Where(m => m.Code == vmSercureLevel.Code && m.Code != sercureLevel.Code).ToListAsync();
                if (listExistSercureLevel != null && listExistSercureLevel.Count() > 0)
                    return new ServiceResultError("Mã cấp độ bảo mật đã tồn tại");

                _mapper.Map(vmSercureLevel, sercureLevel);
                await _dasRepo.SercureLevel.UpdateAsync(sercureLevel);
                await _dasRepo.SaveAync();
                if (sercureLevel.ID == 0)
                    return new ServiceResultError("Cập nhật cấp độ bảo mật không thành công");

                return new ServiceResultSuccess("Cập nhật cấp độ bảo mật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
            
        }
        #endregion

        #region Delete
        public Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }
        public async Task<ServiceResult> DeleteSercureLevel(int id)
        {
            try
            {
                var sercureLevel = await _dasRepo.SercureLevel.GetAsync(id);
                if (sercureLevel == null || sercureLevel.Status == (int)EnumSercureLevel.Status.InActive)
                    return new ServiceResultError("Cấp độ bảo mật này hiện không tồn tại hoặc đã bị xóa");
                sercureLevel.Status = (int)EnumSercureLevel.Status.InActive;
                await _dasRepo.SercureLevel.UpdateAsync(sercureLevel);
                await _dasRepo.SaveAync();

                return new ServiceResultSuccess("Xóa cấp độ bảo mật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }            
        }
        public async Task<ServiceResult> DeleteMultiSercureLevel(IEnumerable<int> ids)
        {
            try
            {
                var sercureLevels = await _dasRepo.SercureLevel.GetAllListAsync(n => ids.Contains(n.ID));
                if (sercureLevels == null || sercureLevels.Count() == 0)
                    return new ServiceResultError("Cấp độ bảo mật không tồn tại hoặc đã bị xóa");
                foreach (var item in sercureLevels)
                {
                    item.Status = (int)EnumSercureLevel.Status.InActive;
                }
                await _dasRepo.SercureLevel.UpdateAsync(sercureLevels);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa cấp độ bảo mật thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }           
        }

        #endregion

        #region Function
        public async Task<bool> IsCodeExist(string code)
        {
            return await _dasRepo.SercureLevel.AnyAsync(s => s.Code == code);
        }
        public async Task<bool> IsNameExist(string name)
        {
            return await _dasRepo.SercureLevel.AnyAsync(s => s.Name == name);
        }

        public Task<ServiceResult> DeleteMultiSercureLevel(string idStr)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
