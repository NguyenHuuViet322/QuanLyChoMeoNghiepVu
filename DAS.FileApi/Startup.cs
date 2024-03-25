using System.Text;
using ESD.Application.AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Services;
using ESD.Domain.Interfaces.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Infrastructure.Contexts;
using ESD.Infrastructure.Repositories.DAS;
using ESD.Utility;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DAS.FileApi
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
            services.AddDbContext<ESDContext>(o => o.UseSqlServer(ConfigUtils.GetConnectionString("DASContext")));
            services.AddSingleton<ILoggerManager, LoggerManager>();

            // Config HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers();

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
                    ValidAudience = Configuration["Tokens:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                    //Do not check the expiry of token
                    ValidateLifetime = false
                };
            });

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
                    Title = "DAS FILE API",
                    Version = "v1",
                    Description = "A DAS File API",
                    Contact = new OpenApiContact
                    {
                        Name = "huytd",
                        Email = "huytd@fsivietnam.com.vn"
                    }
                });
            });

            //Inject repos
            services.AddScoped<IDasRepositoryWrapper, DasRepositoryWrapper>();
            services.AddScoped<IUserPrincipalService, UserPrincipalService>();

            //Inject logic services
            services.AddScoped<IStgFileService, StgFileService>();

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
    }
}