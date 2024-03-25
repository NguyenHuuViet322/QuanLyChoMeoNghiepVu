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
using ESD.Application.Enums;

namespace ESD.Application.Services
{
    public class TeamService : BaseMasterService, ITeamService
    {
        private readonly IMapper _mapper;
        private readonly IAuthorizeService _authorizeService;

        public TeamService(IDasRepositoryWrapper dasRepository
            , IMapper mapper, IAuthorizeService authorizeService) : base(dasRepository)
        {
            _mapper = mapper;
            _authorizeService = authorizeService;
        }

        #region BaseRepo
        public async Task<ServiceResult> Create(Team group)
        {
            await _dasRepo.Team.InsertAsync(group);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }

        public async Task<ServiceResult> Update(Team group)
        {
            await _dasRepo.Team.UpdateAsync(group);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add User suceess!");
        }

        public async Task<Team> Get(object id)
        {
            return await _dasRepo.Team.GetAsync(id);
        }

        public async Task<IEnumerable<Team>> Gets()
        {
            return await _dasRepo.Team.GetAllListAsync();
        }

        public async Task<ServiceResult> Delete(object id)
        {
            var group = await _dasRepo.Team.GetAsync(id);
            await _dasRepo.Team.DeleteAsync(group);
            await _dasRepo.SaveAync();
            if (group == null)
                return new ServiceResultError("User is not Exist!!");

            return new ServiceResultSuccess($"{group.Name} is deleted!");
        }

        #endregion BaseRepo

        #region Create & Search
        public async Task<ServiceResult> CreateTeam(VMCreateTeam vmTeam)
        {
            IEnumerable<Team> listExistTeam = await _dasRepo.Team.GetAllListAsync(g => g.Status == (int)EnumCommon.Status.Active && g.Name == vmTeam.Name);
            if (IsExisted(listExistTeam))
                return new ServiceResultError("Tên nhóm người dùng đã tồn tại!");

            UpdateData(vmTeam);

            Team group = _mapper.Map<Team>(vmTeam);
            await _dasRepo.Team.InsertAsync(group);
            await _dasRepo.SaveAync();
            if (group.ID == 0)
                return new ServiceResultError("Thêm mới nhóm người dùng không thành công!");

            //Người thực hiện phải có quyền "phân quyền" cho user mới có thể  thực hiện
            var isGrant = await _authorizeService.CheckPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Grant });
            if (isGrant && IsExisted(vmTeam.IDGroupPerStrs))
            {
                List<TeamGroupPer> listTeamGroupPer = new List<TeamGroupPer>();
                foreach (var item in vmTeam.IDGroupPerStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    listTeamGroupPer.Add(new TeamGroupPer
                    {
                        IDChannel = group.IDChannel,
                        IDTeam = group.ID,
                        IDGroupPer = int.Parse(item),
                        CreatedBy = group.CreatedBy,
                        CreateDate = DateTime.Now
                    });
                }
                if (IsExisted(listTeamGroupPer))
                    await _dasRepo.TeamGroupPer.InsertAsync(listTeamGroupPer);
            }

            if (IsExisted(vmTeam.IDUserStrs))
            {
                List<UserTeam> listUserTeam = new List<UserTeam>();
                foreach (var item in vmTeam.IDUserStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    listUserTeam.Add(new UserTeam
                    {
                        IDChannel = group.IDChannel,
                        IDTeam = group.ID,
                        IDUser = int.Parse(item),
                        CreatedBy = group.CreatedBy,
                        CreateDate = DateTime.Now
                    });
                }
                if (IsExisted(listUserTeam))
                    await _dasRepo.UserTeam.InsertAsync(listUserTeam);
            }

            await _dasRepo.SaveAync();

