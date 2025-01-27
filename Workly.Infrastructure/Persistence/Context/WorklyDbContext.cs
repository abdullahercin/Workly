using Microsoft.EntityFrameworkCore;

namespace Workly.Infrastructure.Persistence.Context
{
    public class WorklyDbContext(DbContextOptions<WorklyDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorklyDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Domain.Entities.Users.User> User { get; set; }
    }
}
