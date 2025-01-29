using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Workly.Domain.Entities;

namespace Workly.Infrastructure.Persistence.Context
{
    public class WorklyDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public WorklyDbContext(DbContextOptions<WorklyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Identity tablolarının isimlerini özelleştirme
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserToken");

            // Tüm konfigürasyonları uygula (UserConfiguration dahil)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        // DbSet tanımlaması opsiyonel, IdentityDbContext zaten Users property'sini sağlıyor
        // public DbSet<User> Users => Set<User>();
    }
}
