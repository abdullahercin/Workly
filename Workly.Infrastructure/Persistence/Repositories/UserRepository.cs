using Workly.Application.Interfaces;
using Workly.Domain.Entities;
using Workly.Domain.Interfaces;
using Workly.Infrastructure.Persistence.Context;

namespace Workly.Infrastructure.Persistence.Repositories
{
    public class UserRepository(WorklyDbContext dbContext, IUserContext userContext)
        : GenericRepository<User>(dbContext, userContext), IUserRepository;
}
