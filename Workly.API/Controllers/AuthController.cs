using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workly.Application.DTOs.Auth;
using Workly.Application.Interfaces.Services;
using Workly.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Workly.API.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Net;
using Workly.Application.Interfaces;
using Workly.Infrastructure.Services;
using Workly.Application.DTOs.Email;

namespace Workly.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;
        private readonly IMailService _emailService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            ILogger<AuthController> logger,
            IConfiguration config,
            IMailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
            _config = config;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed for email {Email}: User not found", loginDto.Email);
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Email veya şifre hatalı"));
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt failed for email {Email}: Account not active", loginDto.Email);
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Hesabınız aktif değil"));
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login attempt failed for email {Email}: Invalid password", loginDto.Email);
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Email veya şifre hatalı"));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user, roles);
                var refreshToken = _tokenService.CreateRefreshToken();

                user.RefreshToken = refreshToken.Token;
                user.RefreshTokenExpiry = refreshToken.Expires;
                user.LastLoginAt = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update user {UserId} with refresh token", user.Id);
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Giriş işlemi sırasında bir hata oluştu"));
                }

                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                return Ok(ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
                {
                    Token = token,
                    Email = user.Email!,
                    UserName = user.UserName ?? user.Email!
                }, "Giriş başarılı"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return Ok(ApiResponse<AuthResponseDto>.Fail("Bir hata oluştu"));
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken(string token)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(token);
                if (principal == null)
                {
                    _logger.LogWarning("Invalid access token provided for refresh");
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Geçersiz token"));
                }

                var email = principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email!);

                if (user == null || 
                    user.RefreshToken == null || 
                    user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Invalid refresh token for user {Email}", email);
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Geçersiz oturum"));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var newToken = _tokenService.CreateToken(user, roles);
                var newRefreshToken = _tokenService.CreateRefreshToken();

                user.RefreshToken = newRefreshToken.Token;
                user.RefreshTokenExpiry = newRefreshToken.Expires;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user {UserId} with new refresh token", user.Id);
                    return Ok(ApiResponse<AuthResponseDto>.Fail("Token yenileme sırasında bir hata oluştu"));
                }

                _logger.LogInformation("Token refreshed successfully for user {Email}", user.Email);

                return Ok(ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
                {
                    Token = newToken,
                    Email = user.Email!,
                    UserName = user.UserName ?? user.Email!
                }, "Token yenilendi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during token refresh");
                return Ok(ApiResponse<AuthResponseDto>.Fail("Bir hata oluştu"));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Logout attempt failed: No email claim found in token");
                    return Ok(ApiResponse<object>.Fail("Oturum bilgisi bulunamadı"));
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Logout attempt failed: User not found for email {Email}", email);
                    return Ok(ApiResponse<object>.Fail("Kullanıcı bulunamadı"));
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user {UserId} during logout", user.Id);
                    return Ok(ApiResponse<object>.Fail("Çıkış işlemi sırasında bir hata oluştu"));
                }

                _logger.LogInformation("User {Email} logged out successfully", email);
                return Ok(ApiResponse<object>.Success(null, "Başarıyla çıkış yapıldı"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout");
                return Ok(ApiResponse<object>.Fail("Çıkış işlemi sırasında bir hata oluştu"));
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset attempted for non-existent email: {Email}", email);
                    return Ok(ApiResponse<string>.Fail($"{email} kullanıcısı bulunamadı.")); // Güvenlik için aynı mesaj
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                
                var resetLink = $"{_config["AppSettings:ClientBaseUrl"]}/reset-password?email={WebUtility.UrlEncode(email)}&token={encodedToken}";

                var emailModel = new EmailModel
                {
                    To = email,
                    Subject = "Şifre Sıfırlama",
                    Body = $@"
                        <h3>Şifre Sıfırlama Talebi</h3>
                        <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                        <p><a href='{resetLink}'>Şifremi Sıfırla</a></p>
                        <p>Bu bağlantı 30 dakika süreyle geçerlidir.</p>
                        <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>"
                };

                await _emailService.SendEmailAsync(emailModel);
                
                _logger.LogInformation("Password reset email sent to {Email}", email);
                return Ok(ApiResponse<object>.Success(null, "Şifre sıfırlama bağlantısı gönderildi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during password reset request");
                return Ok(ApiResponse<object>.Fail("Şifre sıfırlama bağlantısı gönderilirken bir hata oluştu"));
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset attempted for non-existent email: {Email}", resetPasswordDto.Email);
                    return Ok(ApiResponse<object>.Fail("Geçersiz istek"));
                }

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.Token));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _logger.LogWarning("Password reset failed for {Email}: {Errors}", resetPasswordDto.Email, string.Join(", ", errors));
                    return Ok(ApiResponse<object>.Fail("Şifre sıfırlama başarısız oldu", errors));
                }

                // Tüm refresh tokenları temizle (güvenlik için)
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Password reset successful for {Email}", resetPasswordDto.Email);
                return Ok(ApiResponse<object>.Success(null, "Şifreniz başarıyla değiştirildi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during password reset");
                return Ok(ApiResponse<object>.Fail("Şifre sıfırlama sırasında bir hata oluştu"));
            }
        }
    }
}
