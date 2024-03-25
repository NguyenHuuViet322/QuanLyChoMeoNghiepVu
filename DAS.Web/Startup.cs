using ESD.Application.AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Middwares;
using ESD.Application.Services;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Infrastructure.ContextAccessors;
using ESD.Infrastructure.Contexts;
using ESD.Infrastructure.Notifications;
using ESD.Infrastructure.Repositories.DAS;
using ESD.Infrastructure.Repositories.DASNotify;
using ESD.Utility;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DAS.Web.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ESD.Utility.Binding;
using ESD.Infrastructure.Repositories.DASKTNN;
using ESD.Infrastructure.DapperORM;
using System.Collections.Generic;
using ESD.Domain.Models.DAS;
using ESD.Application.Enums;
using DAS.Web.Constants;
//using LinqToDB.DataProvider.Oracle;
///using Oracle.EntityFrameworkCore
namespace DAS.Web
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
            var authServerConfiguration = Configuration.GetSection(nameof(AuthServerConfiguration)).Get<AuthServerConfiguration>();

            services.AddDbContext<ESDContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASContext")));
            services.AddDbContext<ESDNotifyContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASNotify")));
            services.AddDbContext<ESDNGHIEPVUContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DasNghiepVu")));
            //Add dapper connection
            ConfigDapperContext(services);
            services.AddSingleton<ILoggerManager, LoggerManager>();

            // Config HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (Configuration["UseAuthServer"] == "false" || Configuration["UseAuthServer"] == null)
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.AccessDeniedPath = "/Error/Error/";
                        options.ExpireTimeSpan = TimeSpan.FromHours(1);
                        options.SlidingExpiration = true;
                    });
            }
            else
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.AccessDeniedPath = "/Error/Error/";
                        options.ExpireTimeSpan = TimeSpan.FromHours(1);
                        options.SlidingExpiration = true;
                    })
                    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                    {
                        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.Authority = authServerConfiguration.IdentityServerBaseUrl;
                        options.RequireHttpsMetadata = authServerConfiguration.RequireHttpsMetadata;

                        options.ClientId = authServerConfiguration.ClientId;
                        options.ClientSecret = authServerConfiguration.ClientSecret;
                        options.ResponseType = "code id_token";

                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;

                        // add ClientScopes
                        foreach (var clientScope in authServerConfiguration.ClientScopes)
                        {
                            options.Scope.Add(clientScope);
                        }
                    });
            }

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
                    Title = "ADC API",
                    Version = "v1",
                    Description = "A ADC API",
                    Contact = new OpenApiContact
                    {
                        Name = "dacpv",
                        Email = "dacpv@eds.com.vn"
                    }
                });
            });

            //DistributedCache
            //services.AddDistributedSqlServerCache(o =>
            //{
            //    o.ConnectionString = ConfigUtils.GetConnectionString("DASCache");
            //    o.SchemaName = "dbo";
            //    o.TableName = "CacheTable";
            //});
            services.AddDistributedMemoryCache();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSignalR();

            //Inject repos
            services.DependencyInjectionRepository();

            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHubNotificationHelper, HubNotificationHelper>();


            //Inject logic services
            services.DependencyInjectionService();

            //bind datetime
            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            });
            //AutoMapper
            DasAutoMapper.Configure(services);

            ////Generate Html To PDF
            //services.AddWkhtmltopdf();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                app.UseMiddleware<ExceptionMiddleware>();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Api v1.1");
                s.RoutePrefix = "swagger";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<NotificationHub>("/NotificationHub");
            });

            SettingRouter(app);

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".properties"] = "application/octet-stream";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
            // UpdatePermissonTable
            UpdatePermissonTable(app);

            //Load cache
            var permissionService = (IPermissionService)serviceProvider.GetService(typeof(IPermissionService));
            permissionService.LoadCacheAllPermission().Wait();
        }

        private void ConfigDapperContext(IServiceCollection services)
        {
            //For one Database
            //services.AddTransient<IDbConnection>(o => new SqlConnection(Configuration.GetConnectionString("DASContext")));

            //ForMutipleDatabase
            var connectionDict = new Dictionary<DatabaseConnectionName, string>
            {
                { DatabaseConnectionName.DasDataConnection, this.Configuration.GetConnectionString("DASKTNN") },
            };

            // Inject this dict
            services.AddSingleton<IDictionary<DatabaseConnectionName, string>>(connectionDict);

            // Inject the factory
            services.AddScoped<IDbConnectionFactory, DapperDbConnectionFactory>();
            services.AddScoped<IDasDataDapperRepo, DasDataDapperRepo>();

            //services.AddScoped<IDbThoDapperRepo, DbThoDapperRepo>();

        }

        private static void SettingRouter(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                #region Thông tin cán bộ
                endpoints.MapControllerRoute(CustomsConfig.RouterThongTinCanBo,
                    "can-bo",
                    new { controller = CustomsConfig.ThongTinCanBo, action = "Index" });
                endpoints.MapControllerRoute("ThemMoiCanBo",
                        "them-moi-can-bo",
                        new { controller = CustomsConfig.ThongTinCanBo, action = "Create" });
                endpoints.MapControllerRoute("ChinhSuaCanBo",
                        "chinh-sua-can-bo",
                        new { controller = CustomsConfig.ThongTinCanBo, action = "Edit" });
                endpoints.MapControllerRoute("ChiTietCanBo",
                        "Chi-tiet-can-bo",
                        new { controller = CustomsConfig.ThongTinCanBo, action = "Details" });
                endpoints.MapControllerRoute("XoaCanBo",
                        "xoa-can-bo",
                        new { controller = CustomsConfig.ThongTinCanBo, action = "Delete" });
                endpoints.MapControllerRoute("XuatDuLieu",
                        "xuat-du-lieu",
                        new { controller = CustomsConfig.ThongTinCanBo, action = "Export" });


                endpoints.MapControllerRoute("DongVatNghiepVu", "DongVatNghiepVu/{Type}", new { controller = CustomsConfig.DongVatNghiepVu, action = "Index" });

                endpoints.MapControllerRoute("DonViNghiepVu", "DonViNghiepVu/{PhanLoai}", new { controller = CustomsConfig.DonViNghiepVu, action = "Index" });
                #endregion

                #region Trang chủ
                endpoints.MapControllerRoute("Home",
                    "trang-chu",
                    new { controller = "Home", action = CustomsConfig.TrangChu });
                #endregion

                #region Base funtion
                endpoints.MapControllerRoute("Login",
                    "dang-nhap",
                    new { controller = "Account", action = CustomsConfig.DangNhap });
                #endregion

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=IndexBid}/{id?}");


            });
        }

        /// <summary>
        /// Update Permission table
        /// </summary>
        /// <returns></returns>
        private void UpdatePermissonTable(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var ct = serviceScope.ServiceProvider.GetRequiredService<ESDContext>();
                //Get Module
                var moduleHasCode = ct.Module.Where(s => s.Code != 0).ToList();
                if (moduleHasCode.Count == 0)
                    return;
                var lstPermission = new List<Permission>();
                //Ma trận phân quyền
                var dictMockData = new Dictionary<int, List<EnumPermission.Type>>();
                dictMockData = MapEnum.MatrixPermission;
                foreach (var module in moduleHasCode)
                {
                    var dicType = dictMockData.GetValueOrDefault(module.Code);
                    if (dicType != null && dicType.Count > 0)
                    {
                        foreach (var type in dicType)
                        {
                            lstPermission.Add(new Permission
                            {
                                IDModule = module.ID,
                                Name = StringUltils.GetEnumDescription(type),
                                Type = (int)type
                            });
                        }
                    }
                }
                var oldPer = ct.Permission.ToList();
                var listDelete = oldPer.Where(x => !lstPermission.Any(n => n.IDModule == x.IDModule && n.Type == x.Type)).ToList();
                var listInsert = lstPermission.Where(x => !oldPer.Any(n => n.IDModule == x.IDModule && n.Type == x.Type)).ToList();

                if (listDelete.Count > 0)
                {
                    ct.Permission.RemoveRange(listDelete);
                }

                if (listInsert.Count > 0)
                {
                    ct.Permission.AddRange(listInsert);
                }
                ct.SaveChanges();
            }
        }
    }
}
