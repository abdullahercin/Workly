using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Workly.Application.DTOs.Users;
using Workly.Application.Interfaces;
using Workly.Domain.Entities;
using Workly.Domain.Exceptions;
using Workly.Domain.Interfaces;

namespace Workly.Application.Services
{
    public class UserService(IUserRepository repository, IMailService mailService, ILogger<UserService> logger, IConfiguration config) : IUserService
    {
        private readonly string baseApiUrl = config.GetValue<string>("BaseApiUrl");
        //Yeni kullanıcı oluşturur
        public async Task<int> CreateUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
        {
            //Kullanıcının mail adresi daha önce kullanılmış mı diye kontrol et, kullanılmışsa hata fırlat
            if (await IsEmailExistAsync(registerUserDto.Email, cancellationToken))
            {
                throw new UserAlreadyExistException(registerUserDto.Email);
            }

            // Kullanıcı oluşturma
            var user = new User
            {
                Email = registerUserDto.Email,
                PasswordHash = HashPassword(registerUserDto.Password),
                Name = registerUserDto.Name,
                PhoneNumber = registerUserDto.PhoneNumber,
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            var userId = await repository.AddAsync(user, cancellationToken);

            //Kullanıcıya doğrulama maili gönder
            try
            {
                var confirmationLink = $"{baseApiUrl}users/confirm-email?token={user.EmailConfirmationToken}";
                var emailBody = $"Merhaba {user.Name}, lütfen hesabınızı doğrulamak için <a href=\"{confirmationLink}\">bu bağlantıyı</a> tıklayın.";
                await mailService.SendEmailAsync(user.Email, "Hesabınızı Doğrulayın", emailBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Doğrulama maili gönderilirken hata oluştu.");
            }

            return userId;
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

        public async Task<bool> ConfirmEmailAsync(string token, CancellationToken cancellationToken = default)
        {
            var user = await repository.GetUserByEmailTokenAsync(token, cancellationToken);

            if (user == null || user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
            {
                throw new Exception("Token geçersiz veya süresi dolmuş.");
            }

            // User nesnesinin alanlarını manuel olarak güncelle
            user.IsEmailConfirmed = true;
            user.IsActive = true;

            await repository.UpdateAsync(user, cancellationToken);

            return true;
        }
    }
}
