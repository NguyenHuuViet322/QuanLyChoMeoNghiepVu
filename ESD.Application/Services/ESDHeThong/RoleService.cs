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
using ESD.Domain.Enums;

namespace ESD.Application.Services
{
    public class RoleService : BaseMasterService, IRoleServices
    {
        private readonly IMapper _mapper;

        public RoleService(IDasRepositoryWrapper dasRepository, IMapper mapper) : base(dasRepository)
        {
            _mapper = mapper;
        }

        #region BaseRepo
        public async Task<IEnumerable<Role>> Gets()
        {
            return await _dasRepo.Role.GetAllListAsync();
        }

        public async Task<Role> Get(object id)
        {
            return await _dasRepo.Role.GetAsync(id);
        }

        public async Task<ServiceResult> Create(Role model)
        {
            await _dasRepo.Role.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }

        public async Task<ServiceResult> Update(Role model)
        {
            await _dasRepo.Role.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add category suceess!");
        }
        public async Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }
        #endregion BaseRepo

        #region Create & Search
        public async Task<ServiceResult> CreateRole(VMCreateRole vmRole)
        {
            //check exist unique field
            List<Role> listExistRole;
            listExistRole = await _dasRepo.Role.GetAll().Where(m => m.Name == vmRole.Name && m.Status == (int)EnumRole.Status.Active).ToListAsync();
            if (IsExisted(listExistRole))
                return new ServiceResultError("Tên vai trò đã tồn tại!");

            //update data
            UpdateData(vmRole);

            Role role = _mapper.Map<Role>(vmRole);
            await _dasRepo.Role.InsertAsync(role);
            await _dasRepo.SaveAync();
            if (role.ID == 0)
                return new ServiceResultError("Thêm mới vai trò không thành công!");
            if (!IsExisted(vmRole.IDGroupPermissionStrs))
                return new ServiceResultSuccess("Thêm mới vai trò thành công!");
            RoleGroupPer roleGroup;
            List<RoleGroupPer> listRoleGroupPer = new List<RoleGroupPer>();
            foreach (var item in vmRole.IDGroupPermissionStrs)
            {
                roleGroup = new RoleGroupPer
                {
                    IDRole = role.ID,
                    IDGroupPermission = int.Parse(item),
                    CreatedBy = role.CreatedBy,
                    CreateDate = DateTime.Now
                };
                listRoleGroupPer.Add(roleGroup);
            }
            await _dasRepo.RoleGroupPer.InsertAsync(listRoleGroupPer);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Thêm mới vai trò thành công!");
        }

