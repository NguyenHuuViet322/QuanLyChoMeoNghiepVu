using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.CustomModels;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure;
using ESD.Infrastructure.Constants;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ESD.Infrastructure.Contexts
{
    public class ESDNGHIEPVUContext : DbContext
    {
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ILogBySqlRepository _logBySql;

        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                   .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Warning)
                   .AddFilter(DbLoggerCategory.Query.Name, LogLevel.Debug)
                   .AddConsole();
        }
        );

        public ESDNGHIEPVUContext(DbContextOptions<ESDNGHIEPVUContext> options, IUserPrincipalService userPrincipalService, ILogBySqlRepository logBySql) : base(options)
        {
            _userPrincipalService = userPrincipalService;
            _logBySql = logBySql;
        }

        public ESDNGHIEPVUContext() : base()
        {
        }

        public ESDNGHIEPVUContext(IUserPrincipalService userPrincipalService) : base()
        {
            _userPrincipalService = userPrincipalService;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(ConfigUtils.GetConnectionString("DASNghiepVu"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            //builder.Entity<Job>().HasIndex(u => new { u.Name, u.ProjectId }).IsUnique();
            //builder.Entity<Project>().HasIndex(u => u.Name).IsUnique();
        }

        private object CreateWithValues(EntityEntry values)
        {
            object entity = Activator.CreateInstance(values.Entity.GetType());
            foreach (var property in values.Entity.GetType().GetProperties())
            {
                //var property = type.GetProperty(propname.);
                property.SetValue(entity, values.Property(property.Name).OriginalValue);
            }

            return entity;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {


            try
            {
                var modifiedEntries = ChangeTracker.Entries()
                    .Where(x => x.Entity is IAuditable
                                && (x.State == EntityState.Added || x.State == EntityState.Modified)).ToList();

                if (modifiedEntries.Any())
                {
                    foreach (var entry in modifiedEntries)
                    {
                        var entity = entry.Entity as IAuditable;
                        if (entity == null) continue;

                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedBy = _userPrincipalService.UserId;
                            entity.CreateDate = DateTime.UtcNow;
                        }
                        else
                        {
                            entity.UpdatedBy = _userPrincipalService.UserId;
                            entity.UpdatedDate = DateTime.UtcNow;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
            //Log
            if (ConfigUtils.GetKeyValue("LogConfig", "LogCRUD") == "true")
            {
                try
                {
                    ChangeTracker.DetectChanges();
                    var changeEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).ToList();
                    if (changeEntries.Any())
                    {
                        foreach (var entry in changeEntries)
                        {
                            var entityObj = entry.Entity;
                            LogInfo logInfo;
                            switch (entry.State)
                            {
                                case EntityState.Deleted:
                                    logInfo = new LogInfo(entityObj.GetType().GetProperty("ID") != null ? entityObj.GetType().GetProperty("ID").GetValue(entityObj).ToString() : string.Empty, entityObj.GetType().Name, LogStateConst.DELETE, entityObj, null);
                                    await _logBySql.InsertCRUDLog(logInfo);
                                    break;
                                case EntityState.Modified:
                                    if (entityObj.GetType().GetProperty("Status") != null && entityObj.GetType().GetProperty("Status").GetValue(entityObj).ToString() == "0")
                                        logInfo = new LogInfo(entityObj.GetType().GetProperty("ID") != null ? entityObj.GetType().GetProperty("ID").GetValue(entityObj).ToString() : string.Empty, entityObj.GetType().Name, LogStateConst.DELETELOGIC, CreateWithValues(entry), entityObj);
                                    else
                                        logInfo = new LogInfo(entityObj.GetType().GetProperty("ID") != null ? entityObj.GetType().GetProperty("ID").GetValue(entityObj).ToString() : string.Empty, entityObj.GetType().Name, LogStateConst.UPDATE, CreateWithValues(entry), entityObj);

                                    await _logBySql.InsertCRUDLog(logInfo);

                                    break;
                                case EntityState.Added:
                                    logInfo = new LogInfo(entityObj.GetType().GetProperty("ID") != null ? entityObj.GetType().GetProperty("ID").GetValue(entityObj).ToString() : string.Empty, entityObj.GetType().Name, LogStateConst.INSERT, null, entityObj);
                                    await _logBySql.InsertCRUDLog(logInfo);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return await base.SaveChangesAsync(true, cancellationToken);
        }

        public DbSet<ChuyenMonKiThuat> ChuyenMonKiThuat { get; set; }

        public DbSet<CoSoVatChat> CoSoVatChat { get; set; }

        public DbSet<CoSoVatChat_DonVi> CoSoVatChat_DonVi { get; set; }

        public DbSet<DongVatNghiepVu> DongVatNghiepVu { get; set; }

        public DbSet<DonViNghiepVu> DonViNghiepVu { get; set; }

        public DbSet<LoaiChoNghiepVu> LoaiChoNghiepVu { get; set; }

        public DbSet<NghiepVuDongVat> NghiepVuDongVat { get; set; }

        public DbSet<NghiepVuDongVat_DinhKem> NghiepVuDongVat_DinhKem { get; set; }

        public DbSet<ThongTinCanBo> ThongTinCanBo { get; set; }

        public DbSet<TCDinhLuongAnChoNV> TCDinhLuongAnChoNV { get; set; }

        public DbSet<TCDMTrangBi_DonVi> TCDMTrangBi_DonVi { get; set; }

        public DbSet<TCDMTrangBiCBCS_ChoNV> TCDMTrangBiCBCS_ChoNV { get; set; }

        public DbSet<TCDMTrangBiChoNV> TCDMTrangBiChoNV { get; set; }

        //RenderHere
    }
}