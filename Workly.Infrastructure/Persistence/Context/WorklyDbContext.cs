using Microsoft.EntityFrameworkCore;
using Workly.Domain.Entities;

namespace Workly.Infrastructure.Persistence.Context
{
    public class WorklyDbContext(DbContextOptions<WorklyDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorklyDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> User { get; set; }
    }
}
