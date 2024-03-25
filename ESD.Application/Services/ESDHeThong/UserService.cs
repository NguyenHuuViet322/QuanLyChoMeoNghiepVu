using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ESD.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Newtonsoft.Json;
using ESD.Utility;
using ESD.Application.Constants;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;

namespace ESD.Application.Services
{
    public class UserService : BaseMasterService, IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IAuthorizeService _authorizeService;
        private readonly IDistributedCache _cache;
        private readonly ICacheManagementServices _cacheManagementServices;

        public UserService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService userPrincipalService
            , IAuthorizeService authorizeService
            , IDistributedCache cache
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _userPrincipalService = userPrincipalService;
            _authorizeService = authorizeService;
            _cache = cache;
            _cacheManagementServices = cacheManagementServices;
        }

        #region BaseRepo
        public async Task<ServiceResult> Create(User user)
        {
            await _dasRepo.User.InsertAsync(user);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }

        public async Task<ServiceResult> Update(User user)
        {
            await _dasRepo.User.UpdateAsync(user);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add User suceess!");
        }

        public async Task<User> Get(object id)
        {
            return await _dasRepo.User.GetAsync(id);
        }

        public async Task<IEnumerable<User>> Gets()
        {
            return await _dasRepo.User.GetAllListAsync();
        }

        public async Task<ServiceResult> Delete(object id)
        {
            var user = await _dasRepo.User.GetAsync(id);
            await _dasRepo.User.DeleteAsync(user);
            await _dasRepo.SaveAync();
            if (user == null)
                return new ServiceResultError("User is not Exist!!");

            return new ServiceResultSuccess($"{user.Name} is deleted!");
        }

        #endregion BaseRepo

        #region Create & Search
        public async Task<ServiceResult> CreateUser(VMCreateUser vmUser)
        {
            IEnumerable<User> listExistUser = await _dasRepo.User.GetAllListAsync(m => (m.AccountName == vmUser.AccountName)
            || (m.Email == vmUser.Email) || (m.IdentityNumber == vmUser.IdentityNumber) || (!string.IsNullOrEmpty(vmUser.Phone) && m.Phone == vmUser.Phone));

            if (IsExisted(listExistUser))
            {
                if (IsExisted(listExistUser.Where(m => (m.AccountName == vmUser.AccountName))))
                    return new ServiceResultError("Tên tài khoản đã tồn tại!");
                else if (IsExisted(listExistUser.Where(m => (m.Email == vmUser.Email))))
                    return new ServiceResultError("Email đã tồn tại!");
                else if (IsExisted(listExistUser.Where(m => (m.IdentityNumber == vmUser.IdentityNumber))))
                    return new ServiceResultError("Số CMND/Hộ chiếu đã tồn tại!");
                else
                    return new ServiceResultError("Số điện thoại đã tồn tại!");
            }                

            UpdateData(vmUser);

            User user = _mapper.Map<User>(vmUser);
            await _dasRepo.User.InsertAsync(user);
            await _dasRepo.SaveAync(); 
            if(user.ID == 0)
                return new ServiceResultError("Thêm mới người dùng không thành công!");

            //Người thực hiện phải có quyền "phân quyền" cho user mới có thể  thực hiện
            var isGrant = await _authorizeService.CheckPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Grant });
            if (isGrant && IsExisted(vmUser.IDGroupPerStrs))
            {
                List<UserGroupPer> list = new List<UserGroupPer>();
                foreach (var item in vmUser.IDGroupPerStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    list.Add(new UserGroupPer
                    {
                        IDChannel = user.IDChannel,
                        IDUser = user.ID,
                        IDGroupPer = int.Parse(item),
                        CreatedBy = user.CreatedBy,
                        CreateDate = DateTime.Now
                    });
                }
                if (IsExisted(list))
                    await _dasRepo.UserGroupPer.InsertAsync(list);
            }

            if (IsExisted(vmUser.IDTeamStrs))
            {
                List<UserTeam> list = new List<UserTeam>();
                foreach (var item in vmUser.IDTeamStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    list.Add(new UserTeam
                    {
                        IDChannel = user.IDChannel,
                        IDUser = user.ID,
                        IDTeam = int.Parse(item),
                        CreatedBy = user.CreatedBy,
                        CreateDate = DateTime.Now
                    });
                }
                if (IsExisted(list))
                    await _dasRepo.UserTeam.InsertAsync(list);
            }

            await _dasRepo.SaveAync();
            var rs = new ServiceResultSuccess("Thêm mới người dùng thành công!")
            {
                Data = user.ID
            };
            return rs;
        }

        public async Task<PaginatedList<VMUser>> SearchByConditionPagging(UserCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from u in _dasRepo.User.GetAll()
                       from a in _dasRepo.Organ.GetAll().Where(a => a.ID == u.IDOrgan).DefaultIfEmpty()
                       from d in _dasRepo.Agency.GetAll().Where(d => d.ID == u.IDAgency).DefaultIfEmpty()
                       from p in _dasRepo.Position.GetAll().Where(p => p.ID == u.IDPosition).DefaultIfEmpty()
                       let cdKeyword = string.IsNullOrEmpty(condition.Keyword) || u.AccountName.Contains(condition.Keyword) || u.Name.Contains(condition.Keyword)
                       || u.Email.Contains(condition.Keyword) || u.Phone.Contains(condition.Keyword)
                       let cdStatus = condition.ListStatusStr == null || condition.ListStatusStr.Count == 0 || condition.ListStatusStr.Contains(u.Status.ToString())
                       let cdOrgan = (condition.IDOrganStr == null || condition.IDOrganStr.Count == 0 || condition.IDOrganStr.FirstOrDefault() == "-1" || condition.IDOrganStr.Contains(u.IDOrgan.ToString())) && userData.IDOrgan == u.IDOrgan
                       let cdAgency = condition.IDAgencyStr == null || condition.IDAgencyStr.Count == 0 || condition.IDOrganStr.FirstOrDefault() == "-1" || condition.IDAgencyStr.Contains(u.IDAgency.ToString())
                       let cdPosition = condition.IDPositionStr == null || condition.IDPositionStr.Count == 0 || condition.IDPositionStr.Contains(u.IDPosition.ToString())
                       where cdKeyword && cdStatus && cdOrgan && cdAgency && cdPosition
                       orderby u.ID descending
                       select new VMUser
                       {
                           ID = u.ID,
                           AccountName = u.AccountName,
                           Name = u.Name,
                           IDOrgan = u.IDOrgan,
                           OrganName = a.Name,
                           IDAgency = u.IDAgency,
                           AgencyName = d.Name,
                           IDPosition = u.IDPosition,
                           PositionName = p.Name,
                           IDChannel = u.IDChannel,
                           Status = u.Status,
                           Email = u.Email,
                           Phone = u.Phone
                       };
            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            if (!IsExisted(result))
                return null;

            PaginatedList<VMUser> model = new PaginatedList<VMUser>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<IEnumerable<VMUser>> GetListByCondition(UserCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from u in _dasRepo.User.GetAll()
                       from a in _dasRepo.Organ.GetAll().Where(a => a.ID == u.IDOrgan).DefaultIfEmpty()
                       from d in _dasRepo.Agency.GetAll().Where(d => d.ID == u.IDAgency).DefaultIfEmpty()
                       from p in _dasRepo.Position.GetAll().Where(p => p.ID == u.IDPosition).DefaultIfEmpty()
                       let cdKeyword = string.IsNullOrEmpty(condition.Keyword) || u.AccountName.Contains(condition.Keyword) || u.Name.Contains(condition.Keyword)
                       || u.Email.Contains(condition.Keyword) || u.Phone.Contains(condition.Keyword)
                       let cdStatus = condition.ListStatusStr == null || condition.ListStatusStr.Count == 0 || condition.ListStatusStr.Contains(u.Status.ToString())
                       let cdOrgan = condition.IDOrganStr == null || condition.IDOrganStr.Count == 0 || condition.IDOrganStr.FirstOrDefault() == "-1" || condition.IDOrganStr.Contains(u.IDOrgan.ToString()) && userData.IDOrgan == u.IDOrgan
                       let cdAgency = condition.IDAgencyStr == null || condition.IDAgencyStr.Count == 0 || condition.IDOrganStr.FirstOrDefault() == "-1" || condition.IDAgencyStr.Contains(u.IDAgency.ToString())
                       let cdPosition = condition.IDPositionStr == null || condition.IDPositionStr.Count == 0 || condition.IDPositionStr.Contains(u.IDPosition.ToString())
                       where cdKeyword && cdStatus && cdOrgan && cdAgency && cdPosition
                       select new VMUser
                       {
                           ID = u.ID,
                           AccountName = u.AccountName,
                           Name = u.Name,
                           IDOrgan = u.IDOrgan,
                           OrganName = a.Name,
                           IDAgency = u.IDAgency,
                           AgencyName = d.Name,
                           IDPosition = u.IDPosition ?? 0,
                           PositionName = p.Name,
                           IDChannel = u.IDChannel,
                           Status = u.Status,
                           Email = u.Email,
                           Phone = u.Phone
                       };
            return await temp.ToListAsync();
        }

        public async Task<int> GetUserIDByEmail(string email)
        {
            var user = await _dasRepo.User.GetAllListAsync(u => u.Email == email);
            if (!IsExisted(user))
            {
                return 0;
            }

            return user.FirstOrDefault().ID;

        }
        #endregion Create & Search

        #region Update
        public async Task<ServiceResult> UpdateUser(VMEditUser vmUser)
        {
            var user = await _dasRepo.User.GetAsync(vmUser.ID);
            IEnumerable<User> listExistUser = await _dasRepo.User.GetAllListAsync(m => (m.Email == vmUser.Email && m.Email != user.Email)
            || (m.IdentityNumber == vmUser.IdentityNumber && m.IdentityNumber != user.IdentityNumber));
            if (IsExisted(listExistUser))
                if (IsExisted(listExistUser.Where(m => m.Email == vmUser.Email && m.Email != user.Email)))
                    return new ServiceResultError("Email đã tồn tại!");
                else
                    return new ServiceResultError("Số CMND/Hộ chiếu đã tồn tại!");

            UpdateData(vmUser, user);

            if (vmUser.StartDate.HasValue && vmUser.EndDate.HasValue && vmUser.StartDate.Value > vmUser.EndDate.Value)
                return new ServiceResultError("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");

            if (vmUser.IDAgency != user.IDAgency)
            {
                _userPrincipalService.AddUpdateClaim(CustomClaimTypes.IDAgency, vmUser.IDAgency.ToString());
            }

            _mapper.Map(vmUser, user);

            await _dasRepo.User.UpdateAsync(user);

            //Người thực hiện phải có quyền "phân quyền" cho user mới có thể  thực hiện
            var isGrant = await _authorizeService.CheckPermission((int)EnumModule.Code.S9020, new int[] { (int)EnumPermission.Type.Grant });
            if (isGrant)
            {
                //delete usergroupper by userid
                await _dasRepo.UserGroupPer.DeleteAsync(m => m.IDUser == user.ID);

                //insert new usergroupper
                if (IsExisted(vmUser.IDGroupPerStrs))
                {
                    List<UserGroupPer> listUserGroupUser = new List<UserGroupPer>();
                    foreach (var item in vmUser.IDGroupPerStrs)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        listUserGroupUser.Add(new UserGroupPer
                        {
                            IDChannel = user.IDChannel,
                            IDUser = user.ID,
                            IDGroupPer = int.Parse(item),
                            CreatedBy = _userPrincipalService.UserId,
                            CreateDate = DateTime.Now
                        });
                    }
                    if (IsExisted(listUserGroupUser))
                        await _dasRepo.UserGroupPer.InsertAsync(listUserGroupUser);
                }
            }
            

            //delete userTeam by userid
            await _dasRepo.UserTeam.DeleteAsync(m => m.IDUser == user.ID);

            //insert new userTeam
            if (IsExisted(vmUser.IDTeamStrs))
            {
                List<UserTeam> listUserTeam = new List<UserTeam>();
                foreach (var item in vmUser.IDTeamStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    listUserTeam.Add(new UserTeam
                    {
                        IDChannel = user.IDChannel,
                        IDUser = user.ID,
                        IDTeam = int.Parse(item),
                        CreatedBy = _userPrincipalService.UserId,
                        CreateDate = DateTime.Now
                    });
                }
                if (IsExisted(listUserTeam))
                    await _dasRepo.UserTeam.InsertAsync(listUserTeam);
            }

            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật người dùng thành công!");
        }
        #endregion Update
        #region Delete
        public async Task<ServiceResult> Delete(int id)
        {
            await _dasRepo.User.DeleteAsync(s => s.ID == id);
            await _dasRepo.UserGroupPer.DeleteAsync(m => m.IDUser == id);
            await _dasRepo.UserTeam.DeleteAsync(m => m.IDUser == id);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa người dùng thành công!");
        }
        public async Task<ServiceResult> Delete(IEnumerable<int> ids)
        {
            try
            {
                //Logic delete
                var positionDeletes = await _dasRepo.User.GetAllListAsync(n => ids.Contains(n.ID));
                if (positionDeletes == null || positionDeletes.Count() == 0)
                    return new ServiceResultError("Người dùng đã chọn hiện không tồn tại hoặc đã bị xóa");
                await _dasRepo.User.DeleteAsync(s => ids.Contains(s.ID));
                await _dasRepo.UserGroupPer.DeleteAsync(s => ids.Contains(s.IDUser));
                await _dasRepo.UserTeam.DeleteAsync(s => ids.Contains(s.IDUser));
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa người dùng thành công");

            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        #endregion
        #region Get
        public async Task<VMEditUser> GetUser(int id)
        {
            var user = await _dasRepo.User.GetAsync(id);
            if (user == null || user.ID == 0)
                return null;
            var vmUser = _mapper.Map<VMEditUser>(user);

            //get usergrouppers
            var userGroupPers = await _dasRepo.UserGroupPer.GetAllListAsync(u => u.IDUser == id && u.Status == (int)EnumCommon.Status.Active);
            if (IsExisted(userGroupPers))
                vmUser.IDGroupPerStrs = userGroupPers.Select(m => m.IDGroupPer.ToString());

            //get userTeams
            var userTeams = await _dasRepo.UserTeam.GetAllListAsync(u => u.IDUser == id && u.Status == (int)EnumCommon.Status.Active);
            if (!IsExisted(userTeams))
                return vmUser;

            vmUser.IDTeamStrs = userTeams.Select(m => m.IDTeam.ToString());

            //get teamgrouppers
            var teamGroupPers = await _dasRepo.TeamGroupPer.GetAllListAsync(u => vmUser.IDTeamStrs.Contains(u.IDTeam.ToString()) && u.Status == (int)EnumCommon.Status.Active);
            if (!IsExisted(teamGroupPers))
                return vmUser;

            //get idGroupPerStrs
            var idGroupPerStrs = teamGroupPers.Select(g => g.IDGroupPer.ToString());
            if (!IsExisted(vmUser.IDGroupPerStrs))
                vmUser.IDGroupPerStrs = idGroupPerStrs;
            else
                //insert idGroupPerStrs to vmUser.IDGroupPerStrs
                foreach (var item in idGroupPerStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    if (vmUser.IDGroupPerStrs.Contains(item))
                        continue;
                    vmUser.IDGroupPerStrs = vmUser.IDGroupPerStrs.Concat(new[] { item });
                }

            return vmUser;
        }

        public async Task<VMUser> GetUserDetail(int id)
        {
            var user = await _dasRepo.User.GetAsync(id);
            if (user == null || user.ID == 0)
                return null;
            var vmUser = _mapper.Map<VMUser>(user);

            //get usergroupper
            var userGroupPers = await _dasRepo.UserGroupPer.GetAllListAsync(u => u.IDUser == id && u.Status == (int)EnumCommon.Status.Active);
            if (IsExisted(userGroupPers))
                vmUser.IDGroupPerStrs = userGroupPers.Select(m => m.IDGroupPer.ToString());

            //get userTeam
            var userTeams = await _dasRepo.UserTeam.GetAllListAsync(u => u.IDUser == id && u.Status == (int)EnumCommon.Status.Active);
            if (!IsExisted(userTeams))
                return vmUser;

            vmUser.IDTeamStrs = userTeams.Select(m => m.IDTeam.ToString());

            //get teamgrouppers
            var teamGroupPers = await _dasRepo.TeamGroupPer.GetAllListAsync(u => vmUser.IDTeamStrs.Contains(u.IDTeam.ToString()) && u.Status == (int)EnumCommon.Status.Active);
            if (!IsExisted(teamGroupPers))
                return vmUser;

            //get idGroupPerStrs
            var idGroupPerStrs = teamGroupPers.Select(g => g.IDGroupPer.ToString());
            if (!IsExisted(vmUser.IDGroupPerStrs))
                vmUser.IDGroupPerStrs = idGroupPerStrs;
            else
                //insert idRoleStrs to vmUser.IDRoleStrs
                foreach (var item in idGroupPerStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    if (vmUser.IDGroupPerStrs.Contains(item))
                        continue;
                    vmUser.IDGroupPerStrs = vmUser.IDGroupPerStrs.Concat(new[] { item });
                }

            return vmUser;
        }

        public async Task<IEnumerable<User>> GetActive()
        {
            return await _dasRepo.User.GetAllListAsync(u => u.Status == (int)EnumCommon.Status.Active && u.IDOrgan == _userPrincipalService.IDOrgan);
        }
        #endregion Get

        #region Cache
        public async Task UpdateCacheUser(int userId)
        {
            Dictionary<int, UserData> dicUserData;
            dicUserData = await _cache.GetCacheValueAsync<Dictionary<int, UserData>>(CacheConst.USER_DATA);
            var data = await GetDataForUser(userId);
            if (dicUserData == null) //Chưa tồn tại cache => tạo mới cả tập cache
            {
                dicUserData = new Dictionary<int, UserData>();
                dicUserData.Add(userId, data);
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, dicUserData);
            }
            else if (!dicUserData.ContainsKey(userId)) //Chưa tồn tại quyền cho user trong cache => thêm quyền cho user vào
            {
                dicUserData.Add(userId, data);
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, dicUserData);
            }
            else
            {
                dicUserData[userId] = data;
                await _cache.SetCacheValueAsync(CacheConst.USER_DATA, dicUserData);
            }
        }

        /// <summary>
        /// Đưa thông tin của người dùng vào cache
        /// </summary>
        /// <returns></returns>
        public async Task GetDataForUserLogin()
        {
            await UpdateCacheUser(_userPrincipalService.UserId);
        }
        #endregion Cache

        #region SystemManagement
        public async Task<PaginatedList<VMAdminUser>> SearchAdminUserByConditionPagging(UserCondition condition)
        {
            var temp = from u in _dasRepo.User.GetAll()
                       from o in _dasRepo.Organ.GetAll().Where(o => o.ID == u.IDOrgan).DefaultIfEmpty()
                       from p in _dasRepo.Position.GetAll().Where(p => p.ID == u.IDPosition).DefaultIfEmpty()
                       let cdKeyword = string.IsNullOrEmpty(condition.Keyword) || u.AccountName.Contains(condition.Keyword) || u.Name.Contains(condition.Keyword)
                       || u.Email.Contains(condition.Keyword) || u.Phone.Contains(condition.Keyword)
                       let cdStatus = condition.ListStatusStr == null || condition.ListStatusStr.Count == 0 || condition.ListStatusStr.Contains(u.Status.ToString())
                       let cdOrgan = condition.IDOrganStr == null || condition.IDOrganStr.Count == 0 || condition.IDOrganStr.FirstOrDefault() == "-1" || condition.IDOrganStr.Contains(u.IDOrgan.ToString())
                       let cdAgency = u.IDAgency == 0
                       let cdPosition = condition.IDPositionStr == null || condition.IDPositionStr.Count == 0 || condition.IDPositionStr.Contains(u.IDPosition.ToString())
                       where cdKeyword && cdStatus && cdOrgan && cdPosition && cdAgency
                       orderby u.ID descending
                       select new VMAdminUser
                       {
                           ID = u.ID,
                           AccountName = u.AccountName,
                           Name = u.Name,
                           IDOrgan = u.IDOrgan,
                           OrganName = o.Name,
                           IDPosition = u.IDPosition,
                           PositionName = p.Name,
                           IDChannel = u.IDChannel,
                           Status = u.Status,
                           Email = u.Email,
                           Phone = u.Phone
                       };
            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            if (!IsExisted(result))
                return null;

            return new PaginatedList<VMAdminUser>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<VMEditAdminUser> GetAdminUser(int id)
        {
            var user = await _dasRepo.User.GetAsync(id);
            if (user == null || user.ID == 0)
                return null;
            var vmUser = _mapper.Map<VMEditAdminUser>(user);

            return vmUser;
        }

        public async Task<VMAdminUser> GetAdminUserDetail(int id)
        {
            var user = await _dasRepo.User.GetAsync(id);
            if (user == null || user.ID == 0)
                return null;
            var vmUser = _mapper.Map<VMAdminUser>(user);

            return vmUser;
        }

        public async Task<ServiceResult> CreateAdminUser(VMCreateAdminUser vmUser)
        {
            IEnumerable<User> listExistUser = await _dasRepo.User.GetAllListAsync(m => (m.AccountName == vmUser.AccountName)
            || (m.Email == vmUser.Email) || (m.IdentityNumber == vmUser.IdentityNumber) || (!string.IsNullOrEmpty(vmUser.Phone) && m.Phone == vmUser.Phone));

            if (IsExisted(listExistUser))
                if (IsExisted(listExistUser.Where(m => (m.AccountName == vmUser.AccountName))))
                    return new ServiceResultError("Tên tài khoản đã tồn tại!");
                else if (IsExisted(listExistUser.Where(m => (m.Email == vmUser.Email))))
                    return new ServiceResultError("Email đã tồn tại!");
                else if (IsExisted(listExistUser.Where(m => (m.IdentityNumber == vmUser.IdentityNumber))))
                    return new ServiceResultError("Số CMND/Hộ chiếu đã tồn tại!");
                else
                    return new ServiceResultError("Số điện thoại đã tồn tại!");

            UpdateData(vmUser);
            var groupPer = await _dasRepo.GroupPermission.GetAll().Where(m => m.Name == CommonConst.AdminOrgan).FirstOrDefaultAsync();
            if(groupPer == null || groupPer.ID == 0 || groupPer.Status == (int)EnumCommon.Status.InActive)
                return new ServiceResultError("Thêm mới người dùng không thành công!");

            User user = _mapper.Map<User>(vmUser);
            await _dasRepo.User.InsertAsync(user);
            await _dasRepo.SaveAync();
            if(user.ID == 0)
                return new ServiceResultError("Thêm mới người dùng không thành công!");

            //insert groupper
            UserGroupPer userGroupPer = new UserGroupPer
            {
                IDGroupPer = groupPer.ID,
                IDUser = user.ID,
                Status = (int)EnumCommon.Status.Active
            };
            await _dasRepo.UserGroupPer.InsertAsync(userGroupPer);
            await _dasRepo.SaveAync();

            var rs = new ServiceResultSuccess("Thêm mới người dùng thành công!")
            {
                Data = user.ID
            };
            return rs;
        }

        public async Task<ServiceResult> UpdateAdminUser(VMEditAdminUser vmUser)
        {
            var user = await _dasRepo.User.GetAsync(vmUser.ID);
            IEnumerable<User> listExistUser = await _dasRepo.User.GetAllListAsync(m => (m.Email == vmUser.Email && m.Email != user.Email)
            || (m.IdentityNumber == vmUser.IdentityNumber && m.IdentityNumber != user.IdentityNumber));
            if (IsExisted(listExistUser))
                if (IsExisted(listExistUser.Where(m => m.Email == vmUser.Email && m.Email != user.Email)))
                    return new ServiceResultError("Email đã tồn tại!");
                else
                    return new ServiceResultError("Số CMND/Hộ chiếu đã tồn tại!");

            UpdateData(vmUser, user);

            if (vmUser.StartDate.HasValue && vmUser.EndDate.HasValue && vmUser.StartDate.Value > vmUser.EndDate.Value)
                return new ServiceResultError("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");
            var groupPer = await _dasRepo.GroupPermission.GetAll().Where(m => m.Name == CommonConst.AdminOrgan).FirstOrDefaultAsync();
            if (groupPer == null || groupPer.ID == 0 || groupPer.Status == (int)EnumCommon.Status.InActive)
                return new ServiceResultError("Sửa người dùng không thành công!");

            if (vmUser.IDOrgan != user.IDOrgan)
            {
                _userPrincipalService.AddUpdateClaim(CustomClaimTypes.IDOrgan, vmUser.IDOrgan.ToString());
            }

            _mapper.Map(vmUser, user);
            await _dasRepo.User.UpdateAsync(user);

            //insert groupper
            var userGrouPermission = await _dasRepo.UserGroupPer.GetAll().Where(m => m.IDGroupPer == groupPer.ID).FirstOrDefaultAsync();
            if(userGrouPermission == null || userGrouPermission.ID == 0 || userGrouPermission.Status == (int)EnumCommon.Status.InActive)
            {
                UserGroupPer userGroupPer = new UserGroupPer
                {
                    IDGroupPer = groupPer.ID,
                    IDUser = user.ID,
                    Status = (int)EnumCommon.Status.Active
                };
                await _dasRepo.UserGroupPer.InsertAsync(userGroupPer);
            }
            

            await _dasRepo.SaveAync();

            var rs = new ServiceResultSuccess("Thêm mới người dùng thành công!")
            {
                Data = user.ID
            };
            return rs;
        }
        #endregion SystemManagement

        #region Private method
        private void UpdateData(VMEditUser vMEditUser, User user)
        {
            DateTime.TryParseExact(vMEditUser.StartDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            vMEditUser.StartDate = date;
            DateTime.TryParseExact(vMEditUser.EndDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            vMEditUser.EndDate = date;
            if (vMEditUser.IDGroupPerStrs != null && !string.IsNullOrEmpty(vMEditUser.IDGroupPerStrs.First()) && vMEditUser.IDGroupPerStrs.First().Contains("["))
                vMEditUser.IDGroupPerStrs = JsonConvert.DeserializeObject<List<string>>(vMEditUser.IDGroupPerStrs.First());
            if (vMEditUser.IDTeamStrs != null && !string.IsNullOrEmpty(vMEditUser.IDTeamStrs.First()) && vMEditUser.IDTeamStrs.First().Contains("["))
                vMEditUser.IDTeamStrs = JsonConvert.DeserializeObject<List<string>>(vMEditUser.IDTeamStrs.First());
            vMEditUser.Password = user.Password;
            vMEditUser.AccountName = user.AccountName;
        }

        private void UpdateData(VMCreateUser vmUser)
        {
            DateTime.TryParseExact(vmUser.StartDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            vmUser.StartDate = date;
            DateTime.TryParseExact(vmUser.EndDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            vmUser.EndDate = date;
            vmUser.Password = StringUltils.Md5Encryption(vmUser.Password);
            if (vmUser.IDGroupPerStrs != null && !string.IsNullOrEmpty(vmUser.IDGroupPerStrs.First()) && vmUser.IDGroupPerStrs.First().Contains("["))
                vmUser.IDGroupPerStrs = JsonConvert.DeserializeObject<List<string>>(vmUser.IDGroupPerStrs.First());
            if (vmUser.IDTeamStrs != null && !string.IsNullOrEmpty(vmUser.IDTeamStrs.First()) && vmUser.IDTeamStrs.First().Contains("["))
                vmUser.IDTeamStrs = JsonConvert.DeserializeObject<List<string>>(vmUser.IDTeamStrs.First());
        }

        private void UpdateData(VMEditAdminUser vMEditUser, User user)
        {
            DateTime.TryParseExact(vMEditUser.StartDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            vMEditUser.StartDate = date;
            DateTime.TryParseExact(vMEditUser.EndDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            vMEditUser.EndDate = date;
            vMEditUser.Password = user.Password;
            vMEditUser.AccountName = user.AccountName;
        }

        private void UpdateData(VMCreateAdminUser vmUser)
        {
            DateTime.TryParseExact(vmUser.StartDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            vmUser.StartDate = date;
            DateTime.TryParseExact(vmUser.EndDateStr, CommonConst.DfDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            vmUser.EndDate = date;
            vmUser.Password = StringUltils.Md5Encryption(vmUser.Password);
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }

        private async Task<UserData> GetDataForUser(int userId)
        {
            var rs = await (from u in _dasRepo.User.GetAll()
                            where u.ID == userId
                            join a in _dasRepo.Agency.GetAll() on u.IDAgency equals a.ID
                            select new UserData
                            {
                                IDAgency = u.IDAgency,
                                IDOrgan = u.IDOrgan,
                                ParentPath = a.ParentPath,
                                HasOrganPermission = u.HasOrganPermission
                            }).FirstOrDefaultAsync();

            return rs;
        }

        public async Task<List<VMUser>> GetListAll()
        {
            var result = from u in _dasRepo.User.GetAll()
                         select new VMUser { 
                         ID=u.ID,
                         Name=u.Name,
                         Email=u.Email
                         };
            return await result.ToListAsync();
        }
        #endregion Private method
    }
}