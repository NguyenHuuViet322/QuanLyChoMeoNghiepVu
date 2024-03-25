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

namespace ESD.Application.Services
{
    public class AgencyService : BaseMasterService, IAgencyServices
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _iUserPrincipalService;

        public AgencyService(IDasRepositoryWrapper dasRepository
            , IMapper mapper, ILoggerManager logger
            , IUserPrincipalService iUserPrincipalService) : base(dasRepository) 
        {
            _mapper = mapper;
            _logger = logger;
            _iUserPrincipalService = iUserPrincipalService;
        }

        public async Task<IEnumerable<Agency>> Gets()
        {
            return await _dasRepo.Agency.GetAllListAsync();
        }

        public async Task<Agency> Get(object id)
        {
            return await _dasRepo.Agency.GetAsync(id);
        }

        public async Task<ServiceResult> Create(Agency model)
        {
            await _dasRepo.Agency.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }
        public async Task<ServiceResult> Update(Agency model)
        {
            await _dasRepo.Agency.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add category suceess!");
        }

        public async Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }

        #region Create & Search
        public async Task<ServiceResult> CreateAgency(VMCreateAgency vmAgency)
        {
            //check exist unique field
            List<Agency> listExist;
            listExist = await _dasRepo.Agency.GetAll().Where(m => (m.Status == (int)EnumCommon.Status.Active) && m.Code == vmAgency.Code).ToListAsync();
            if (IsExisted(listExist))
                return new ServiceResultError("Mã đơn vị đã tồn tại!");

            //update data
            UpdateData(vmAgency);

            Agency Agency = _mapper.Map<Agency>(vmAgency);
            await _dasRepo.Agency.InsertAsync(Agency);
            await _dasRepo.SaveAync();
            if (Agency.ID == 0)
                return new ServiceResultError("Thêm mới đơn vị không thành công");
            return new ServiceResultSuccess("Thêm mới đơn vị thành công");
        }

        public async Task<PaginatedList<VMAgency>> SearchByConditionPagging(AgencyCondition condition)
        {
            var temp = from d in _dasRepo.Agency.GetAll()
                       where d.Status == (int)EnumCommon.Status.Active && (string.IsNullOrEmpty(condition.Keyword) || d.Name.Contains(condition.Keyword))
                       join a in _dasRepo.Organ.GetAll() on d.IDOrgan equals a.ID
                       where a.ID == _iUserPrincipalService.IDOrgan
                       join ag in _dasRepo.Agency.GetAll() on d.ParentId equals ag.ID into joined
                       from j in joined.DefaultIfEmpty()
                       select new VMAgency
                       {
                           ID = d.ID,
                           Name = d.Name,
                           Code = d.Code,
                           ParentName = j.Name,
                           OrganName = a.Name,
                           ParentPath = d.ParentPath,
                           ParentId = d.ParentId
                       };

            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var Agencys = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var vmAgency = _mapper.Map<List<VMAgency>>(Agencys);

            //Render tree
            var treeModels = Utils.RenderTree(vmAgency.Select(n => new TreeModel<VMAgency>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.ParentId,
                ParentPath = n.ParentPath,
                Item = n
            }).ToList(), null);

            return new PaginatedList<VMAgency>(treeModels.Select(n => n.Item).ToList(), (int)total, condition.PageIndex, condition.PageSize);
        }
        public async Task<IEnumerable<VMAgency>> GetListByCondition(AgencyCondition condition)
        {
            var temp = from d in _dasRepo.Agency.GetAll()
                       where d.Status == (int)EnumCommon.Status.Active && (string.IsNullOrEmpty(condition.Keyword) || d.Name.Contains(condition.Keyword))
                       join a in _dasRepo.Organ.GetAll() on d.IDOrgan equals a.ID
                       where a.ID == _iUserPrincipalService.IDOrgan
                       join ag in _dasRepo.Agency.GetAll() on d.ParentId equals ag.ID into joined
                       from j in joined.DefaultIfEmpty()
                       select new VMAgency
                       {
                           ID = d.ID,
                           Name = d.Name,
                           Code = d.Code,
                           ParentName = j.Name,
                           OrganName = a.Name,
                           ParentPath = d.ParentPath
                       };
            var agencies = await temp.ToListAsync();
            //Render tree
            var treeModels = Utils.RenderTree(agencies.Select(n => new TreeModel<VMAgency>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.ParentId,
                ParentPath = n.ParentPath,
                Item = n
            }).ToList(), null);

            return treeModels.Select(n => n.Item).ToList();
        }
        #endregion Create & Search

        #region Get
        public async Task<VMAgency> GetDetail(int id)
        {
            var temp = from x in _dasRepo.Agency.GetAll()
                       where x.Status == (int)EnumCommon.Status.Active && x.ID == id
                       join y in _dasRepo.Organ.GetAll() on x.IDOrgan equals y.ID
                       select new VMAgency
                       {
                           ID = x.ID,
                           Name = x.Name,
                           Code = x.Code,
                           Description = x.Description,
                           OrganName = y.Name,
                           ParentId = x.ParentId
                       };
            var Agency = await temp.FirstOrDefaultAsync();

            //get all list organ without this organ and child this organ
            var agencies = await GetParentAgency(id);
            if (!IsExisted(agencies))
                return Agency;
            Agency.Parents = agencies;

            return Agency;
        }

        public async Task<VMEditAgency> GetAgency(int id)
        {
            //get Agency by id
            var Agency = await _dasRepo.Agency.GetAsync(id);
            if (!IsExisted(Agency))
                return null;
            var vmAgency = _mapper.Map<VMEditAgency>(Agency);

            //get all list organ without this organ and child this organ
            var agencies = await GetParentAgency(id);
            if (!IsExisted(agencies))
                return vmAgency;
            vmAgency.Parents = agencies;

            return vmAgency;
        }

        public async Task<IEnumerable<Agency>> GetParentAgency(int id)
        {
            //get all list agency without this agency and child this agency
            var temp = from a in _dasRepo.Agency.GetAll()
                       where a.Status == (int)EnumCommon.Status.Active && a.ID != id && (id == 0 || !a.ParentPath.Contains(id.ToString()))
                       && a.IDOrgan == _iUserPrincipalService.IDOrgan
                       select new Agency
                       {
                           ID = a.ID,
                           Name = a.Name
                       };

            var agencies = await temp.ToListAsync();
            return agencies;
        }

        public async Task<IEnumerable<Agency>> GetActive()
        {
            return await _dasRepo.Agency.GetAllListAsync(m => m.Status == (int)EnumCommon.Status.Active && m.IDOrgan == _iUserPrincipalService.IDOrgan);
        }

        public async Task<IEnumerable<Agency>> GetAgencyByUser()
        {
            return await _dasRepo.Agency.GetAllListAsync(m => m.Status == (int)EnumCommon.Status.Active && m.IDOrgan == _iUserPrincipalService.IDOrgan);
        }

        public async Task<IEnumerable<HierachyAgency>> GetHierachyAgency(int id)
        {
            var temp = from a1 in _dasRepo.Agency.GetAll()
                       where a1.Status == (int)EnumCommon.Status.Active && a1.ParentId == id && a1.IDOrgan == _iUserPrincipalService.IDOrgan
                       select new HierachyAgency
                       {
                           ID = a1.ID,
                           Name = a1.Name
                       };
            var agencies = await temp.OrderBy(o => o.Name).ToListAsync();

            foreach (var item in agencies)
            {
                item.HasChild = false;
                var total = await _dasRepo.Agency.GetAll().Where(a => a.ParentId == item.ID && a.Status == (int)EnumCommon.Status.Active).LongCountAsync();
                if (total > 0)
                    item.HasChild = true;
            }

            return agencies;
        }
        #endregion Get

        #region Update    
        public async Task<ServiceResult> UpdateAgency(VMEditAgency vmAgency)
        {
            var agency = await _dasRepo.Agency.GetAsync(vmAgency.ID);
            if (!IsExisted(agency))
                return new ServiceResultError("Không tồn tại đơn vị này!");

            //check exist unique field
            List<Agency> listExist;
            listExist = await _dasRepo.Agency.GetAll().Where(m => (m.Status == (int)EnumCommon.Status.Active) && m.Code == vmAgency.Code  && m.Code != agency.Code).ToListAsync();
            if (IsExisted(listExist))
                return new ServiceResultError("Mã đơn vị đã tồn tại!");

            //update data
            UpdateData(vmAgency);

            await UpdateChildParentPath(vmAgency, string.Concat(vmAgency.ParentPath, "|", vmAgency.ID), string.Concat(agency.ParentPath, "|", vmAgency.ID));

            _mapper.Map(vmAgency, agency);
            await _dasRepo.Agency.UpdateAsync(agency);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Sửa đơn vị thành công!");
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteMultiAgency(IEnumerable<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    await DeleteAgency(id);
                }
            }
            catch (Exception)
            {
                return new ServiceResultError("Xóa đơn vị không thành công!");
            }

            return new ServiceResultSuccess("Xóa đơn vị thành công!");
        }


        public async Task<ServiceResult> DeleteAgency(int id)
        {
            var Agency = await _dasRepo.Agency.GetAsync(id);
            if (Agency.Status != (int)EnumCommon.Status.Active)
                return new ServiceResultError("đơn vị này không tồn tại!");
            //update status this Agency
            Agency.Status = (int)EnumCommon.Status.InActive;
            await _dasRepo.Agency.UpdateAsync(Agency);
            await _dasRepo.SaveAync();

            if (IsExisted(Agency))
                return new ServiceResultError("Sửa đơn vị thành công!");
            return new ServiceResultSuccess("Xóa đơn vị thành công!");
        }
        #endregion

        #region Commom
        public async Task<IEnumerable<Agency>> GetAgencys(IEnumerable<int> OrganIds, IEnumerable<int> ids)
        {
            return await _dasRepo.Agency.GetAll().Where(m => m.IDOrgan.HasValue && OrganIds.Contains(m.IDOrgan.Value) && m.Status == (int)EnumCommon.Status.Active).ToListAsync();
        }
        #endregion Common

        #region Private method
        private void UpdateData(VMCreateAgency vmAgency)
        {
            vmAgency.Status = (int)EnumCommon.Status.Active;
            vmAgency.ParentId = string.IsNullOrEmpty(vmAgency.ParentIdStr) ? 0 : int.Parse(vmAgency.ParentIdStr);
            vmAgency.ParentPath = "0";
            if (vmAgency.ParentId > 0)
            {
                var parent = _dasRepo.Agency.Get(vmAgency.ParentId);
                if (IsExisted(parent))
                    vmAgency.ParentPath = parent.ParentPath + "|" + parent.ID;
            }
            else
                vmAgency.ParentId = 0;
        }

        private void UpdateData(VMEditAgency vmAgency)
        {
            vmAgency.Status = (int)EnumOrgan.Status.Active;
            vmAgency.ParentId = string.IsNullOrEmpty(vmAgency.ParentIdStr) ? 0 : int.Parse(vmAgency.ParentIdStr);
            vmAgency.ParentPath = "0";
            if (vmAgency.ParentId > 0)
            {
                var parent = _dasRepo.Agency.Get(vmAgency.ParentId);
                if (IsExisted(parent))
                    vmAgency.ParentPath = parent.ParentPath + "|" + parent.ID;
            }
            else
                vmAgency.ParentId = 0;
        }

        private async Task<bool> UpdateChildParentPath(VMEditAgency vmAgency, string newParent, string oldParent)
        {
            var childs = await _dasRepo.Agency.GetAllListAsync(n => n.ParentId == vmAgency.ID || n.ParentPath.IndexOf(oldParent) == 0);
            if (childs.IsNotEmpty())
            {
                foreach (var child in childs)
                {
                    child.ParentPath = child.ParentPath.Replace(oldParent, newParent);
                }
                if (newParent == "0")
                {
                    foreach (var child in childs.Where(n => n.ParentId == vmAgency.ID))
                    {
                        child.ParentId = 0;
                    }
                }
                await _dasRepo.Agency.UpdateAsync(childs);
                await _dasRepo.SaveAync();
            }
            return true;
        }

        private bool IsExisted(Agency Agency)
        {
            if (Agency == null || Agency.ID == 0 || Agency.Status != (int)EnumCommon.Status.Active)
                return false;
            return true;
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }
        #endregion Private method
    }
}
