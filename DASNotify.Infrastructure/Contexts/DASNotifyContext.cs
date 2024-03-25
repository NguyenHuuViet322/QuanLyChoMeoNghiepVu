using DASNotify.Domain.Models.DASNotify;
using ESD.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ESD.Infrastructure.Constants;

namespace DASNotify.Infrastructure.Contexts
{
    public class DASNotifyContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DASNotifyContext(DbContextOptions<DASNotifyContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DASNotifyContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseOracle(_configuration.GetConnectionString("DASNotify"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            //builder.Entity<Job>().HasIndex(u => new { u.Name, u.ProjectId }).IsUnique();
            //builder.Entity<Project>().HasIndex(u => u.Name).IsUnique();
           
        }

        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationPortal> NotificationPortal { get; set; }
        // public DbSet<NotificationType> NotificationType { get; set; }
    }


}
