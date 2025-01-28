using Workly.Domain.Entities;

namespace Workly.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailTokenAsync(string emailConfirmToken, CancellationToken cancellationToken = default);
    }
}
