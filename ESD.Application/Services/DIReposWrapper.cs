using ESD.Application.Interfaces;
using ESD.Application.Interfaces.DasKTNN;
using ESD.Application.Services.DasKTNN;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Infrastructure.ContextAccessors;
using ESD.Infrastructure.Repositories.DAS;
using ESD.Infrastructure.Repositories.DASNotify;
using ESD.Infrastructure.Repositories.ESDNghiepVu;
using Microsoft.Extensions.DependencyInjection;

namespace ESD.Application.Services
{
    public static class DIReposWrapper
    {
         
        public static void DependencyInjectionRepository(this IServiceCollection services)
        {
            services.AddScoped<IDasNotifyRepositoryWrapper, DasNotifyRepositoryWrapper>();
            services.AddScoped<IDasRepositoryWrapper, DasRepositoryWrapper>();
            services.AddScoped<IUserPrincipalService, UserPrincipalService>();
            services.AddScoped<ILogBySqlRepository, LogBySqlRepository>();
            services.AddScoped<IESDNghiepVuRepositoryWrapper, ESDNghiepVuRepositoryWrapper>();
        }
    }
}
