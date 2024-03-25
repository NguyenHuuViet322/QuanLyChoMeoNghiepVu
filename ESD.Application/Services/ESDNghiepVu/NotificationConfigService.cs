using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Interfaces.DasKTNN;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.DasKTNN;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Infrastructure.ContextAccessors;
using ESD.Infrastructure.Repositories.DASKTNN;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ESD.Domain.Enums;
using ESD.Utility;
using ESD.Application.Models.CustomModels;
using System.Collections;
using ESD.Domain.Interfaces.ESDNghiepVu;

namespace ESD.Application.Services.DasKTNN
{
    public class NotificationConfigService : BaseMasterService, INotificationConfigService
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserService _userService;
        private readonly ICacheManagementServices _cacheManagementServices;
        private readonly IUserPrincipalService _userPrincipalService;
        #endregion

        #region Ctor
        public NotificationConfigService(IDasRepositoryWrapper dasRepository
            , ILoggerManager logger
            , IMapper mapper, ICacheManagementServices cacheManagementServices
            , IUserService userService
            , IUserPrincipalService iUserPrincipalService
            , IESDNghiepVuRepositoryWrapper dasKTNNRepositoryWrapper
            , IDasDataDapperRepo dasDapperRepo
           ) : base(dasRepository, dasDapperRepo, dasKTNNRepositoryWrapper)
        {
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _dasNghiepVuRepo = dasKTNNRepositoryWrapper;
            _cacheManagementServices = cacheManagementServices;
            _userPrincipalService = iUserPrincipalService;
        }
        #endregion


        #region Gets
        public async Task<VMNotificationConfig> SearchByCondition(NotificationConfigCondition condition)
        {
            var model = new VMNotificationConfig();
            model.Condition = condition;
            var temp = from gp in _dasRepo.GroupPermission.GetAll()
                       where (condition.Keyword.IsEmpty() || gp.Name.Contains(condition.Keyword)) && gp.Status == (int)EnumGroupPermission.Status.Active
                       select new NotificationConfig
                       {
                           ID = gp.ID,
                           Name = gp.Name,
                           ActiveNotification = gp.ActiveNotification,
                       };

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.NotificationConfigs = new PaginatedList<NotificationConfig>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<ServiceResult> Update(Hashtable Data)
        {
            try
            {
                var id = Utils.GetInt(Data, "ID");
                var activeNotification = Utils.GetInt(Data, "ActiveNotification");
                var groupPermissionUpdate = await _dasRepo.GroupPermission.GetAsync(id);
                if (groupPermissionUpdate == null)
                    return new ServiceResultError("Nhóm quyền này hiện không tồn tại hoặc đã bị xóa");

                groupPermissionUpdate.ActiveNotification = activeNotification;
                await _dasRepo.GroupPermission.UpdateAsync(groupPermissionUpdate);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion
    }
}