        public async Task<PaginatedList<VMRole>> SearchByConditionPagging(RoleCondition condition)
        {
            var temp = _dasRepo.Role.GetAll().Where(m => (string.IsNullOrEmpty(condition.Keyword) || m.Name.Contains(condition.Keyword)) && m.Status == (int)EnumRole.Status.Active);
            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var roles = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var vmRoles = _mapper.Map<List<VMRole>>(roles);
            var result = new PaginatedList<VMRole>(vmRoles.ToList(), (int)total, condition.PageIndex, condition.PageSize);

            //get list joined (roleGroupPers join groupPermissions) by list roleid
            var joined = await (from r in _dasRepo.RoleGroupPer.GetAll()
                                join g in _dasRepo.GroupPermission.GetAll() on r.IDGroupPermission equals g.ID
                                where roles.Select(x => x.ID).Contains(r.IDRole) && g.Status == (int)EnumGroupPermission.Status.Active
                                select new VMRole
                                {
                                    ID = r.IDRole,
                                    IDGroupPermission = r.IDGroupPermission,
                                    GroupPermissionName = g.Name
                                }).ToListAsync();
            if (!IsExisted(joined))
                return result;

            //check exist GroupPermissionName in joined
            var existeds = joined.Where(m => !string.IsNullOrEmpty(m.GroupPermissionName));
            if (!IsExisted(joined))
                return result;

            //group by joined and update GroupPermissionName
            joined = joined.GroupBy(m => new { m.ID }).Select(x => new VMRole
            {
                ID = x.Key.ID,
                ListGroupPermission = x.ToList()
            }).ToList();

            foreach (var item in joined)
            {
                item.GroupPermissionName = string.Join(",", item.ListGroupPermission.Select(m => m.GroupPermissionName));
            }

            //merge vmRoles and joined
            vmRoles = vmRoles.GroupJoin(joined, left => left.ID, right => right.ID, (x, y) => new { Left = x, Rights = y }).SelectMany(
                x => x.Rights.DefaultIfEmpty(), (x, y) => new VMRole
                {
                    ID = x.Left.ID,
                    IDChannel = x.Left.IDChannel,
                    Name = x.Left.Name,
                    Status = x.Left.Status,
                    GroupPermissionName = y == null ? x.Left.GroupPermissionName : y.GroupPermissionName
                }
            ).ToList();

            return new PaginatedList<VMRole>(vmRoles.ToList(), (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<IEnumerable<Role>> GetActive()
        {
            return await _dasRepo.Role.GetAllListAsync(r => r.Status == (int)EnumRole.Status.Active);
        }
        public async Task<IEnumerable<VMRole>> GetListByCondition(RoleCondition condition)
        {
            var temp = _dasRepo.Role.GetAll().Where(m => (string.IsNullOrEmpty(condition.Keyword) || m.Name.Contains(condition.Keyword)) && m.Status == (int)EnumRole.Status.Active);
            var roles = await temp.ToListAsync();
            var vmRoles = _mapper.Map<List<VMRole>>(roles);
            var joined = await (from r in _dasRepo.RoleGroupPer.GetAll()
                                join g in _dasRepo.GroupPermission.GetAll() on r.IDGroupPermission equals g.ID
                                where roles.Select(x => x.ID).Contains(r.IDRole) && g.Status == (int)EnumGroupPermission.Status.Active
                                select new VMRole
                                {
                                    ID = r.IDRole,
                                    IDGroupPermission = r.IDGroupPermission,
                                    GroupPermissionName = g.Name
                                }).ToListAsync();
            if (!IsExisted(joined))
                return vmRoles;
            var existeds = joined.Where(m => !string.IsNullOrEmpty(m.GroupPermissionName));
            if (!IsExisted(joined))
                return vmRoles;
            //group by joined and update GroupPermissionName
            joined = joined.GroupBy(m => new { m.ID }).Select(x => new VMRole
            {
                ID = x.Key.ID,
                ListGroupPermission = x.ToList()
            }).ToList();

            foreach (var item in joined)
            {
                item.GroupPermissionName = string.Join(",", item.ListGroupPermission.Select(m => m.GroupPermissionName));
            }

            //merge vmRoles and joined
            vmRoles = vmRoles.GroupJoin(joined, left => left.ID, right => right.ID, (x, y) => new { Left = x, Rights = y }).SelectMany(
                x => x.Rights.DefaultIfEmpty(), (x, y) => new VMRole
                {
                    ID = x.Left.ID,
                    IDChannel = x.Left.IDChannel,
                    Name = x.Left.Name,
                    Status = x.Left.Status,
                    GroupPermissionName = y == null ? x.Left.GroupPermissionName : y.GroupPermissionName
                }
            ).ToList();
            return vmRoles.ToList();

        }
        #endregion Create & Search

        #region Detail
        public async Task<VMRole> GetRoleDetail(int id)
        {
            //get role by id
            var role = await _dasRepo.Role.GetAsync(id);
            if (!IsExisted(role))
                return null;
            var result = _mapper.Map<VMRole>(role);

            //get list RoleGroupPer
            var roleGroupPers = await _dasRepo.RoleGroupPer.GetAll().Where(x => x.IDRole == id).ToListAsync();
            if (!IsExisted(roleGroupPers))
                return result;

            //get list groupPermissions by list idGroupPermissions in roleGroupPers
            var groupPermissions = await _dasRepo.GroupPermission.GetAll().Where(x => roleGroupPers.Select(m => m.IDGroupPermission).Contains(x.ID)).ToListAsync();
            if (!IsExisted(groupPermissions))
                return result;

            //check null GroupPermissionName in groupPermissions
            var existedUserRole = groupPermissions.Where(m => !string.IsNullOrEmpty(m.Name));
            if (!IsExisted(existedUserRole))
                return result;

            //group by list GroupPermissionName from groupPermissions into result
            result.GroupPermissionName = string.Join(",", groupPermissions.Select(m => m.Name));

            return result;
        }

        public async Task<VMCreateRole> GetRole(int id)
        {
            //get role by id
            var role = await _dasRepo.Role.GetAsync(id);
            if (!IsExisted(role))
                return null;
            var result = _mapper.Map<VMCreateRole>(role);

            //get list RoleGroupPer
            IEnumerable<RoleGroupPer> roleGroupPers = await _dasRepo.RoleGroupPer.GetAll().Where(x => x.IDRole == id).ToListAsync();
            if (!IsExisted(roleGroupPers))
                return result;

            //select list IDGroupPermission from roleGroupPers into result
            result.IDGroupPermissionStrs = roleGroupPers.Select(m => m.IDGroupPermission.ToString()).ToList();

            return result;
        }
        #endregion Detail

        #region Update    
        public async Task<ServiceResult> UpdateRole(VMCreateRole vmRole)
        {
            var role = await _dasRepo.Role.GetAsync(vmRole.ID);

            if (!IsExisted(role))
                return new ServiceResultError("Không tồn tại vai trò này!");
            //check exist unique field
            List<Role> listExistRole;
            listExistRole = await _dasRepo.Role.GetAll().Where(m => m.Name == vmRole.Name && m.Name != role.Name).ToListAsync();
            if (IsExisted(listExistRole))
                return new ServiceResultError("Tên vai trò đã tồn tại!");

            //update data
            UpdateData(vmRole);

            _mapper.Map(vmRole, role);
            await _dasRepo.Role.UpdateAsync(role);
            await _dasRepo.SaveAync();
            if (role.ID == 0)
                return new ServiceResultError("Sửa vai trò không thành công!");

            //get roleGroupPers by roleid
            var roleGroupPers = await _dasRepo.RoleGroupPer.GetAll().Where(m => m.IDRole == role.ID).ToListAsync();
            if (IsExisted(roleGroupPers))
            {
                //delete old roleGroupPers
                await _dasRepo.RoleGroupPer.DeleteAsync(roleGroupPers);
                await _dasRepo.SaveAync();
            }

            if (!IsExisted(vmRole.IDGroupPermissionStrs))
                return new ServiceResultSuccess("Sửa vai trò thành công!");

            //create new roleGroupPers
            RoleGroupPer roleGroupPer;
            roleGroupPers = new List<RoleGroupPer>();
            foreach (var item in vmRole.IDGroupPermissionStrs)
            {
                roleGroupPer = new RoleGroupPer
                {
                    IDRole = vmRole.ID,
                    IDGroupPermission = int.Parse(item),
                    CreatedBy = role.CreatedBy,
                    CreateDate = DateTime.Now
                };
                roleGroupPers.Add(roleGroupPer);
            }

            await _dasRepo.RoleGroupPer.InsertAsync(roleGroupPers);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Sửa vai trò thành công!");
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteRole(int id)
        {
            var role = await _dasRepo.Role.GetAsync(id);
            role.Status = (int)EnumRole.Status.InActive;
            await _dasRepo.Role.UpdateAsync(role);
            //await _dasRepo.Role.DeleteAsync(s => s.ID == id);
            //await _dasRepo.RoleGroupPer.DeleteAsync(s => s.IDRole == id);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa vai trò thành công!");
        }

        public async Task<ServiceResult> DeleteMultiRole(IEnumerable<int> ids)
        {
            var roles = await _dasRepo.Role.GetAll().Where(m => ids.Contains(m.ID)).ToListAsync();
            roles = roles.Select(m => { m.Status = (int)EnumRole.Status.InActive; return m; }).ToList();
            await _dasRepo.Role.UpdateAsync(roles);
            //await _dasRepo.Role.DeleteAsync(s => ids.Contains(s.ID));
            //await _dasRepo.RoleGroupPer.DeleteAsync(s => ids.Contains(s.IDRole));
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa vai trò thành công!");
        }
        #endregion

        #region Common
        //public async Task<IEnumerable<int>> UpdateRolesByTeams(IEnumerable<int> roleIds, IEnumerable<int> groupIds, string type)
        //{
        //    var teamRole = await _dasRepo.TeamRole.GetAllListAsync(g => groupIds.Contains(g.IDTeam));
        //    if (!IsExisted(teamRole))
        //        return roleIds;
        //    if (type == "selected")
        //    {
        //        foreach (var item in teamRole)
        //        {
        //            if (item == null || item.IDRole == 0)
        //                continue;
        //            if (roleIds.Contains(item.IDRole))
        //                continue;
        //            roleIds = roleIds.Concat(new[] { item.IDRole });
        //        }
        //    }
        //    else if (type == "unselected")
        //    {
        //        roleIds = roleIds.Where(g => !teamRole.Select(m => m.IDRole).Contains(g));
        //    }

        //    return roleIds;
        //}
        #endregion Common

        #region Private method
        public async Task<bool> IsNameExist(string name)
        {
            return await _dasRepo.Role.AnyAsync(s => s.Name == name);
        }

        private void UpdateData(VMCreateRole vmRole)
        {
            if (vmRole.IDGroupPermissionStrs != null && !string.IsNullOrEmpty(vmRole.IDGroupPermissionStrs.First()) && vmRole.IDGroupPermissionStrs.First().Contains("["))
                vmRole.IDGroupPermissionStrs = JsonConvert.DeserializeObject<List<string>>(vmRole.IDGroupPermissionStrs.First());
            vmRole.Status = (int)EnumRole.Status.Active;

        }

        private bool IsExisted(Role role)
        {
            if (role == null || role.ID == 0 || role.Status != (int)EnumRole.Status.Active)
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
