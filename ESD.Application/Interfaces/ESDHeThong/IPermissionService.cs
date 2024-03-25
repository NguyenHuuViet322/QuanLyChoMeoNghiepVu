using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionForGroupPer>> GetPermissionWithModule();
        Task<IEnumerable<GroupPermission>> GetListBasePermission();
        Task<IEnumerable<PermissionForGroupPer>> GetPermissionForEditGroupPer(int IdGroupPer);
        Task UpdateCachePermission(int UserId);
        Task AddCachePermission(int UserId);
        Task UpdateCachePermission(int[] UserIds);
        Task UpdateCachePermissionByIdGroupPer(int GroupPermissionId);
        Task LoadCacheAllPermission();
    }
}