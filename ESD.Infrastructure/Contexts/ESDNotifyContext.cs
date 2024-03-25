using ESD.Domain.Models.DASNotify;
using ESD.Infrastructure.Constants;
using ESD.Utility;
using Microsoft.EntityFrameworkCore;

namespace ESD.Infrastructure.Contexts
{
    public class ESDNotifyContext: DbContext
    {
        public ESDNotifyContext(DbContextOptions<ESDNotifyContext> options) : base(options)
        {
        }

        public ESDNotifyContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(ConfigUtils.GetConnectionString("DASNotify"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            //builder.Entity<Job>().HasIndex(u => new { u.Name, u.ProjectId }).IsUnique();
            //builder.Entity<Project>().HasIndex(u => u.Name).IsUnique();
        }

        public DbSet<Notification> Notification { get; set; }
       // public DbSet<NotificationType> NotificationType { get; set; }
    }

   
}
