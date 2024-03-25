using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.CustomModels;
using ESD.Domain.Models.DAS;
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
    public class ESDContext : DbContext
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

        public ESDContext(DbContextOptions<ESDContext> options, IUserPrincipalService userPrincipalService, ILogBySqlRepository logBySql) : base(options)
        {
            _userPrincipalService = userPrincipalService;
            _logBySql = logBySql;
        }

        public ESDContext() : base()
        {
        }

        public ESDContext(IUserPrincipalService userPrincipalService) : base()
        {
            _userPrincipalService = userPrincipalService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(loggerFactory).UseSqlServer(ConfigUtils.GetConnectionString("DASContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            //builder.Entity<Job>().HasIndex(u => new { u.Name, u.ProjectId }).IsUnique();
            //builder.Entity<Project>().HasIndex(u => u.Name).IsUnique();
            // builder.Entity<StgFile>().Property(p => p.Size).HasColumnType("decimal(18,2)");
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

        public DbSet<Agency> Agency { get; set; }
        public DbSet<CatalogingDoc> CatalogingDoc { get; set; }
        public DbSet<CatalogingDocField> CatalogingDocField { get; set; }
        public DbSet<CatalogingProfile> CatalogingProfile { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryField> CategoryField { get; set; }
        public DbSet<CategoryType> CategoryType { get; set; }
        public DbSet<CategoryTypeField> CategoryTypeField { get; set; }
        public DbSet<CodeBox> CodeBox { get; set; }
        public DbSet<DataType> DataType { get; set; }
        public DbSet<DeliveryRecord> DeliveryRecord { get; set; }
        public DbSet<Doc> Doc { get; set; }
        public DbSet<DocField> DocField { get; set; }
        public DbSet<DocType> DocType { get; set; }
        public DbSet<DocTypeField> DocTypeField { get; set; }
        public DbSet<DownloadLink> DownloadLink { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<ExpiryDate> ExpiryDate { get; set; }
        public DbSet<GroupPermission> GroupPermission { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<ModuleChild> ModuleChild { get; set; }
        public DbSet<Organ> Organ { get; set; }
        public DbSet<OrganConfig> OrganConfig { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<PermissionGroupPer> PermissionGroupPer { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<PlanAgency> PlanAgency { get; set; }
        public DbSet<PlanProfile> PlanProfile { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<ProfileList> ProfileList { get; set; }
        public DbSet<ProfileTemplate> ProfileTemplate { get; set; }
        public DbSet<Reader> Reader { get; set; }
        public DbSet<ReaderInOrgan> ReaderInOrgan { get; set; }
        public DbSet<ResetPassword> ResetPassword { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleGroupPer> RoleGroupPer { get; set; }
        public DbSet<SercureLevel> SercureLevel { get; set; }
        public DbSet<StgFile> StgFile { get; set; }
        public DbSet<Storage> Storage { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<TeamGroupPer> TeamGroupPer { get; set; }
        public DbSet<TeamRole> TeamRole { get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<TemplateParam> TemplateParam { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserGroupPer> UserGroupPer { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<UserTeam> UserTeam { get; set; }
        public DbSet<CatalogingBorrow> CatalogingBorrow { get; set; }
        public DbSet<CatalogingBorrowDoc> CatalogingBorrowDoc { get; set; }
        public DbSet<UserBookmark> UserBookmark { get; set; }
        public DbSet<DestructionProfile> DestructionProfile { get; set; }
        public DbSet<ProfileDestroyed> ProfileDestroyed { get; set; }
        public DbSet<LogSystemCRUD> LogSystemCRUD { get; set; }
        public DbSet<LogUserAction> LogUserAction { get; set; }

        //RenderHere
    }
}