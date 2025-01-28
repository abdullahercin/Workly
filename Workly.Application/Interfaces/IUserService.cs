using Workly.Application.DTOs.Users;

namespace Workly.Application.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default);
        Task<bool> ConfirmEmailAsync(string token, CancellationToken cancellationToken = default);
    }
}
