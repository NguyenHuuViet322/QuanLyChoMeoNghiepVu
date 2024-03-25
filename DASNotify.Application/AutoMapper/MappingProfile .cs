using DASNotify.Application.Models.ViewModels;
using DASNotify.Domain.Models.DASNotify;
using Profile = AutoMapper.Profile;

namespace DASNotify.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VMNotification, Notification>();
            CreateMap<Notification, VMNotification>();
            CreateMap<VMNotificationPortal, NotificationPortal>();
            CreateMap<NotificationPortal, VMNotificationPortal>();
        }
    }
}