            var rs = new ServiceResultSuccess("Thêm mới nhóm người dùng thành công!")
            {
                Data = group.ID
            };
            return rs;
        }

        public async Task<PaginatedList<VMTeam>> SearchByConditionPagging(TeamCondition condition)
        {
            var temp = from g in _dasRepo.Team.GetAll()
                       where ((string.IsNullOrEmpty(condition.Keyword) || g.Name.Contains(condition.Keyword)) && g.Status == (int)EnumCommon.Status.Active)
                       select new VMTeam
                       {
                           ID = g.ID,
                           Name = g.Name,
                           IDChannel = g.IDChannel,
                           Status = g.Status,
                           Description = g.Description
                       };
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            PaginatedList<VMTeam> model = new PaginatedList<VMTeam>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<IEnumerable<VMTeam>> GetListByCondition(TeamCondition condition)
        {
            var temp = from g in _dasRepo.Team.GetAll()
                       where ((string.IsNullOrEmpty(condition.Keyword) || g.Name.Contains(condition.Keyword)) && g.Status == (int)EnumCommon.Status.Active)
                       select new VMTeam
                       {
                           ID = g.ID,
                           Name = g.Name,
                           IDChannel = g.IDChannel,
                           Status = g.Status
                       };
            return await temp.ToListAsync();
        }

        public async Task<int[]> GetUserOfTeam(int idTeam)
        {
            return await _dasRepo.UserTeam.GetAll().Where(x => x.IDTeam == idTeam).Select(s => s.IDUser).Distinct().ToArrayAsync();
        }
        #endregion Create & Search

        #region Update
        public async Task<ServiceResult> UpdateTeam(VMEditTeam vmGroup)
        {
            var group = await _dasRepo.Team.GetAsync(vmGroup.ID);
            IEnumerable<Team> listExisted = await _dasRepo.Team.GetAllListAsync(m => m.Name == vmGroup.Name && m.Name != group.Name);
            if (IsExisted(listExisted))
                return new ServiceResultError("Tên nhóm người dùng đã tồn tại!");

            UpdateData(vmGroup);
            _mapper.Map(vmGroup, group);

            await _dasRepo.Team.UpdateAsync(group);

            //Người thực hiện phải có quyền "phân quyền" cho user mới có thể  thực hiện
            var isGrant = await _authorizeService.CheckPermission((int)EnumModule.Code.S9010, new int[] { (int)EnumPermission.Type.Grant });
            if (isGrant)
            {
                //delete teamgroupper by teamid
                List<TeamGroupPer> listTeamGroupPer = _dasRepo.TeamGroupPer.GetAll().Where(m => m.IDTeam == group.ID).ToList();
                if (IsExisted(listTeamGroupPer))
                    await _dasRepo.TeamGroupPer.DeleteAsync(listTeamGroupPer);

                //insert new Teamrole
                listTeamGroupPer = new List<TeamGroupPer>();
                if (IsExisted(vmGroup.IDGroupPerStrs))
                {
                    foreach (var item in vmGroup.IDGroupPerStrs)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        listTeamGroupPer.Add(new TeamGroupPer
                        {
                            IDChannel = group.IDChannel,
                            IDTeam = group.ID,
                            IDGroupPer = int.Parse(item),
                            CreatedBy = group.CreatedBy,
                            CreateDate = DateTime.Now
                        });
                    }
                    if (IsExisted(listTeamGroupPer))
                        await _dasRepo.TeamGroupPer.InsertAsync(listTeamGroupPer);
                }
            }
            

            //delete userTeam by teamid
            List<UserTeam> listUserTeam = _dasRepo.UserTeam.GetAll().Where(m => m.IDTeam == group.ID).ToList();
            if (IsExisted(listUserTeam))
                await _dasRepo.UserTeam.DeleteAsync(listUserTeam);

            //insert new userTeam
            listUserTeam = new List<UserTeam>();
            if (IsExisted(vmGroup.IDUserStrs))
            {
                foreach (var item in vmGroup.IDUserStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    listUserTeam.Add(new UserTeam
                    {
                        IDChannel = group.IDChannel,
                        IDTeam = group.ID,
                        IDUser = int.Parse(item),
                        CreatedBy = group.CreatedBy,
                        CreateDate = DateTime.Now
                    });
                }
                if (IsExisted(listUserTeam))
                    await _dasRepo.UserTeam.InsertAsync(listUserTeam);
            }

            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Sửa nhóm người dùng thành công!");
        }
        #endregion Update

        #region Get
        public async Task<VMEditTeam> GetTeam(int id)
        {
            var group = await _dasRepo.Team.GetAsync(id);
            if (!IsExisted(group))
                return null;

            VMEditTeam vmGroup = _mapper.Map<VMEditTeam>(group);

            //get IDGroupPers
            var groupPers = await _dasRepo.TeamGroupPer.GetAllListAsync(g => g.Status == (int)EnumCommon.Status.Active && g.IDTeam == id);
            if (IsExisted(groupPers))
                vmGroup.IDGroupPerStrs = groupPers.Select(r => r.IDGroupPer.ToString());

            //get IDUsers
            var users = await _dasRepo.UserTeam.GetAllListAsync(g => g.Status == (int)EnumCommon.Status.Active && g.IDTeam == id);
            if (IsExisted(users))
                vmGroup.IDUserStrs = users.Select(r => r.IDUser.ToString());

            return vmGroup;
        }

        public async Task<VMTeam> GetTeamDetail(int id)
        {
            var group = await _dasRepo.Team.GetAsync(id);
            if (!IsExisted(group))
                return null;

            VMTeam vmGroup = _mapper.Map<VMTeam>(group);

            //get grouppers
            vmGroup.GroupPers = await (from gur in _dasRepo.TeamGroupPer.GetAll()
                                       join r in _dasRepo.GroupPermission.GetAll() on gur.IDGroupPer equals r.ID
                                       where gur.Status == (int)EnumCommon.Status.Active && gur.IDTeam == id && r.Status == (int)EnumCommon.Status.Active
                                       select new GroupPermission
                                       {
                                           Name = r.Name,
                                           ID = r.ID
                                       }).ToListAsync();

            //get users
            vmGroup.Users = await (from ugu in _dasRepo.UserTeam.GetAll()
                                   join r in _dasRepo.User.GetAll() on ugu.IDUser equals r.ID
                                   where ugu.Status == (int)EnumCommon.Status.Active && ugu.IDTeam == id && r.Status == (int)EnumCommon.Status.Active
                                   select new User
                                   {
                                       Name = r.Name,
                                       ID = r.ID
                                   }).ToListAsync();
            return vmGroup;
        }

        public async Task<IEnumerable<Team>> GetActive()
        {
            return await _dasRepo.Team.GetAllListAsync(g => g.Status == (int)EnumCommon.Status.Active);
        }
        #endregion Get

        #region Delete
        public async Task<ServiceResult> DeleteTeam(int id)
        {
            var group = await _dasRepo.Team.GetAsync(id);
            group.Status = (int)EnumCommon.Status.InActive;
            await _dasRepo.Team.UpdateAsync(group);

            IEnumerable<int> idGroupPers = null;
            IEnumerable<int> idUsers = null;
            List<UserGroupPer> userGroupPers = new List<UserGroupPer>();
            //await _dasRepo.Role.DeleteAsync(s => s.ID == id);
            //await _dasRepo.RoleGroupPer.DeleteAsync(s => s.IDRole == id);

            //get teamgroupper
            var teamGroupPers = await _dasRepo.TeamGroupPer.GetAllListAsync(g => g.IDTeam == id && g.Status == (int)EnumCommon.Status.Active);
            if (IsExisted(teamGroupPers))
            {
                teamGroupPers.Select(g => { g.Status = (int)EnumCommon.Status.InActive; return g; }).ToList();
                await _dasRepo.TeamGroupPer.UpdateAsync(teamGroupPers);

                teamGroupPers = teamGroupPers.GroupBy(t => t.IDGroupPer).Select(t => t.First());
                if (IsExisted(teamGroupPers))
                    idGroupPers = teamGroupPers.Select(g => g.IDGroupPer);
            }

            //get userTeam
            var userTeam = await _dasRepo.UserTeam.GetAllListAsync(g => g.IDTeam == id && g.Status == (int)EnumCommon.Status.Active);
            if (IsExisted(userTeam))
            {
                userTeam.Select(g => { g.Status = (int)EnumCommon.Status.InActive; return g; }).ToList();
                await _dasRepo.UserTeam.UpdateAsync(userTeam);

                userTeam = userTeam.GroupBy(t => t.IDUser).Select(t => t.First());
                if (IsExisted(userTeam))
                    idUsers = userTeam.Select(g => g.IDUser);
            }

            //update userGroupPers
            if(IsExisted(idUsers) && IsExisted(idGroupPers))
            {
                foreach(var idUser in idUsers)
                {
                    var userGroupPer = await _dasRepo.UserGroupPer.GetAllListAsync(u => u.IDUser == idUser && u.Status == (int)EnumCommon.Status.Active && idGroupPers.Contains(u.IDGroupPer));
                    userGroupPer.Select(g => { g.Status = (int)EnumCommon.Status.InActive; return g; }).ToList();
                    if (IsExisted(userGroupPer))
                        userGroupPers.AddRange(userGroupPer);
                }
                await _dasRepo.UserGroupPer.UpdateAsync(userGroupPers);
            }

            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa nhóm người dùng thành công!");
        }

        public async Task<ServiceResult> DeleteMultiTeam(IEnumerable<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    await DeleteTeam(id);
                }
            }
            catch (Exception)
            {
                return new ServiceResultSuccess("Xóa nhóm người dùng không thành công!");
            }
            return new ServiceResultSuccess("Xóa nhóm người dùng thành công!");
        }
        #endregion

        #region Common
        public async Task<IEnumerable<int>> UpdateTeamsByGroupPers(IEnumerable<int> groupPerIds, IEnumerable<int> groupIds, int idRemoved)
        {
            if (!IsExisted(groupIds))
                return null;

            //get teamGroupPerRemoveds by idremoved
            var teamGroupPerRemoveds = await _dasRepo.TeamGroupPer.GetAllListAsync(g => g.IDGroupPer == idRemoved && g.Status == (int)EnumCommon.Status.Active);
            if (!IsExisted(teamGroupPerRemoveds))
                return groupIds;

            //remove item in teamGroupPerRemoveds but not in restOfTeamGroupPers
            var teamGroupPerRemovedIDs = teamGroupPerRemoveds.Select(t => t.IDTeam).ToList();
            foreach (var item in teamGroupPerRemovedIDs)
            {
                if (groupIds.Contains(item))
                    groupIds = groupIds.Where(g => g != item).ToList();

            }
            return groupIds;
        }
        #endregion Common

        #region Private method
        private void UpdateData(VMEditTeam vmGroup)
        {
            vmGroup.Status = (int)EnumCommon.Status.Active;
            if (vmGroup.IDGroupPerStrs != null && !string.IsNullOrEmpty(vmGroup.IDGroupPerStrs.First()) && vmGroup.IDGroupPerStrs.First().Contains("["))
                vmGroup.IDGroupPerStrs = JsonConvert.DeserializeObject<List<string>>(vmGroup.IDGroupPerStrs.First());
            if (vmGroup.IDUserStrs != null && !string.IsNullOrEmpty(vmGroup.IDUserStrs.First()) && vmGroup.IDUserStrs.First().Contains("["))
                vmGroup.IDUserStrs = JsonConvert.DeserializeObject<List<string>>(vmGroup.IDUserStrs.First());
        }

        private void UpdateData(VMCreateTeam vmGroup)
        {
            vmGroup.Status = (int)EnumCommon.Status.Active;
            if (vmGroup.IDGroupPerStrs != null && !string.IsNullOrEmpty(vmGroup.IDGroupPerStrs.First()) && vmGroup.IDGroupPerStrs.First().Contains("["))
                vmGroup.IDGroupPerStrs = JsonConvert.DeserializeObject<List<string>>(vmGroup.IDGroupPerStrs.First());
            if (vmGroup.IDUserStrs != null && !string.IsNullOrEmpty(vmGroup.IDUserStrs.First()) && vmGroup.IDUserStrs.First().Contains("["))
                vmGroup.IDUserStrs = JsonConvert.DeserializeObject<List<string>>(vmGroup.IDUserStrs.First());
        }

        private bool IsExisted(Team group)
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