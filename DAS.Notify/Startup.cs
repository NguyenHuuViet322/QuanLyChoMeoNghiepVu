using DASNotify.Infrastructure.Contexts;
using DAS.Notify.CustomHub;
using DAS.Notify.CustomHub.WebHub;
using ESD.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using DAS.Notify.CustomHub.CommonHub;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DASNotify.Domain.Interfaces.DASNotify;
using DASNotify.Infrastructure.Repositories.DASNotify;
using DASNotify.Application.Services;
using DASNotify.Application.Interfaces;
using DASNotify.Application.AutoMapper;

namespace DAS.Notify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //init
            services.AddScoped<IDasNotifyRepositoryWrapper, DasNotifyRepositoryWrapper>();
            //Inject logic services
            services.DependencyInjectionService();
            services.AddControllers();
            services.AddDbContext<DASNotifyContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DASNotify")));
            services.AddCors();
            services.AddSignalR();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    ValidAudience = Configuration["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                    //Do not check the expiry of token
                    ValidateLifetime = false
                };
            });
            //AutoMapper
            DasAutoMapper.Configure(services);
            //services.AddScoped<IDasRepositoryWrapper, DasRepositoryWrapper>();
            //  services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<ConnectionManager>();
            services.AddTransient<Func<string, IConnectionManager>>(serviceProvider => key =>
            {
                return key switch
                {
                    "DasWeb" => serviceProvider.GetService<ConnectionManager>(),
                    "DasPortal" => serviceProvider.GetService<ConnectionManager>(),
                    _ => throw new Exception($"No service registered for IConnectionManager"),
                };
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<HubNotifyHelper>();
            services.AddSingleton<HubCommonHelper>();
            services.AddTransient<Func<string, IHubNotifyHelper>>(serviceProvider => key =>
            {
                return key switch
                {
                    "DasWeb" => serviceProvider.GetService<HubNotifyHelper>(),
                    "DasPortal" => serviceProvider.GetService<HubNotifyHelper>(),
                    _ => throw new Exception($"No service registered for IHubNotifyHelper"),
                };
            });
            services.AddTransient<Func<string, IHubCommonHelper>>(serviceProvider => key =>
            {
                return key switch
                {
                    "DasWeb" => serviceProvider.GetService<HubCommonHelper>(),
                    "DasPortal" => serviceProvider.GetService<HubCommonHelper>(),
                    _ => throw new Exception($"No service registered for IHubCommonHelper"),
                };
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ADC Notify API",
                    Version = "v1",
                    Description = "A ADC Notify API",
                    Contact = new OpenApiContact
                    {
                        Name = "adcpv",
                        Email = "adc@eds.com.vn"
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

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
                endpoints.MapHub<NotifyHub>("/NotificationHub");
                endpoints.MapHub<CommonHub>("/CommonHub");
            });
        }
    }
}
