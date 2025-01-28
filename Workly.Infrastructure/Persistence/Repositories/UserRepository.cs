using Microsoft.EntityFrameworkCore;
using Workly.Application.Interfaces;
using Workly.Domain.Entities;
using Workly.Domain.Interfaces;
using Workly.Infrastructure.Persistence.Context;

namespace Workly.Infrastructure.Persistence.Repositories
{
    public class UserRepository(WorklyDbContext dbContext, IUserContext userContext)
        : GenericRepository<User>(dbContext, userContext), IUserRepository
    {
        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
            return result;
        }

        public async Task<User?> GetUserByEmailTokenAsync(string emailConfirmToken, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.FirstOrDefaultAsync(x => x.EmailConfirmationToken == emailConfirmToken, cancellationToken);
            return result;
        }
    }
}
