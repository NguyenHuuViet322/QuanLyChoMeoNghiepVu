using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ESD.Application.Interfaces;
using ESD.Application.Services;
using ESD.Domain.Interfaces.DAS;
using ESD.Infrastructure.Repositories.DAS;
using ESD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
//using ESD.Infrastructure.HttpClientAccessors.Interfaces;
//using ESD.Infrastructure.HttpClientAccessors.Implementations;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Infrastructure.Repositories.DASNotify;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.AutoMapper;
using ESD.Utility.LogUtils;
using ESD.Infrastructure.DapperORM;
using ESD.Infrastructure.Repositories.DASKTNN;
using System.Collections.Generic;
using ESD.Utility;
using ESD.Infrastructure.Notifications;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using ESD.Infrastructure.HttpClientAccessors.Interfaces;
using ESD.Infrastructure.HttpClientAccessors.Implementations;
using Microsoft.AspNetCore.Authentication;
using WebApi.Helpers;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Infrastructure.Repositories.ESDNghiepVu;

namespace DAS.MobileApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
            //Config NLog
            var appBasePath = string.IsNullOrEmpty(Configuration["LogsFolder"]) ? Directory.GetCurrentDirectory() : Configuration["LogsFolder"];
            NLog.GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
            NLog.LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IBaseHttpClient, BaseHttpClient>();   //Transient, don't Inject to Scope or Singleton
            services.AddSingleton<IBaseHttpClientFactory, BaseHttpClientFactory>();
            services.AddSingleton<ILogHttpClient, LogHttpClient>();

           // services.AddSingleton<IStgFileClientService, StgFileClientService>(); //service use BaseHttpClientFactory must be Singleton
           // services.AddSingleton<ISendNotificationServices, SendNotificationService>(); //service use BaseHttpClientFactory must be Singleton
           // services.AddSingleton<IDasClientService, DasClientService>(); //service use BaseHttpClientFactory must be Singleton
          //  services.AddSingleton<IDasLtlsClientService, DasLtlsClientService>(); //service use BaseHttpClientFactory must be Singleton
           // services.AddSingleton<IHangfireClientServices, HangfireClientService>(); //service use BaseHttpClientFactory must be Singleton

           services.AddControllers();
            // services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\DacPV\Downloads\du-an-das-master"));
            #region - Authen bear
            //services.AddAuthentication(o =>
            //{
            //    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(cfg =>
            //{
            //    cfg.RequireHttpsMetadata = false;
            //    cfg.SaveToken = true;

            //    cfg.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidIssuer = Configuration["Tokens:Issuer"],
            //        ValidAudience = Configuration["Tokens:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
            //        //Do not check the expiry of token
            //        ValidateLifetime = true
            //    };
            //});
            #endregion

            #region -- authen basic
            services.AddAuthentication("BasicAuthentication")
               .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            #endregion
            services.AddCors(o => o.AddPolicy("DASCorsPolicy", b =>
            {
                b.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
            }));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MobileApi",
                    Version = "v1",
                    Description = "A DAS MobileApi",
                    Contact = new OpenApiContact
                    {
                        Name = "ESD",
                        Email = "ESD@ESDGlobal.com.vn"
                    }
                });

                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
            services.AddDbContext<ESDContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASContext")));
            services.AddDbContext<ESDNotifyContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASNotify")));
            services.AddDbContext<ESDNGHIEPVUContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASNghiepVu")));
            //Add dapper connection
            ConfigDapperContext(services);
            services.AddSingleton<ILoggerManager, LoggerManager>();
            // Config HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //DistributedCache
            if (Configuration["CacheType"] == "1" || Configuration["CacheType"] == null)
            {
                services.AddDistributedMemoryCache();
            }
            //else if (Configuration["CacheType"] == "2")
            //{
            //    services.AddDistributedSqlServerCache(o =>
            //    {
            //        o.ConnectionString = Configuration.GetConnectionString("DASCache");
            //        o.SchemaName = "dbo";
            //        o.TableName = "CacheTable";
            //        o.DefaultSlidingExpiration = TimeSpan.FromDays(365);
            //        o.ExpiredItemsDeletionInterval = TimeSpan.FromDays(365);
            //    });
            //}
            //else if (Configuration["CacheType"] == "3")
            //{
            //    // config redis cache
            //}

            //Inject repos
            services.AddScoped<IDasNotifyRepositoryWrapper, DasNotifyRepositoryWrapper>();
            services.AddScoped<IDasRepositoryWrapper, DasRepositoryWrapper>();
            services.AddScoped<IUserPrincipalService, UserPrincipalService>();
            services.AddScoped<ILogBySqlRepository, LogBySqlRepository>();
            services.AddScoped<IESDNghiepVuRepositoryWrapper, ESDNghiepVuRepositoryWrapper>();

            //services.AddSingleton<IConnectionManager, ConnectionManager>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IHubNotificationHelper, HubNotificationHelper>();


            //Inject logic services
            services.DependencyInjectionService();

            //AutoMapper
            DasAutoMapper.Configure(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("DASCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Api v1.1");
                s.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void ConfigDapperContext(IServiceCollection services)
        {
            //For one Database
            //services.AddTransient<IDbConnection>(o => new SqlConnection(Configuration.GetConnectionString("DASContext")));

            //ForMutipleDatabase
            var connectionDict = new Dictionary<DatabaseConnectionName, string>
            {
                { DatabaseConnectionName.DasDataConnection, this.Configuration.GetConnectionString("DasNghiepVu") },
                { DatabaseConnectionName.APIManageDBConnection, this.Configuration.GetConnectionString("DASAPIManage") },
                //{ DatabaseConnectionName.DbThoConnetion, this.Configuration.GetConnectionString("DbThoContext") }
            };

            // Inject this dict
            services.AddSingleton<IDictionary<DatabaseConnectionName, string>>(connectionDict);

            // Inject the factory
            services.AddScoped<IDbConnectionFactory, DapperDbConnectionFactory>();
            services.AddScoped<IDasDataDapperRepo, DasDataDapperRepo>();
            services.AddScoped<IDynamicDBDapperRepo, DynamicDBDapperRepo>();

        }
    }
}
