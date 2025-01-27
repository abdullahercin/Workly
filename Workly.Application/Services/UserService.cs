using System.Security.Cryptography;
using System.Text;
using Workly.Application.DTOs.Users;
using Workly.Application.Interfaces;
using Workly.Domain.Entities;
using Workly.Domain.Interfaces;

namespace Workly.Application.Services
{
    public class UserService(IUserRepository repository) : IUserService
    {
        //Yeni kullanıcı oluşturur
        public async Task<int> CreateUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
        {
            //Kullanıcının mail adresi daha önce kullanılmış mı diye kontrol et, kullanılmışsa hata fırlat
            if (await IsEmailExistAsync(registerUserDto.Email, cancellationToken))
            {
                throw new Exception("Bu mail adresi daha önce kullanılmış.");
            }

            // Kullanıcı oluşturma
            var user = new User
            {
                Email = registerUserDto.Email,
                PasswordHash = HashPassword(registerUserDto.Password),
                Name = registerUserDto.Name,
                PhoneNumber = registerUserDto.PhoneNumber,
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24),
            };

            return await repository.AddAsync(user, cancellationToken);
        }

        //Kullanıcının mail adresi daha önce kullanılmış mı diye kontrol et
        private async Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken = default)
        {
            var isEmailExist = await repository.WhereAsync(x => x.Email == email, cancellationToken);
            return isEmailExist.Any();
        }

        private static string HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
