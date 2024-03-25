using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Models.DASNotify;
using ESD.Infrastructure.HttpClientAccessors.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ESD.Application.Services
{
    public class SendNotificationService : ISendNotificationServices
    {
        private readonly IBaseHttpClientFactory _clientFatory;
        private readonly string _apiNotify;
        protected IDasNotifyRepositoryWrapper _dasNotifyRepo;
        public SendNotificationService(
            IConfiguration configuration
             , IDasNotifyRepositoryWrapper dasNotifyRepo,
            IBaseHttpClientFactory factory)
        {
            _clientFatory = factory;
            _dasNotifyRepo = dasNotifyRepo;
            _apiNotify = configuration["NotifyDomain"];

            if (string.IsNullOrWhiteSpace(_apiNotify))
            {
                throw new Exception("Not found domain Notification Service, please check appsettings config");
            }
        }


        public async Task<ServiceResult> PushToUsers(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, int UserImpactType = 0, string accessToken = "")
        {
            var client = _clientFatory.Create();
            var apiUrl = "api/Notify/PushToUsers";
            var model = new VMSendNotification
            {
                idsUser = idsUser,
                content = content,
                url = url,
                IDImpactUser = IDImpactUser,
                IDAffectedObject = IDAffectedObject,
                AffectedObjectType = AffectedObjectType,
                IDImpactAgency = IDImpactAgency,
                IDImpactOrgan = IDImpactOrgan,
                UserImpactType = UserImpactType
            };
            var response = true;
            //ToDo DacPV Fix tạm  chua chạy được source notify
            List<Notification> listNoti = new List<Notification>();
            foreach (var userId in model.idsUser)
            {
                listNoti.Add(new Notification
                {
                    UserId = userId,
                    Content = model.content,
                    IsRead = false,
                    CreatedDate = DateTime.Now,
                    Url = model.url
                });
            }
            await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            await _dasNotifyRepo.SaveAync();
            //var response = await client.PostAsync(_apiNotify, apiUrl, model, null, null, accessToken);

            if (response)
            {
                return new ServiceResultSuccess("Push Notify success", response);
            }
            else
            {
                return new ServiceResultError("Push Notify Fail");
            }
        }
        public async Task<ServiceResult> PushToUsersPortal(int[] idsUser, string content, string url, int IDImpactUser = 0, int IDAffectedObject = 0, int AffectedObjectType = 0, int IDImpactAgency = 0, int IDImpactOrgan = 0, string accessToken = "")
        {
            var client = _clientFatory.Create();
            var apiUrl = "api/Notify/PushToUsersPortal";
            var model = new VMSendNotificationPortal
            {
                idsUser = idsUser,
                content = content,
                url = url,
                IDImpactUser = IDImpactUser,
                IDAffectedObject = IDAffectedObject,
                AffectedObjectType = AffectedObjectType,
                IDImpactAgency = IDImpactAgency,
                IDImpactOrgan = IDImpactOrgan,
            };
            var response = await client.PostAsync<ServiceResult>(_apiNotify, apiUrl, model, null, null, accessToken);
            //if (response)
            //{
            //    return new ServiceResultSuccess("Push Notify success", response);
            //}
            //else
            //{
            //    return new ServiceResultError("Push Notify Fail");
            //}
            return response;
        }
    }
}
