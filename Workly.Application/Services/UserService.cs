using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Workly.Application.DTOs.Users;
using Workly.Application.Interfaces;
using Workly.Domain.Entities;
using Workly.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Workly.Application.Services
{
    public class UserService(
        UserManager<User> userManager, 
        IMailService mailService, 
        ILogger<UserService> logger, 
        IConfiguration config) : IUserService
    {
        private readonly string baseApiUrl = config.GetValue<string>("BaseApiUrl");

        public async Task<int> CreateUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                Email = registerUserDto.Email,
                UserName = registerUserDto.Email, // Identity için gerekli
                Name = registerUserDto.Name,
                PhoneNumber = registerUserDto.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, registerUserDto.Password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"Kullanıcı oluşturma hatası: {errors}");
            }

            try
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token)));
                var confirmationLink = $"{baseApiUrl}users/confirm-email?userId={user.Id}&token={encodedToken}";
                var emailBody = $"Merhaba {user.Name}, lütfen hesabınızı doğrulamak için <a href=\"{confirmationLink}\">bu bağlantıyı</a> tıklayın.";
                await mailService.SendEmailAsync(user.Email, "Hesabınızı Doğrulayın", emailBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Doğrulama maili gönderilirken hata oluştu.");
            }

            return user.Id;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            try 
            {
                var decodedToken = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(WebUtility.UrlDecode(token)));
                
                var result = await userManager.ConfirmEmailAsync(user, decodedToken);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("Email doğrulama hatası: {Errors}", errors);
                    throw new Exception($"E-posta doğrulama işlemi başarısız oldu: {errors}");
                }

                user.IsActive = true;
                await userManager.UpdateAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Token decode edilirken veya doğrulama sırasında hata oluştu");
                throw;
            }
        }
    }
}
