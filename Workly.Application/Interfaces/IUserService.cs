using Workly.Application.DTOs.Users;

namespace Workly.Application.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default);
    }
}
