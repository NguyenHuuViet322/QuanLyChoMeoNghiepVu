using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Utility;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class PositionService : BaseMasterService, IPositionServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        #endregion

        #region Ctor
        public PositionService(IDasRepositoryWrapper dasRepository, IMapper mapper, ILoggerManager logger) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region Gets  

        public async Task<IEnumerable<Position>> Gets()
        {
            return await _dasRepo.Position.GetAllListAsync();
        }
        public async Task<IEnumerable<Position>> GetsActive()
        {
            return await _dasRepo.Position.GetAllListAsync(n => n.Status == (int)EnumPosition.Status.Active);
        }

        public async Task<Position> Get(object id)
        {
            return await _dasRepo.Position.GetAsync(id);
        }

        public async Task<PaginatedList<VMPosition>> SearchByConditionPagging(PositionCondition positionCondition) 
        {
            var temp = from pos in _dasRepo.Position.GetAll()
                       where (positionCondition.Keyword.IsEmpty() || pos.Name.Contains(positionCondition.Keyword.Trim())) && pos.Status == (int)EnumPosition.Status.Active
                       select new VMPosition
                       {
                           ID = pos.ID,
                           Parent = pos.Parent,
                           ParentPath = pos.ParentPath,
                           IDChannel = pos.IDChannel,
                           Description = pos.Description,
                           Name = pos.Name,
                           Code = pos.Code,
                           Status = pos.Status,
                       };
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)positionCondition.PageSize);
            if (totalPage < positionCondition.PageIndex)
            {
                positionCondition.PageIndex = 1;
            }
            var result = await temp.ToListAsync();
            //Render tree
            var treeModels = Utils.RenderTree(result.Select(n => new TreeModel<VMPosition>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.Parent,
                ParentPath = n.ParentPath,
                Item = n
            }).ToList(), null);
            return new PaginatedList<VMPosition>(treeModels.Select(n => n.Item).ToList(), (int)total, positionCondition.PageIndex, positionCondition.PageSize);
        }
        public async Task<List<SelectListItemTree>> GetPostionByTree(VMPosition vMPosition)
        {
            var prPath = $"{vMPosition.ParentPath}|{vMPosition.ID}";
            var result = await _dasRepo.Position.GetAllListAsync(n => n.Status == (int)EnumPosition.Status.Active
            && (vMPosition.ID == 0 || (vMPosition.ID > 0 && n.ID != vMPosition.ID && n.Parent != vMPosition.ID && n.ParentPath.IndexOf(prPath) == -1))); //ko lấy chính nó và cấp con
            //Render tree
            var treeModels = Utils.RenderTree(result.Select(n => new TreeModel<VMPosition>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.Parent,
                ParentPath = n.ParentPath,
            }).ToList(), null, string.Empty);

            return treeModels.Select(n => new SelectListItemTree
            {
                Value = n.ID.ToString(),
                Text = n.Name,
                Selected = n.ID == vMPosition.Parent,
                Level = n.Level
            }).ToList();

        }
        public async Task<IEnumerable<VMPosition>> GetListByCondition(PositionCondition positionCondition, bool getParents=false)
        {

            var temp = from pos in _dasRepo.Position.GetAll()
                       where (positionCondition.Keyword.IsEmpty() || pos.Name.Contains(positionCondition.Keyword.Trim())) && pos.Status == (int)EnumPosition.Status.Active
                       select new VMPosition
                       {
                           ID = pos.ID,
                           Parent = pos.Parent,
                           ParentPath = pos.ParentPath,
                           IDChannel = pos.IDChannel,
                           Description = pos.Description,
                           Name = pos.Name,
                           Code = pos.Code,
                           Status = pos.Status,
                       };


            var result = await temp.ToListAsync();
            var treeModels = Utils.RenderTree(result.Select(n => new TreeModel<VMPosition>
            {
                ID = n.ID,
                Name = n.Name,
                Parent = n.Parent,
                ParentPath = n.ParentPath,
                Item = n
            }).ToList(), null);

            
            var rs = treeModels.Select(n => n.Item).ToList();

            if (getParents)
            {
                IEnumerable<Position> parents = null;
                var parentIds = result.Select(n => n.Parent).Distinct();
                if (parentIds.IsNotEmpty())
                    parents = await _dasRepo.Position.GetAllListAsync(n => parentIds.Contains(n.ID));

                if (parents.IsNotEmpty())
                {
                    foreach (var item in rs)
                    {
                        item.ParentName = parents.FirstOrNewObj(n => n.ID == item.Parent).Name;
                    }
                }
            }
            return rs;
        }
        #endregion

        #region Create
        public async Task<ServiceResult> Create(VMPosition vmPosition)
        {
            try
            {
                var position = _mapper.Map<Position>(vmPosition);
                BindUpdate(position, vmPosition);
                position.Status = (int)EnumPosition.Status.Active;
                if (await _dasRepo.Position.IsCodeExist(position.Code))
                {
                    return new ServiceResultError("Mã chức vụ đã tồn tại");
                }
                await _dasRepo.Position.InsertAsync(position);
                await _dasRepo.SaveAync();

                return new ServiceResultSuccess("Thêm chức vụ thành công");
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

        public async Task<ServiceResult> Update(VMPosition vmPosition)
        {
            try
            {
                var positionUpdate = await _dasRepo.Position.GetAsync(vmPosition.ID);
                if (positionUpdate == null || positionUpdate.Status == (int)EnumPosition.Status.InActive)
                    return new ServiceResultError("Chức vụ này hiện không tồn tại hoặc đã bị xóa");
                var oldParent = positionUpdate.ParentPath;
                BindUpdate(positionUpdate, vmPosition);
                if (await _dasRepo.Position.IsCodeExist(positionUpdate.Code, positionUpdate.ID))
                {
                    return new ServiceResultError("Mã chức vụ đã tồn tại");
                }
                await BindChildParentPath(positionUpdate, oldParent, $"{positionUpdate.ParentPath}|{positionUpdate.ID}");
                await _dasRepo.Position.UpdateAsync(positionUpdate);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật chức vụ thành công");
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
                var positionDelete = await _dasRepo.Position.GetAsync(id);
                if (positionDelete == null || positionDelete.Status == (int)EnumPosition.Status.InActive)
                    return new ServiceResultError("Chức vụ này hiện không tồn tại hoặc đã bị xóa");

                var isUsed = await _dasRepo.User.AnyAsync(n => n.IDPosition == id);
                if (isUsed)
                    return new ServiceResultError("Chức vụ này hiện đang được sử dụng, không được phép xoá");

                positionDelete.Status = (int)EnumPosition.Status.InActive;
                await _dasRepo.Position.UpdateAsync(positionDelete);
                await _dasRepo.SaveAync();

                await BindChildParentPath(positionDelete, positionDelete.ParentPath, "0");

                return new ServiceResultSuccess("Xóa chức vụ thành công");
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
                var positionDeletes = await _dasRepo.Position.GetAllListAsync(n => ids.Contains(n.ID));
                if (positionDeletes == null || positionDeletes.Count() == 0)
                    return new ServiceResultError("Chức vụ đã chọn hiện không tồn tại hoặc đã bị xóa");

                var positionUsed = await _dasRepo.User.GetAllListAsync(n => ids.Contains(n.IDPosition.GetValueOrDefault(0)));
                if (positionUsed.IsNotEmpty())
                {
                    var usedIds = positionUsed.Select(n => n.IDPosition).Distinct().ToArray();
                    var deletedNames = positionDeletes.Where(m => usedIds.Contains(m.ID)).Select(n => n.Name);
                    return new ServiceResultError("Chức vụ " + string.Join(", ", deletedNames) + " hiện đang được sử dụng, không được phép xoá");
                }

                foreach (var pos in positionDeletes)
                {
                    pos.Status = (int)EnumPosition.Status.InActive;
                    await BindChildParentPath(pos, pos.ParentPath, "0");
                }
                await _dasRepo.Position.UpdateAsync(positionDeletes);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa chức vụ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion

        #region Funtions

        private void BindUpdate(Position positionUpdate, VMPosition vmPosition)
        {
            positionUpdate.Parent = vmPosition.Parent;
            positionUpdate.ParentPath = "0";
            if (vmPosition.Parent > 0)
            {
                var pr = _dasRepo.Position.Get(vmPosition.Parent);
                if (pr != null)
                    positionUpdate.ParentPath = pr.ParentPath + "|" + pr.ID;

            }
            positionUpdate.Code = vmPosition.Code?.Trim();
            positionUpdate.Name = vmPosition.Name?.Trim();
            positionUpdate.Description = vmPosition.Description?.Trim();
        }
        private async Task<bool> BindChildParentPath(Position positionUpdate, string oldParent, string newParent)
        {
            var parentPath = $"{oldParent}|{positionUpdate.ID}";
            var childs = await _dasRepo.Position.GetAllListAsync(n => n.Parent == positionUpdate.ID || n.ParentPath.IndexOf(parentPath) == 0);
            if (childs.IsNotEmpty())
            {
                foreach (var child in childs)
                {
                    child.ParentPath = child.ParentPath.Replace(parentPath, newParent);
                }
                if (newParent == "0")
                {
                    foreach (var child in childs.Where(n => n.Parent == positionUpdate.ID))
                    {
                        child.Parent = 0;
                    }
                }
                await _dasRepo.Position.UpdateAsync(childs);
                await _dasRepo.SaveAync();
            }

            return true;
        }
        #endregion
    }
}
