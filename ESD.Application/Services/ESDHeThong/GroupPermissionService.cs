using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.LogUtils;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class GroupPermissionService : BaseMasterService, IGroupPermissionService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        #endregion Properties

        #region Ctor

        public GroupPermissionService(IDasRepositoryWrapper dasRepository, IMapper mapper, ILoggerManager logger, IUserPrincipalService userPrincipalService) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
        }

        #endregion Ctor

        #region Get

        public async Task<GroupPermission> Get(object id)
        {
            return await _dasRepo.GroupPermission.GetAsync(id);
        }

        public async Task<IEnumerable<GroupPermission>> Gets()
        {
            return await _dasRepo.GroupPermission.GetAllListAsync(n => n.Status == (int)EnumGroupPermission.Status.Active);
        }

        public async Task<PaginatedList<VMGroupPermision>> SearchByConditionPagging(PermissionGroupCondition permissionGroup)
        {
            var temp = from gp in _dasRepo.GroupPermission.GetAll()
                       where (permissionGroup.Keyword.IsEmpty() || gp.Name.Contains(permissionGroup.Keyword)) 
                       //&& gp.Status == (int)EnumGroupPermission.Status.Active
                       select new VMGroupPermision
                       {
                           ID = gp.ID,
                           IDChannel = gp.IDChannel,
                           Description = gp.Description,
                           Name = gp.Name,
                           Status = gp.Status,
                       };


            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)permissionGroup.PageSize);
            if (totalPage < permissionGroup.PageIndex)
            {
                permissionGroup.PageIndex = 1;
            }
            var result = await temp.Skip((permissionGroup.PageIndex - 1) * permissionGroup.PageSize).Take(permissionGroup.PageSize).ToListAsync();
            PaginatedList<VMGroupPermision> model = new PaginatedList<VMGroupPermision>(result.ToList(), (int)total, permissionGroup.PageIndex, permissionGroup.PageSize);
            return model;
        }

        public async Task<IEnumerable<VMGroupPermision>> GetGroupPermissionsInList()
        {
            var temp = from r in _dasRepo.GroupPermission.GetAll()
                           //join rgp in _dasRepo.RoleGroupPer.GetAll() on r.ID equals rgp.IDRole
                           //  join gp in _dasRepo.GroupPermission.GetAll() on rgp.IDGroupPermission equals gp.ID
                       select new VMGroupPermision
                       {
                           ID = r.ID,
                           IDChannel = r.IDChannel,
                           Description = r.Description,
                           Name = r.Name,
                           Status = r.Status,
                       };
            return await temp.ToListAsync();
        }

        public async Task<IEnumerable<VMGroupPermision>> GetListByCondition(PermissionGroupCondition permissionGroup)
        {
            var temp = from gp in _dasRepo.GroupPermission.GetAll()
                       where (permissionGroup.Keyword.IsEmpty() || gp.Name.Contains(permissionGroup.Keyword)) 
                       //&& gp.Status == (int)EnumGroupPermission.Status.Active
                       select new VMGroupPermision
                       {
                           ID = gp.ID,
                           IDChannel = gp.IDChannel,
                           Description = gp.Description,
                           Name = gp.Name,
                           Status = gp.Status,
                       };
            return await temp.ToListAsync();
        }

        #endregion Get

        #region Create

        public Task<ServiceResult> Create(GroupPermission user)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> Create(VMGroupPermision vmGroupPermision)
        {
            try
            {
                //1. Insert GroupPermission
                var groupPermission = _mapper.Map<GroupPermission>(vmGroupPermision);
                if (await _dasRepo.GroupPermission.IsNameExist(groupPermission.Name))
                    return new ServiceResultError("Nhóm quyền này đã tồn tại trong hệ thống");

                await _dasRepo.GroupPermission.InsertAsync(groupPermission);
                await _dasRepo.SaveAync();
                if (groupPermission.ID == 0)
                    return new ServiceResultError("Thêm mới nhóm quyền không thành công!");

                if (vmGroupPermision.Permissions.Count() > 0)
                {
                    Parallel.ForEach(vmGroupPermision.Permissions, per =>
                    {
                        per.IsCheck = per.IsCheck || per.IsChecked == 1; //Set IsCheck
                    });
                }

                //2. Insert PermissionGroupPer
                if (vmGroupPermision.Permissions != null && vmGroupPermision.Permissions.Count() > 0)
                {
                    //check permission is exist in DB => err
                    var idPermissions = vmGroupPermision.Permissions.Select(o => o.IDPermission);

                    var coutPermisionViewModel = _dasRepo.Permission.Count(o => idPermissions.Contains(o.ID));
                    var coutPermisionDatabase = _dasRepo.Permission.Count();

                    if (coutPermisionViewModel != coutPermisionDatabase)
                    {
                        return new ServiceResultError("Tồn tại quyền không hợp lệ trong dữ liệu đầu vào");
                    }

                    var permissionGroupPers = new List<PermissionGroupPer>();
                    var dtNow = DateTime.Now;
                    foreach (var item in vmGroupPermision.Permissions.Where(n => n.IsCheck))
                    {
                        permissionGroupPers.Add(new PermissionGroupPer
                        {
                            IDPermission = item.IDPermission,
                            IDGroupPermission = groupPermission.ID,
                            CreateDate = dtNow,
                            CreatedBy = _userPrincipalService.UserId,
                        });
                    }
                    await _dasRepo.PermissionGroupPer.UpdateAsync(permissionGroupPers);
                }

                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Thêm nhóm quyền thành công");
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

        public async Task<ServiceResult> Update(VMGroupPermision vmGroupPermision)
        {
            try
            {

                //1. Update GroupPermission
                var groupPermissionUpdate = await _dasRepo.GroupPermission.GetAsync(vmGroupPermision.ID);
                if (groupPermissionUpdate == null)
                    return new ServiceResultError("Nhóm quyền này hiện không tồn tại hoặc đã bị xóa");

                groupPermissionUpdate.Name = vmGroupPermision.Name;
                groupPermissionUpdate.Description = vmGroupPermision.Description;
                groupPermissionUpdate.Status = vmGroupPermision.Status;
                if (await _dasRepo.GroupPermission.IsNameExist(groupPermissionUpdate.Name, groupPermissionUpdate.ID))
                    return new ServiceResultError("Nhóm quyền này đã tồn tại trong hệ thống");
                await _dasRepo.GroupPermission.UpdateAsync(groupPermissionUpdate);

                if (vmGroupPermision.Permissions.Count() > 0)
                {
                    Parallel.ForEach(vmGroupPermision.Permissions, per =>
                    {
                        per.IsCheck = per.IsCheck || per.IsChecked == 1; //Set IsCheck
                    });
                }
                //2. Update PermissionGroupPer
                if (vmGroupPermision.Permissions != null && vmGroupPermision.Permissions.Count() > 0)
                {
                    List<PermissionGroupPer> lstPermissionGroupPer_Delete = new List<PermissionGroupPer>();
                    List<PermissionGroupPer> lstPermissionGroupPer_Insert = new List<PermissionGroupPer>();

                    foreach (var per in vmGroupPermision.Permissions)
                    {
                        //IDPermission gán vào check box, lúc check IDPermission >0
                        if (per.IDPermissionGroupPer.GetValueOrDefault(0) == 0 && per.IsCheck) //Insert
                        {
                            lstPermissionGroupPer_Insert.Add(new PermissionGroupPer
                            {
                                IDGroupPermission = vmGroupPermision.ID,
                                IDPermission = per.IDPermission
                            });
                        }
                        if (per.IDPermissionGroupPer.GetValueOrDefault(0) > 0 && !per.IsCheck)//Delete
                        {
                            var deleteItem = _dasRepo.PermissionGroupPer.Get(per.IDPermissionGroupPer);
                            if (deleteItem != null)
                            {
                                lstPermissionGroupPer_Delete.Add(deleteItem);
                            }
                        }
                    }
                    await _dasRepo.PermissionGroupPer.InsertAsync(lstPermissionGroupPer_Insert);
                    await _dasRepo.PermissionGroupPer.DeleteAsync(lstPermissionGroupPer_Delete); ;
                }

                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật nhóm quyền thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        public Task<ServiceResult> Update(GroupPermission user)
        {
            throw new NotImplementedException();
        }

        #endregion Update

        #region Delete

        public Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                //Logic delete
                var positionDelete = await _dasRepo.GroupPermission.GetAsync(id);
                if (positionDelete == null )
                    return new ServiceResultError("Nhóm quyền này hiện không tồn tại hoặc đã bị xóa");
                //positionDelete.Status = (int)EnumGroupPermission.Status.InActive;
                await _dasRepo.GroupPermission.DeleteAsync(positionDelete);
                await _dasRepo.TeamGroupPer.DeleteAsync(s => s.IDGroupPer == id);
                await _dasRepo.UserGroupPer.DeleteAsync(s => s.IDGroupPer == id);
                await _dasRepo.PermissionGroupPer.DeleteAsync(s => s.IDGroupPermission == id);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa nhóm quyền thành công");

                ////check bảng RoleGroupPer, PermissionGroupPer
                //await _dasRepo.RoleGroupPer.DeleteAsync(s => s.IDGroupPermission == id);
               
                //await _dasRepo.GroupPermission.DeleteAsync(s => s.ID == id);

                //await _dasRepo.SaveAync();
                //return new ServiceResultSuccess("Xóa nhóm quyền thành công");
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
                //Logic delete
                var positionDeletes = await _dasRepo.GroupPermission.GetAllListAsync(n => ids.Contains(n.ID));
                if (positionDeletes == null || positionDeletes.Count() == 0)
                    return new ServiceResultError("Nhóm quyền đã chọn hiện không tồn tại hoặc đã bị xóa");

                //foreach (var pos in positionDeletes)
                //{
                //    pos.Status = (int)EnumGroupPermission.Status.InActive;
                //}
                await _dasRepo.GroupPermission.DeleteAsync(positionDeletes);

                await _dasRepo.TeamGroupPer.DeleteAsync(s => ids.Contains(s.IDGroupPer));
                await _dasRepo.UserGroupPer.DeleteAsync(s => ids.Contains(s.IDGroupPer));
                await _dasRepo.PermissionGroupPer.DeleteAsync(s => ids.Contains(s.IDGroupPermission));

                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa nhóm quyền thành công");

                ////check bảng RoleGroupPer, PermissionGroupPer
                //await _dasRepo.RoleGroupPer.DeleteAsync(s => ids.Contains(s.IDGroupPermission));
                //await _dasRepo.PermissionGroupPer.DeleteAsync(s => ids.Contains(s.IDGroupPermission));
                //await _dasRepo.GroupPermission.DeleteAsync(s => ids.Contains(s.ID));

                //await _dasRepo.SaveAync();
                //return new ServiceResultSuccess("Xóa nhóm quyền thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        #endregion Delete

        #region Common
        public async Task<IEnumerable<int>> UpdateGroupPersByTeams(IEnumerable<int> groupPerIds, IEnumerable<int> groupIds, string type, int idRemoved)
        {
            var listGroupPerIds = new List<int>();
            if (!IsExisted(groupPerIds))
                listGroupPerIds = new List<int>();
            else
                listGroupPerIds = groupPerIds.ToList();

            if (type == "selected")
            {
                if (!IsExisted(groupIds))
                    return null;

                var teamGroupPers = await _dasRepo.TeamGroupPer.GetAllListAsync(g => groupIds.Contains(g.IDTeam) && g.Status == (int)EnumCommon.Status.Active);
                if (!IsExisted(teamGroupPers))
                    return null;                

                foreach (var item in teamGroupPers)
                {
                    if (item == null || item.IDGroupPer == 0)
                        continue;
                    if (listGroupPerIds.Contains(item.IDGroupPer))
                        continue;
                    listGroupPerIds.Add(item.IDGroupPer);
                }
                return listGroupPerIds;
            }
            else if(type == "unselected")
            {
                if (!IsExisted(groupPerIds))
                    return null;

                //get teamGroupPerRemoveds by idremoved
                var teamGroupPerRemoveds = await _dasRepo.TeamGroupPer.GetAllListAsync(g => g.IDTeam == idRemoved && g.Status == (int)EnumCommon.Status.Active);
                if (!IsExisted(teamGroupPerRemoveds))
                    return groupPerIds;

                if (!IsExisted(groupIds))
                {
                    //remove item in teamGroupPerRemoveds
                    foreach(var item in teamGroupPerRemoveds.Select(t => t.IDGroupPer))
                    {
                        if (listGroupPerIds.Contains(item))
                            listGroupPerIds.Remove(item);
                    }
                    return listGroupPerIds;
                }
                else
                {
                    //get restOfTeamGroupPers by groupIds
                    var restOfTeamGroupPers = await _dasRepo.TeamGroupPer.GetAllListAsync(g => groupIds.Contains(g.IDTeam) && g.Status == (int)EnumCommon.Status.Active);
                    if (!IsExisted(restOfTeamGroupPers))
                    {
                        //remove item in teamGroupPerRemoveds
                        foreach (var item in teamGroupPerRemoveds.Select(t => t.IDGroupPer))
                        {
                            if (listGroupPerIds.Contains(item))
                                listGroupPerIds.Remove(item);
                        }
                        return listGroupPerIds;
                    }
                    else
                    {
                        //remove item in teamGroupPerRemoveds but not in restOfTeamGroupPers
                        foreach (var item in teamGroupPerRemoveds.Select(t => t.IDGroupPer))
                        {
                            if (listGroupPerIds.Contains(item) && !restOfTeamGroupPers.Select(t => t.IDGroupPer).Contains(item))
                                listGroupPerIds.Remove(item);
                        }
                    }
                    return listGroupPerIds;
                }
                
            }

            return groupPerIds;
        }
        #endregion Common

        #region Private method
        private bool IsExisted(GroupPermission group)
        {
            if (group == null || group.ID == 0 || group.Status != (int)EnumCommon.Status.Active)
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