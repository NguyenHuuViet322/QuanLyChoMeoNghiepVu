using DAS.Utility;
using DAS.Utility.LogUtils;
using DASAuthServer.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DAS.AuthServer.Configuration;
using IdentityServer4.Configuration;

namespace DAS.AuthServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            //Config NLog
            var appBasePath = System.IO.Directory.GetCurrentDirectory();
            NLog.GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
            NLog.LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();

            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var authServerOptions = Configuration.GetSection(nameof(AuthServerOptions)).Get<AuthServerOptions>();

            services.AddDbContext<IdentityServerConfigurationDbContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASAuthServer")));
            services.AddDbContext<IdentityServerPersistedGrantDbContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASAuthServer")));

            services.AddSingleton<ILoggerManager, LoggerManager>();

            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction.LoginUrl = authServerOptions.LoginUrl; // The LoginUrl should be local url which identity server could help redirecting user to login
                options.UserInteraction.LogoutUrl = authServerOptions.LogoutUrl;

                options.Authentication = new AuthenticationOptions
                {
                    CookieLifetime = TimeSpan.FromHours(1),
                    CookieSlidingExpiration = true
                };
            })
                .AddConfigurationStore<IdentityServerConfigurationDbContext>()
                .AddOperationalStore<IdentityServerPersistedGrantDbContext>();

            if (Environment.IsDevelopment())
            {
                // Adding Developer Signing Credential, This will generate tempkey.rsa file 
                // In Production need to provide the asymmetric key pair (certificate or rsa key) that support RSA with SHA256.
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                // ignore
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
