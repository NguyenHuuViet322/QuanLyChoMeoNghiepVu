using DASNotify.Application.Models.CustomModels;
using DASNotify.Domain.Interfaces.DASNotify;
using DASNotify.Domain.Models.DASNotify;
using DASNotify.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DASNotify.Application.Services
{
    public class SendNotificationService : BaseMasterService, ISendNotificationServices
    {
        private readonly string _apiNotify;
        public SendNotificationService(
            IDasNotifyRepositoryWrapper dasNotifyRepository,
            IConfiguration configuration) : base(dasNotifyRepository)
        {
            _apiNotify = configuration["NotifyDomain"];

            if (string.IsNullOrWhiteSpace(_apiNotify))
            {
                throw new Exception("Not found domain Notification Service, please check appsettings config");
            }
        }

        public Task<ServiceResult> Create(Notification model)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<Notification> Get(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Notification>> Gets()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> PushToUsers(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, int UserImpactType = 0)
        {
            try
            {
                List<Notification> listNoti = new List<Notification>();
                foreach (var userId in idsUser)
                {
                    listNoti.Add(new Notification
                    {
                        UserId = userId,
                        Content = content,
                        IsRead = false,
                        CreatedDate = DateTime.Now,
                        Url = url,
                        IDImpactUser = IDImpactUser,
                        IDAffectedObject = IDAffectedObject,
                        AffectedObjectType = AffectedObjectType,
                        IDImpactAgency = IDImpactAgency,
                        IDImpactOrgan = IDImpactOrgan,
                        UserImpactType = UserImpactType
                    });
                }

                await _dasNotifyRepo.Notification.InsertAsync(listNoti);
                await _dasNotifyRepo.SaveAync();
                return new ServiceResultSuccess();
            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }
        public async Task<ServiceResult> PushToUsersPortal(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0)
        {
            try
            {
                List<NotificationPortal> listNoti = new List<NotificationPortal>();
                foreach (var userId in idsUser)
                {
                    listNoti.Add(new NotificationPortal
                    {
                        ReaderId = userId,
                        Content = content,
                        IsRead = false,
                        CreatedDate = DateTime.Now,
                        Url = url,
                        IDImpactUser = IDImpactUser,
                        IDAffectedObject = IDAffectedObject,
                        AffectedObjectType = AffectedObjectType,
                        IDImpactAgency = IDImpactAgency,
                        IDImpactOrgan = IDImpactOrgan,
                    });
                }

                await _dasNotifyRepo.NotificationPortal.InsertAsync(listNoti);
                await _dasNotifyRepo.SaveAync();
                return new ServiceResultSuccess();
            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public Task<ServiceResult> Update(Notification model)
        {
            throw new NotImplementedException();
        }
    }
}
