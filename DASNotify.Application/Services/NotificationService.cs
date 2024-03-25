using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DASNotify.Application.Interfaces;
using DASNotify.Application.Models.CustomModels;
using DASNotify.Application.Models.ViewModels;
using DASNotify.Domain.Interfaces.DASNotify;
using DASNotify.Domain.Models.DASNotify;

namespace DASNotify.Application.Services
{
    public class NotificationService : BaseMasterService, INotificationService
    {
        public NotificationService(IDasNotifyRepositoryWrapper dasNotifyRepository) : base(dasNotifyRepository)
        {

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

        public async Task<ServiceResult> PushToUsers(VMSendNotification model)
        {
            try
            {
                List<Notification> listNoti = new List<Notification>();
                foreach (var userId in model.idsUser)
                {
                    listNoti.Add(new Notification
                    {
                        UserId = userId,
                        Content = model.content,
                        IsRead = false,
                        CreatedDate = DateTime.Now,
                        Url = model.url,
                        IDImpactUser = model.IDImpactUser,
                        IDAffectedObject = model.IDAffectedObject,
                        AffectedObjectType = model.AffectedObjectType,
                        IDImpactAgency = model.IDImpactAgency,
                        IDImpactOrgan = model.IDImpactOrgan,
                        UserImpactType = model.UserImpactType
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
        public async Task<ServiceResult> PushToUsersPortal(VMSendNotificationPortal model)
        {
            try
            {
                List<NotificationPortal> listNoti = new List<NotificationPortal>();
                foreach (var userId in model.idsUser)
                {
                    listNoti.Add(new NotificationPortal
                    {
                        ReaderId = userId,
                        Content = model.content,
                        IsRead = false,
                        CreatedDate = DateTime.Now,
                        Url = model.url,
                        IDImpactUser = model.IDImpactUser,
                        IDAffectedObject = model.IDAffectedObject,
                        AffectedObjectType = model.AffectedObjectType,
                        IDImpactAgency = model.IDImpactAgency,
                        IDImpactOrgan = model.IDImpactOrgan,
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
