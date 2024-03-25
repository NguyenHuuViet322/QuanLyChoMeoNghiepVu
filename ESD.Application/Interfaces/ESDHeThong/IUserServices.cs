using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;

namespace ESD.Application.Interfaces
{
    public interface IUserService : IBaseMasterService<User>
    {
        Task<ServiceResult> CreateUser(VMCreateUser user);
        Task<ServiceResult> UpdateUser(VMEditUser user);
        Task<PaginatedList<VMUser>> SearchByConditionPagging(UserCondition condition);
        Task<VMEditUser> GetUser(int id);
        Task<VMUser> GetUserDetail(int id);
        Task<IEnumerable<VMUser>> GetListByCondition(UserCondition condition);
        Task<IEnumerable<User>> GetActive();
        Task<List<VMUser>> GetListAll();
        Task<int> GetUserIDByEmail(string email);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);

        #region Cache
        Task UpdateCacheUser(int userId);
        Task GetDataForUserLogin();
        #endregion Cache

        #region SystemManagement
        Task<PaginatedList<VMAdminUser>> SearchAdminUserByConditionPagging(UserCondition condition);
        Task<VMEditAdminUser> GetAdminUser(int id);
        Task<VMAdminUser> GetAdminUserDetail(int id);
        Task<ServiceResult> CreateAdminUser(VMCreateAdminUser vmUser);
        Task<ServiceResult> UpdateAdminUser(VMEditAdminUser vmUser);
        #endregion SystemManagement
    }
}