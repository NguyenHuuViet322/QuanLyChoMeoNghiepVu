using DASNotify.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DASNotify.Application.Services
{
    public static class DIServiceWrapper
    {
        public static void DependencyInjectionService(this IServiceCollection services)
        {
            // HttpClientService
            //services.AddScoped<IHttpClientService, HttpClientService>();
            //services.AddScoped<IIPAddressClientServices, IPAddressClientService>();
            //services.AddScoped<ISendNotificationServices, SendNotificationService>();
            services.AddScoped<INotificationService, NotificationService>();
        }
    }
}
